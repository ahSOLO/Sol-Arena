using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace io.newgrounds
{
	/// <summary>
	/// A generic callback delegate used when calls in <c>io.newgrounds.core</c> makes calls.
	/// </summary>
	/// <param name="result">The results of a call.</param>
	public delegate void onCallResult<T>(T result);
	
	/// <summary>
	/// This is the core class for making calls to the Newgrounds.io server.
	/// </summary>
	public class core : MonoBehaviour
	{
		/// <summary>
		/// The URL we post API calls to.
		/// </summary>
		public static string GATEWAY_URI = "//newgrounds.io/gateway_v3.php";
		//public static string GATEWAY_URI = "//ng-local2.newgrounds.com/ngads/gateway_v3.php";

		/// <summary>
		/// The unique ID of your app as found in the 'API Tools' tab of your Newgrounds.com project.
		/// </summary>
		public string app_id;
		
		/// <summary>
		/// A unique session id used to identify the active user.
		/// </summary>
		public string session_id;
        
        /// <summary>
        /// A base64-encoded, 128-bit AES encryption key.
        /// </summary>
		public string aes_base64_key;

		/// <summary>
		/// If true, network data will be logged to the console.
		/// </summary>
		public bool output_network_data = false;

        private ngCrypto crypto;
		
		private SimpleJSONImportableList call_queue = new SimpleJSONImportableList(typeof(objects.call));
		
		private Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
		
		private SessionLoader sessionLoader;

		private bool _waiting_for_login = false;
		public bool waiting_for_login
		{
			get { return _waiting_for_login;  }
		}

		private Action _onready = null;
        private bool _ready = false;

		/// <summary>
		/// Executes a callback when the core instance has been initialized and is ready to be used.
		/// </summary>
		/// <param name="callback"></param>
		public void onReady(Action callback)
        {
            if (_ready)
            {
				callback();
            }
            else
            {
                _onready = callback;
            }
        }

		/// <summary>
		/// Used to get the current web host of your game.
		/// </summary>
		/// <returns>A web domain, LocalHost or AppView.</returns>
		public static string getHost()
		{
			string web_url = Application.absoluteURL;
			
			if (string.IsNullOrEmpty(web_url)) return "<AppView>";
			if (web_url.StartsWith("file"))	return "<LocalHost>";

			if (web_url.StartsWith("http"))
			{
				Uri uri = new Uri(web_url);
				string host = uri.Host;
				return string.IsNullOrEmpty(host) ? "<Unknown>" : host;
			}
			
			return "<Unknown>";
		}
        
        /* Plugin methods for WebGL builds */
        [DllImport("__Internal")]
        private static extern void newgroundsioOpenUrlInNewTab(string url);

		/// <summary>
		/// Used to open URLs.
		/// </summary>
		/// <param name="url">The URL to open.</param>
		/// <param name="open_in_new_tab">Setto true if you want to URL opened in a new tab. WebGL games will prompt the user before doing so (to get around popup blockers).</param>
		public static void openUrl(string url, bool open_in_new_tab = false)
		{
			// if a new tab is requested and we're in WebGL mode, we need to use our plugin magic
			if (open_in_new_tab && SessionLoader.ngioPluginLoaded())
            {
                newgroundsioOpenUrlInNewTab(url);
			}
			else
			{ 
				Application.OpenURL(url);
			}
		}

		/// <summary>
		/// Starts the monobehavior and fires the onReady callback (if available).
		/// </summary>
        void Start()
        {
            this.sessionLoader = new SessionLoader(this);
            if (!String.IsNullOrEmpty(app_id)) initialize(app_id, aes_base64_key, session_id);

            _ready = true;
            if (_onready != null) _onready();
            _onready = null;
        }

		/// <summary>
		/// When waiting for a user to log in, this will periodically make checks to their session status.
		/// </summary>
        void Update()
        {
            // this will check the server for changes in login status. (This really only sends one request every 3 seconds to limit server impact.)
            if (_waiting_for_login)
            {
                if (getSessionLoader() != null && getSessionLoader().getStatus() == SessionResult.REQUEST_LOGIN)
                {
                    getSessionLoader().checkSession((SessionResult r) =>
                    {
                        if (r.getStatus() == SessionResult.USER_LOADED)
                        {
                            _loginSuccess(r);
                        }
                        else if (r.getStatus() == SessionResult.SESSION_EXPIRED)
                        {
							_loginFailed(r);
                        }
                    });
                }
            }
        }

		private void debug_log(object msg)
		{
			if (output_network_data) Debug.Log(msg);
		}

		/// <summary>
		/// Initializes your core instance
		/// </summary>
		/// <param name="app_id">The unique ID of your app as found in the 'API Tools' tab of your Newgrounds.com project.</param>
		/// <param name="aes_base64_key">Your 128-bit AES encryption key, encoded as a base64 string. This can be found in the 'API Tools' tab of your Newgrounds.com project.</param>
		/// <param name="session_id">If you have a saved session_id, you may set it here ahead of time.</param>
		public void initialize(string app_id, string aes_base64_key, string session_id = null)
		{
			this.app_id = app_id;
			this.crypto = new ngCrypto(aes_base64_key, ngCrypto.CIPHER_AES128, ngCrypto.ENCODE_BASE64);
			if (!string.IsNullOrEmpty(session_id)) this.session_id = session_id;
			this.sessionLoader = new SessionLoader(this);
		}
		
		/// <summary>
		/// Gets the sessionLoader object.
		/// </summary>
		/// <returns></returns>
		public SessionLoader getSessionLoader()
		{
			return this.sessionLoader;
		}
		
		/// <summary>
		/// Adds a function to an event table. This function will be called anytime the associated eventType is triggered.
		/// </summary>
		/// <param name="eventType">The type of event, typically a Newgrounds.io component like "Medal.unlock"</param>
		/// <param name="handler">A handler function to call whenever this event is triggered.</param>
		public void addEventListener<T>(string eventType, onCallResult<T> handler)
		{
			lock (eventTable)
			{
				if (!eventTable.ContainsKey(eventType)) eventTable.Add(eventType, null);
				
				eventTable[eventType] = (onCallResult<T>)eventTable[eventType] + handler;
			}
		}
		
		/// <summary>
		/// Removes a callback function you have already added to the event table.
		/// </summary>
		/// <param name="eventType">The type of event you want to remove a handler from, typically a Newgrounds.io component like "Medal.unlock"</param>
		/// <param name="handler">The handler function you want to remove from the event table.</param>
		/// <returns></returns>
		public bool removeEventListener<T>(string eventType, onCallResult<T> handler)
		{
			lock (eventTable)
			{
				if (!eventTable.ContainsKey(eventType)) return false;
				
				eventTable[eventType] = (onCallResult<T>)eventTable[eventType] - handler;
				
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
		/// <param name="eventType">The type of event you want to clear handlers from, typically a Newgrounds.io component like "Medal.unlock"</param>
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
		public void dispatchEvent<T>(string eventType, T data)
		{
			Delegate d;
			if (eventTable.TryGetValue(eventType, out d))
			{
				onCallResult<T> cb = (onCallResult<T>)d;
				
				if (cb != null) cb(data);
			}
		}

		/// <summary>
		/// Creates a objects.call object using the provided parameters
		/// </summary>
		/// <param name="component"></param>
		/// <param name="parameters"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		private objects.call _createCall(string component, object parameters = null, Action<ResultModel> callback = null)
		{
			objects.call c = new objects.call();
			
			CallModel cm = CallModel.getByComponentName(component);
			
			if (cm == null) throw new ArgumentException("Unknown component \""+component+"\"");
			
			if (parameters != null && (parameters.GetType() == typeof(CallProperties) || parameters.GetType().IsSubclassOf(typeof(CallProperties))))
			{
				cm.setProperties((CallProperties)parameters);
				parameters = cm;
			}
			
			if (parameters != null && parameters.GetType() != cm.GetType())
			{
				throw new ArgumentException("parameters must be set in an io.newgrounds.CallProperties or " + cm.GetType().ToString() + " instance.");
			}

            c.component = component;

            if (cm.secure)
			{
                secureCall scall = new secureCall();
                scall.component = component;
                scall.parameters = parameters;
				string jscall = scall.toJSON();
				debug_log("Created secure call from: " + jscall);
				c.secure = crypto.encrypt(jscall);
			}
			else
			{
				c.parameters = parameters;
			}
			c.callback = callback;
			
			return c;
		}

		/// <summary>
		/// Calls a component on the Newgrounds.io server.
		/// Note: This is an abstract method and you are better served using the callWith() method of any object in the io.newgrounds.components namespace.
		/// </summary>
		/// <param name="component">The component name to call, ie Gateway.ping</param>
		/// <param name="parameters">An object containing any properties you need to send to the component.</param>
		/// <param name="callback">An optional callback action that will run when the server has executed this component call.</param>
		public void callComponent(string component, object parameters=null, Action<ResultModel> callback = null)
		{
			SimpleJSONImportableList single_call = new SimpleJSONImportableList(typeof(objects.call));
			single_call.Add(_createCall(component, parameters, callback));
			StartCoroutine(_executeCallList(single_call));
		}

		/// <summary>
		/// Queues a component to be called later (when executeQueuedComponents is called).
		/// This allows you to execute multiple components on the server in a single request.
		/// Note: There may be limits on the number of components and overall execution time the server will accept.
		/// Also: This is an abstract method and you are better served using the queueWith() method of any object in the io.newgrounds.components namespace.
		/// </summary>
		/// <param name="component">The component name to call, ie Gateway.ping</param>
		/// <param name="parameters">An object containing any properties you need to send to the component.</param>
		/// <param name="callback">An optional callback action that will run when the server has executed this component call.</param>
		public void queueComponent(string component, object parameters=null, Action<ResultModel> callback=null)
		{
			call_queue.Add(_createCall(component, parameters, callback));
		}

		/// <summary>
		/// Executes any components that have been queued.
		/// </summary>
		public void executeQueuedComponents()
		{
			StartCoroutine(_executeCallList(call_queue));
			call_queue.Clear();
		}

		/// This handles the actual transactions with the Newgrounds.io server. This runs as a Coroutine in Unity.
		private IEnumerator _executeCallList(SimpleJSONImportableList call_list)
		{
			objects.input input = new objects.input();
			input.app_id = app_id;
			
			if (!string.IsNullOrEmpty(session_id)) input.session_id = session_id;
			
			SimpleJSONImportableList calls = new SimpleJSONImportableList(typeof(objects.call));
			objects.call vc;
			CallModel cm;
			ResultModel vr;
			
			for (int c = 0; c < call_list.Count; c++)
			{
				vc = (objects.call)call_list[c];
				cm = CallModel.getByComponentName(vc.component);
				
				if (cm.require_session && string.IsNullOrEmpty(session_id))
				{
					vr = (ResultModel)ResultModel.getByComponentName(vc.component);

					vr.setIsLocal(true);
					vr.setCall(vc);
					vr.component = vc.component;
					vr.success = false;
					vr.error.message = vc.component + " requires a valid user session.";
					vr.ngio_core = this;

					debug_log("Call failed local validation:\n"+vc.toJSON());
					debug_log("Local Response:\n" + vr.toJSON());

					if (vc.callback != null)
					{
						vc.callback(vr);
					}
					
					vr.dispatchMe(vc.component);
					continue;
				}
				else
				{
					calls.Add(vc);
				}
			}

			if (calls.Count > 0)
			{

				input.call = calls;
				string json = input.toJSON();
				debug_log("Sent to Server:\n" + json);

				WWWForm webform = new WWWForm();
				webform.AddField("input", json);

                // hopefully, our results will populate to this
                objects.output _output = new objects.output();
                string default_error = "There was a problem connecting to the server at '" + GATEWAY_URI + "'.";

                using (UnityWebRequest json_results = UnityWebRequest.Post(GATEWAY_URI, webform))
                {
                    yield return json_results.SendWebRequest();

                    debug_log("Server Response:\n" + json_results.downloadHandler.text);

                    // we'll try decoding what the server sent back.  If it's valid JSON there should be no problems
                    try
                    {
                        // decode the overall response
                        JObject jobject = JSONDecoder.Decode(json_results.downloadHandler.text);

                        // populate the basic info our output model will need to proceed
                        if (jobject.Kind == JObjectKind.Object)
                        {
                            _output.setPropertiesFromSimpleJSON(jobject);
                        }
                    }
                    // catch any exceptions and update the generic error message we'll spit back.
                    catch (Exception e)
                    {
                        Debug.LogWarning("Caught an exception decoding the data:\n" + e.Message);
                        default_error = e.Message;
                    }
                }
                _output.ngio_core = this;

				// We'll typically only get here if the was a server or connection error.
				if (_output.success != true && _output.error == null)
				{
					_output.error = new objects.error();
					_output.error.message = default_error;
				}

				// cheap way to fill debug info even with debug mode being off
				_output.debug.input = input;

				ResultModel _rm;
				objects.result _result;
				objects.call call;

				for (int i = 0; i < calls.Count; i++)
				{
					call = (objects.call)calls[i];

					if (_output.success != true)
					{
						_rm = new ResultModel();
						_rm.error = _output.error;
					}
					else if (_output.result.GetType() == typeof(SimpleJSONImportableList) && ((SimpleJSONImportableList)_output.result).ElementAtOrDefault<object>(i) != null)
					{
						_result = (objects.result)((SimpleJSONImportableList)_output.result)[i];
						_result.ngio_core = this;

						if (_result.component != call.component)
						{
							_rm = new ResultModel();
							_rm.success = false;
							_rm.error.message = "Unexpected index mismatch in API response!";
						}
						else
						{
							_rm = (ResultModel)_result.data;
						}
					}
					else
					{
						_rm = new ResultModel();
						_rm.success = false;
						_rm.error.message = "Unexpected index mismatch in API response!";
					}

					_rm.ngio_core = this;
					_rm.setCall(call);

					if (call.callback != null)
					{
						call.callback(_rm);
					}

					_rm.dispatchMe(call.component);
				}
			}
		}

		/// <summary>
		/// This will check the server connection, check any version or liscence settings.
		/// If log_view is true and version/license settings pass, a view event will be logged on the server.
		/// </summary>
		/// <param name="log_view">Set to false to skip logging a view event.</param>
		/// <param name="client_version">Set to the version number of your current build.</param>
		/// <param name="callback">An optional callback handler that will fire when the server responds.</param>
		public void connect(bool log_view=true, string client_version=null, Action<connectStatus> callback=null)
		{
			connectStatus status = new connectStatus();
			
			components.App.getHostLicense license = new components.App.getHostLicense();

			components.App.getCurrentVersion version = new components.App.getCurrentVersion();
			version.version = client_version;

			components.App.logView view = new components.App.logView();
			
			int completed_calls = 0;
			int total_calls = 2;

			ResultModel connect_result = null;

			Action onConnectResult = () =>
			{
				if (completed_calls >= total_calls) return;

				status.success = connect_result.success;
				status.error = connect_result.error;

				if (connect_result.success)
				{
					completed_calls++;
				}
				else
				{
					completed_calls = total_calls;
				}

				if (completed_calls >= total_calls)
				{
					if (log_view) view.callWith(this);
					if (callback != null) callback(status);
				}
			};

			license.queueWith(this, (results.App.getHostLicense r) =>
				{
					status.host_approved = r.host_approved;
					connect_result = (ResultModel)r;
					onConnectResult();
				}
			);

			version.queueWith(this, (results.App.getCurrentVersion r) =>
				{
					status.current_version = r.current_version;
					status.client_deprecated = r.client_deprecated;
					connect_result = (ResultModel)r;
					onConnectResult();
				}
			);

			executeQueuedComponents();
		}

        /// <summary>
        /// Checks any current login status.
        /// </summary>
        /// <param name="callback">An optional callback. Will be sent a true/false value indicating the user's login status.</param>
		/// <param name="silent">If true, callbacks set via onLoggedIn, onLoginFailed and onLoginCancelled will not be executed.</param>
        public void checkLogin(Action<bool> callback=null)
        {
            SessionLoader sl = getSessionLoader();
            
            sl.checkSession((SessionResult r) => {
				bool logged_in = (r.getStatus() == SessionResult.USER_LOADED) ? true : false;
                if (callback != null) callback(logged_in);
            });
        }


		/// <summary>
		/// This will request a user's login status.  If the user is not logged in, Newgrounds Passport will be loaded in a new browser.
		/// Note: Due to security sandbox settings, WebGL players will see a prompt asking them if they want to load passport.
		/// </summary>
		public void requestLogin(Action onLoggedIn = null, Action onLoginFailed = null, Action onLoginCancelled=null)
        {
			SessionLoader sl = getSessionLoader();
            _waiting_for_login = false;

            // first, check any existing sessions we may have saved
            sl.checkSession((SessionResult r1) => {

				if (onLoggedIn != null) this.onLoggedIn(onLoggedIn);
				if (onLoginFailed != null) this.onLoginFailed(onLoginFailed);
				if (onLoginCancelled != null) this.onLoginCancelled(onLoginCancelled);

				if (r1.getStatus() == SessionResult.USER_LOADED)
                {
					// if we get here, the user has a valid, saved session
					_loginSuccess(r1);
                }
                else
                {
					// if we get here we need to start a new session
					sl.startSession((SessionResult r2) =>
                    {
						// this shouldn't ever happen unless there's a server issue.
						if (r2.getStatus() == SessionResult.SESSION_EXPIRED)
                        {
							_loginFailed(r2);
                        }

                        // if the user is already considered logged in, we're good to go
                        else if (r2.getStatus() == SessionResult.USER_LOADED)
                        {
							_loginSuccess(r2);
                        }

                        // make a note that we are waiting for the user to log in now (see Update() method)
                        else
                        {
                            sl.loadPassport();
                            _waiting_for_login = true;
                        }
                    });
                }
            });
        }

        /// <summary>
        /// Use this to cancel any pending login events that would be fired after calling requestLogin().
        /// </summary>
        public void cancelLoginRequest()
        {
            _waiting_for_login = false;
            SessionResult sr = new SessionResult();
            sr.session = getSessionLoader().session;

            dispatchEvent<SessionResult>(loginEvent.LOGIN_CANCELLED, sr);
			if (_onlogincancelled != null) _onlogincancelled();
		}

		private Action _onlogout = null;

		/// <summary>
		/// Executes a callback when the user is logged out.
		/// </summary>
		/// <param name="callback"></param>
		public void onLoggedOut(Action callback)
		{
			_onlogout = callback;
		}

		private Action _onloginsuccess = null;

		/// <summary>
		/// Executes a callback when the user is logged in.
		/// Callback receives an io.newgrounds.objects.user object with user details.
		/// </summary>
		/// <param name="callback"></param>
		public void onLoggedIn(Action callback)
		{
			_onloginsuccess = callback;
		}

		private Action _onloginfailed = null;
		private objects.error _login_error = null;

		/// <summary>
		/// This is the last known login error, or null if no errors have been caught.
		/// </summary>
		public objects.error login_error
		{
			get { return _login_error; }
		}

		/// <summary>
		/// Executes a callback when a user login fails. 
		/// Callback receives an io.newgrounds.objects.error object with additional details.
		/// </summary>
		/// <param name="callback"></param>
		public void onLoginFailed(Action callback)
		{
			_onloginfailed = callback;
		}

		private Action _onlogincancelled = null;

		/// <summary>
		/// Executes a callback when the user cancels a login.
		/// </summary>
		/// <param name="callback"></param>
		public void onLoginCancelled(Action callback)
		{
			_onlogincancelled = callback;
		}

		/// <summary>
		/// Ends current user session.
		/// </summary>
		public void logOut()
        {
            // tell the server it can kill the session
            getSessionLoader().endSession();

            // we're manually making a result here so we can proceed independent of the server.
            SessionResult sr = new SessionResult();
            sr.session = getSessionLoader().session;
            sr.success = true;

            dispatchEvent<SessionResult>(loginEvent.LOGGED_OUT, sr);
			if (_onlogout != null) _onlogout();
        }

		/// <summary>
		/// The current, logged in user. If null try using checkLogin() or requestLogin()
		/// </summary>
		public objects.user current_user
		{
			get {
				if (getSessionLoader().session == null || getSessionLoader().session.user == null) return null;
				return getSessionLoader().session.user;
			}
		}

        private void _loginSuccess(SessionResult r)
        {
            dispatchEvent<SessionResult>(loginEvent.LOGIN_SUCCESS, r);
			if (_onloginsuccess != null) _onloginsuccess();
        }

        private void _loginFailed(SessionResult r)
        {
			// code 111 means the user clicked the "No Thanks" 
			if (waiting_for_login && !r.success && r.error.code == 111)
			{
				_login_error = new objects.error();
				_login_error.code = 111;
				_login_error.message = "Session was cancelled by the user";
				cancelLoginRequest();
			}
			else
			{
				_login_error = r.error;
                dispatchEvent<SessionResult>(loginEvent.LOGIN_FAILED, r);
				if (_onloginfailed != null) _onloginfailed();
			}
        }
    }

    /// <summary>
    /// Contains constants used by login events
    /// </summary>
    public class loginEvent
    {
		/// <summary>
		/// Indicates the user has successfully logged in.
		/// </summary>
        public const string LOGIN_SUCCESS = "login-success";

		/// <summary>
		/// Indicates there was a problem logging the user in.
		/// </summary>
        public const string LOGIN_FAILED = "login-failed";

		/// <summary>
		/// Indicates the user cancelled any login prompts.
		/// </summary>
        public const string LOGIN_CANCELLED = "login-cancelled";

		/// <summary>
		/// Indicates the user is logged out.
		/// </summary>
        public const string LOGGED_OUT = "logged-out";
    }
	
	/// <summary>
	/// Contains results for calls to core.connect();
	/// </summary>
	public class connectStatus
	{
		/// <summary>
		/// If false, there was an error connecting to the server. See 'error' object for information.
		/// </summary>
		public bool success = false;

		/// <summary>
		/// If there was an error connecting to the server, details should be available in this object.
		/// </summary>
		public objects.error error = null;

		/// <summary>
		/// The 'current version' number loaded from the server.
		/// </summary>
		public string current_version = null;

		/// <summary>
		/// If true, there is a newer version of your game available. You can use the 'Loader.loadOfficialUrl' component to get the player there.
		/// </summary>
		public bool client_deprecated = true;

		/// <summary>
		/// This will be true if the domain hosting this game is allowed in your server-side license settings.
		/// </summary>
		public bool host_approved = false;
	}
}