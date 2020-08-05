using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using io.newgrounds.objects;
using UnityEngine.Networking;

namespace io.newgrounds {
	
	/// <summary>
	/// A helper class for getting login sessions from Newgrounds Passport.
	/// </summary>
	public class SessionLoader {

		/* Plugin methods for WebGL builds */

		[DllImport("__Internal")]
		private static extern bool newgroundsioPluginExists();

		[DllImport("__Internal")]
		private static extern bool newgroundsioAddPluginScripts();

		[DllImport("__Internal")]
		private static extern bool newgroundsioPromptIsOpen();
		
		[DllImport("__Internal")]
		private static extern bool newgroundsioOpenPassportPrompt(string url);
		
		[DllImport("__Internal")]
		private static extern bool newgroundsioClosePassportPrompt();
		
		[DllImport("__Internal")]
		private static extern bool newgroundsioUserCancelledPrompt();
		
		[DllImport("__Internal")]
		private static extern void newgroundsioSaveSessionId(string id);
		
		[DllImport("__Internal")]
		private static extern string newgroundsioLoadSessionId();
		
		public const string SESSION_STATUS_CHANGED = "session-status-changed";
		
		private static int plugin_exists = -1;
		private bool first_check = true;
		private string last_status = SessionResult.SESSION_EXPIRED;
		
		public static bool ngioPluginLoaded()
		{
			if (plugin_exists > -1) return plugin_exists == 1;
			
			bool exists = true;
			try
			{
				exists = newgroundsioPluginExists();
			}
			catch (Exception e)
			{
				// this is just to hack around a warning about 'e' being assigned but unused.
				if (!string.IsNullOrEmpty(e.Message) || string.IsNullOrEmpty(e.Message)) {
					exists = false;
				}
			}
			
			plugin_exists = exists ? 1 : 0;
			return exists;
		}
		
		private Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
		private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
		
		private core _core;
		private session _session;
		private string _session_pref = "__ngio_app_session_id__";

		public SessionLoader(core core)
		{
			_core = core;
			session initial_session = new session();
			
			string session_id = getSessionIdFromURL();

            if (ngioPluginLoaded())
            {
				newgroundsioAddPluginScripts();

				if (string.IsNullOrEmpty(session_id)) session_id = getSessionFromLocalStorage();
            }
            else
            {
                if (UnityEngine.PlayerPrefs.HasKey(_session_pref))
                {
                    session_id = UnityEngine.PlayerPrefs.GetString(_session_pref);
                }
            }

			if (string.IsNullOrEmpty(session_id)) session_id = core.session_id;
			
			if (!string.IsNullOrEmpty(session_id))
			{
				initial_session.id = session_id;
			}
			else
			{
				initial_session.expired = true;
			}
			
			session = initial_session;
			
			stopwatch.Start();
		}
		
		/// <summary>
		/// Adds a function to an event table. This function will be called anytime the associated eventType is triggered.
		/// </summary>
		/// <param name="eventType">One of the following consts from io.newgrounds.SessionResult: SESSION_EXPIRED, USER_LOADED, REQUEST_LOGIN</param>
		/// <param name="handler">A handler function to call whenever this event is triggered.</param>
		public void addEventListener(string eventType, onCallResult<SessionResult> handler)
		{
			lock (eventTable)
			{
				if (!eventTable.ContainsKey(eventType)) eventTable.Add(eventType, null);
				
				eventTable[eventType] = (onCallResult<SessionResult>)eventTable[eventType] + handler;
			}
		}
		
		/// <summary>
		/// Removes a callback function you have already added to the event table.
		/// </summary>
		/// <param name="eventType">One of the following consts from io.newgrounds.SessionResult: SESSION_EXPIRED, USER_LOADED, REQUEST_LOGIN</param>
		/// <param name="handler">The handler function you want to remove from the event table.</param>
		/// <returns></returns>
		public bool removeEventListener(string eventType, onCallResult<SessionResult> handler)
		{
			lock (eventTable)
			{
				if (!eventTable.ContainsKey(eventType)) return false;
				
				eventTable[eventType] = (onCallResult<SessionResult>)eventTable[eventType] - handler;
				
				if (eventTable[eventType] == null)
				{
					eventTable.Remove(eventType);
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Removes all handlers from a specific eventType.
		/// </summary>
		/// <param name="eventType">One of the following consts from io.newgrounds.SessionResult: SESSION_EXPIRED, USER_LOADED, REQUEST_LOGIN</param>
		/// <returns></returns>
		public int removeAllEventListeners(string eventType)
		{
			if (!eventTable.ContainsKey(eventType)) return 0;
			
			int count = eventTable[eventType].GetInvocationList().Length;
			eventTable[eventType] = null;
			eventTable.Remove(eventType);
			
			return count;
		}
		
		/// <summary>
		/// Calls all handler functions in the event table attached to the given eventType.
		/// </summary>
		/// <param name="eventType">The eventType to trigger</param>
		/// <param name="data">Any object to pass to the handler function(s).</param>
		public void dispatchEvent(string eventType, SessionResult data)
		{
			Delegate d;
			
			if (eventTable.TryGetValue(eventType, out d))
			{
				onCallResult<SessionResult> cb = (onCallResult<SessionResult>)d;
				
				if (cb != null) cb(data);
			}
		}
		
		/// <summary>
		/// WebGL only: Looks for session id saved in localStorage
		/// </summary>
		/// <returns></returns>
		public string getSessionFromLocalStorage()
		{
			if (ngioPluginLoaded())
			{
				string si = newgroundsioLoadSessionId();
				if (!string.IsNullOrEmpty(si)) return si;
			}
			return null;
		}
		
		/// <summary>
		/// Looks for a newgrounds session id in the hosted url (only works for WebGL builds published on Newgrounds.com)
		/// </summary>
		/// <returns>If no session id is available, returns null</returns>
		public string getSessionIdFromURL()
		{
			string web_url = Application.absoluteURL;
			if (string.IsNullOrEmpty(web_url)) return null;
			
			web_url = web_url.Replace(((char)0).ToString(), "");
			
			char[] host_query_delim = { '?' };
			char[] pairs_delim = { '&' };
			char[] key_value_delim = { '=' };
			
			string[] host_query = web_url.Split(host_query_delim);
			
			if (host_query.Length < 2) return null;
			
			string[] pairs = host_query[1].Split(pairs_delim);
			if (pairs.Length < 1) return null;
			
			for (int i = 0; i < pairs.Length; i++)
			{
				string[] key_value = pairs[i].Split(key_value_delim);
				
				if (key_value.Length == 2 && UnityWebRequest.UnEscapeURL(key_value[0]) == "ngio_session_id") return UnityWebRequest.UnEscapeURL(key_value[1]);
			}
			
			return null;
		}


		
		public string session_id
		{
			get {
				return _session.id;
			}
		}
		
		public session session
		{
			get
			{
				return _session;
			}
			
			set
			{
				_session = value;
				_core.session_id = _session.id;

				SessionResult sr = new SessionResult();
				sr.session = _session;
				
				if (last_status != sr.getStatus())
				{
					last_status = sr.getStatus();
					dispatchEvent(SESSION_STATUS_CHANGED, sr);
					dispatchEvent(sr.getStatus(), sr);

					if (sr.getStatus() == SessionResult.USER_LOADED && sr.session.remember)
					{
						_saveSessionId(sr.session.id);
					}
					else if (sr.getStatus() == SessionResult.SESSION_EXPIRED)
					{
						_saveSessionId("");
					}
				}
			}
		}

        private void _saveSessionId(string id)
        {
            if (ngioPluginLoaded())
            {
                newgroundsioSaveSessionId(id);
            }
            else
            {
                UnityEngine.PlayerPrefs.SetString(_session_pref, id);
                UnityEngine.PlayerPrefs.Save();
            }
        }
		
		delegate void callDelegate(ResultModel model);
		
		/// <summary>
		/// Creates a new session for your app user
		/// </summary>
		/// <param name="callback"></param>
		public void startSession(Action<SessionResult> callback = null)
		{
            session = new session();

			components.App.startSession component = new components.App.startSession();

			component.callWith(_core, (results.App.startSession r) =>
				{
					SessionResult session_result = (SessionResult)r;

					if (session_result.success)
					{
						session = session_result.session;
					}
					else
					{
						if (session_result.error == null) session_result.error = new error { message = "Could not connect to Newgrounds.io server" };
					}

					if (callback != null) callback(session_result);

				}
			);
		}
		
		/// <summary>
		/// Will check the current status of a session.
		/// </summary>
		/// <param name="callback"></param>
		public void checkSession(Action<SessionResult> callback = null)
		{
			SessionResult sr = new SessionResult();
			sr.session = session;
			
			Action<ResultModel> resultHandler = (ResultModel r) =>
			{
				sr = (SessionResult)r;
				
				if (!r.success)
				{
					if (sr.error == null) sr.error = new error { message = "Could not connect to Newgrounds.io server" };
					if (sr.session == null) sr.session = new session();
				}
				session = sr.session;
				
				if (callback != null) callback(sr);
			};
			
			bool cooled = false;
			
			/* this makes sure we aren't spamming the newgrounds.io server (wich would get us blocked) */
			if (first_check || stopwatch.ElapsedMilliseconds > 3000)
			{
				stopwatch.Reset();
				stopwatch.Start();
				cooled = true;
				first_check = false;
			}
			
			/* if we have an active session and our stopwatch has cooled down, reload the session from the server */
			if (cooled && sr.getStatus() != SessionResult.SESSION_EXPIRED)
			{

				if (ngioPluginLoaded() && newgroundsioUserCancelledPrompt()) {

                	SessionResult csr = new SessionResult();
					csr.setIsLocal(true);
					session ns = new session();
					ns.expired = true;
					ns.id = session.id;
					csr.session = ns;

					csr.success = false;
					csr.error.message = "User cancelled login";
                    if (callback != null) callback(csr);
                    session = new session();

				} else {
					_core.callComponent(
						"App.checkSession",
						null,
						resultHandler
					);
				}
			}
			/* otherwise, just use the currently loaded session */
			else
			{
				if (callback != null) callback(sr);
			}
		}
		
		/// <summary>
		/// Clears the current active session (if any).
		/// </summary>
		/// <param name="callback"></param>
		public void endSession(Action<SessionResult> callback = null)
		{

			if (!string.IsNullOrEmpty(session.id)) _core.callComponent("App.endSession");

			session = new session();
			
			SessionResult sr = new SessionResult();
			sr.session = session;
			
			if (callback != null) callback(sr);
		}
		
		/// <summary>
		/// If a non-expired session has been loaded, this will open Newgrounds Passport in a browser window so the player may log in securely.
		/// At the time this class as written, Unity lacked the ability to open URLs in new tabs when built for WebGL
		/// When this is called from WebGL, a prompt will be drawn over your game asking the player if they want to sign in. 
		/// This is necessary to load passport in conjunction with a mouse click so popup blockers can be bypassed.
		/// </summary>
		/// <returns></returns>
		public bool loadPassport()
		{
			SessionResult sr = new SessionResult();
			sr.session = session;
			
			if (sr.getStatus() != SessionResult.REQUEST_LOGIN) return false;
			
			if (ngioPluginLoaded())
			{
				newgroundsioOpenPassportPrompt(session.passport_url);
				return true;
			} else {
				Application.OpenURL(session.passport_url);
				return true;
			}
		}
		
		public string getStatus()
		{
			return last_status;
		}
	}
}