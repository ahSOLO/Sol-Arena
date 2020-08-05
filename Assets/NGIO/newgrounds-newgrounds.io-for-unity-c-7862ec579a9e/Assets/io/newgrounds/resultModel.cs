using System;
using UnityEngine;

namespace io.newgrounds
{
	/// <summary>
	/// A simple model for result data sent from the Newgronds.io server.
	/// </summary>
	public class ResultModel : Model
	{
		private string _echo = null;
        private objects.call _call = null;
		private bool _islocal = false;

		/// <summary>
		/// Used to indicate when a resultModel was created locally, not using data from the server. If true, this object was probably created because of an error.
		/// </summary>
		/// <param name="islocal"></param>
		public void setIsLocal(bool islocal) {
			_islocal = islocal;
		}

		/// <summary>
		/// Used to indicate when a resultModel was created locally, not using data from the server. If true, this object was probably created because of an error.
		/// </summary>
		/// <returns></returns>
		public bool getIsLocal() {
			return _islocal;
		}

		public void setCall(objects.call c)
        {
            _call = c;
        }

		/// <summary>
		/// gets the related call object that this result object was generated from.
		/// </summary>
		/// <returns></returns>
        public objects.call getCall()
        {
            return _call;
        }

		/// <summary>
		/// The name of the component this result came from
		/// </summary>
		public string component { get; set; }

		/// <summary>
		/// If an echo value was sent in a corresponding call's parameters, this will have the same value.
		/// </summary>
		public string echo
		{
			get
			{
				return _echo;
			}
			set
			{
				_echo = value;
			}

		}

		/// <summary>
		/// If true, the associated request was successful.
		/// </summary>
		public bool success { get; set; }

		private objects.error _error = new objects.error();

		/// <summary>
		/// This is only set if there was a problem with the request or call.
		/// </summary>
		public objects.error error {
			get
			{
				return _error;
			}
			set
			{
				_error = value;
			}
		}

		/// <summary>
		/// Used to get the appropriate io.newgrounds.results.XXX.YYYY model using the string component name.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public static ResultModel getByComponentName(string component)
		{
			ResultModel model;

			Type modelType = null;
			if (!String.IsNullOrEmpty(component)) modelType = Type.GetType("io.newgrounds.results." + component);
			
			if (modelType == null)
			{
				model = new ResultModel();
			}
			else
			{
				model = (ResultModel)Activator.CreateInstance(modelType);
			}

			return model;
		}

		/// <summary>
		/// Dispatches any events listening for the provided component name.
		/// </summary>
		/// <param name="component"></param>
		public virtual void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<ResultModel>(component, this);
		}
	}
}

/// <summary>
/// Compatable with App.startSession and App.checkSession result models
/// </summary>
namespace io.newgrounds
{
	public class LoaderResult: ResultModel
	{
		public string url { get; set; }
	}

    public class SessionResult : ResultModel
    {
        public const string SESSION_EXPIRED = "session-expired";
        public const string REQUEST_LOGIN = "request-login";
        public const string USER_LOADED = "user-loaded";

        public objects.session __session = new objects.session();
        public objects.session session
        {
            get
            {
                return __session;
            }

            set
            {
                __session = value;
            }
        }


        public override void dispatchMe(string component)
        {
            ngio_core.dispatchEvent<SessionResult>(component, this);
        }

        public string getStatus()
        {
            if (session == null || session.expired || string.IsNullOrEmpty(session.id)) return SESSION_EXPIRED;
            if (session.user != null && !string.IsNullOrEmpty(session.user.name)) return USER_LOADED;
            return REQUEST_LOGIN;
        }
    }
}