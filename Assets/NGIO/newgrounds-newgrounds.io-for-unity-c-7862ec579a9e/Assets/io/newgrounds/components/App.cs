/* start App.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.App {


	/// <summary>
	/// Used to contain the results of calls to the 'App.checkSession' component.
	/// </summary>
	public class checkSession : CallModel	{

		/// <summary>
		/// This component requires an active session.
		/// </summary>
		public override bool require_session { get { return true; } }
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.checkSession";

		private void castCallback(ResultModel result, Action<results.App.checkSession> handler) {
			results.App.checkSession r = (results.App.checkSession)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.checkSession> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.checkSession> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.checkSession> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'App.endSession' component.
	/// </summary>
	public class endSession : CallModel	{

		/// <summary>
		/// This component requires an active session.
		/// </summary>
		public override bool require_session { get { return true; } }
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.endSession";

		private void castCallback(ResultModel result, Action<results.App.endSession> handler) {
			results.App.endSession r = (results.App.endSession)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.endSession> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.endSession> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.endSession> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'App.getCurrentVersion' component.
	/// </summary>
	public class getCurrentVersion : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.getCurrentVersion";

		private void castCallback(ResultModel result, Action<results.App.getCurrentVersion> handler) {
			results.App.getCurrentVersion r = (results.App.getCurrentVersion)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.getCurrentVersion> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.getCurrentVersion> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.getCurrentVersion> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter version 
		/// The version number (in "X.Y.Z" format) of the client-side app. (default = "0.0.0") 
		/// </summary>
		public string version 
		{
			get
			{
				return getPropertyValue<string>("version");
			}
			set
			{
				_property_values["version"] = value;
			}
		}

	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'App.getHostLicense' component.
	/// </summary>
	public class getHostLicense : EventComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.getHostLicense";

		private void castCallback(ResultModel result, Action<results.App.getHostLicense> handler) {
			results.App.getHostLicense r = (results.App.getHostLicense)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.getHostLicense> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.getHostLicense> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.getHostLicense> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'App.logView' component.
	/// </summary>
	public class logView : EventComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.logView";

		private void castCallback(ResultModel result, Action<results.App.logView> handler) {
			results.App.logView r = (results.App.logView)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.logView> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.logView> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.logView> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'App.startSession' component.
	/// </summary>
	public class startSession : EventComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "App.startSession";

		private void castCallback(ResultModel result, Action<results.App.startSession> handler) {
			results.App.startSession r = (results.App.startSession)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.App.startSession> callback) {
			if (callback == null) return null;

			Action<ResultModel> cbCast = (ResultModel result) => {
				castCallback(result, callback);
			};
			
			return cbCast;
		}
		
		/// <summary>
		/// Calls this component on the server.
		/// </summary>
		/// <param name="core">The core instance to call this component with.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void callWith(core core, Action<results.App.startSession> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.App.startSession> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter force 
		/// If true, will create a new session even if the user already has an existing one.
		/// 
		/// Note: Any previous session ids will no longer be valid if this is used. 
		/// </summary>
		public bool force 
		{
			get
			{
				return getPropertyValue<bool>("force");
			}
			set
			{
				_property_values["force"] = value;
			}
		}

	}
	
}

/* end App.cs */

