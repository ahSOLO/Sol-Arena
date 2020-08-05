/* start Event.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.Event {


	/// <summary>
	/// Used to contain the results of calls to the 'Event.logEvent' component.
	/// </summary>
	public class logEvent : EventComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Event.logEvent";

		private void castCallback(ResultModel result, Action<results.Event.logEvent> handler) {
			results.Event.logEvent r = (results.Event.logEvent)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Event.logEvent> callback) {
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
		public void callWith(core core, Action<results.Event.logEvent> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Event.logEvent> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter event_name 
		/// The name of your custom event as defined in your Referrals and Events settings. 
		/// </summary>
		public string event_name 
		{
			get
			{
				return getPropertyValue<string>("event_name");
			}
			set
			{
				_property_values["event_name"] = value;
			}
		}

	}
	
}

/* end Event.cs */

