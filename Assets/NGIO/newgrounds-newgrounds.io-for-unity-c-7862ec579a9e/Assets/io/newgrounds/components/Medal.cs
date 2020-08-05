/* start Medal.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.Medal {


	/// <summary>
	/// Used to contain the results of calls to the 'Medal.getList' component.
	/// </summary>
	public class getList : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Medal.getList";

		private void castCallback(ResultModel result, Action<results.Medal.getList> handler) {
			results.Medal.getList r = (results.Medal.getList)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Medal.getList> callback) {
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
		public void callWith(core core, Action<results.Medal.getList> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Medal.getList> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Medal.unlock' component.
	/// </summary>
	public class unlock : CallModel	{

		/// <summary>
		/// This component requires an active session.
		/// </summary>
		public override bool require_session { get { return true; } }

		/// <summary>
		/// This component requires encryption (handled internally).
		/// </summary>
		public override bool secure { get { return true; } }
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Medal.unlock";

		private void castCallback(ResultModel result, Action<results.Medal.unlock> handler) {
			results.Medal.unlock r = (results.Medal.unlock)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Medal.unlock> callback) {
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
		public void callWith(core core, Action<results.Medal.unlock> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Medal.unlock> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter id 
		/// The numeric ID of the medal to unlock. 
		/// </summary>
		public int id 
		{
			get
			{
				return getPropertyValue<int>("id");
			}
			set
			{
				_property_values["id"] = value;
			}
		}

	}
	
}

/* end Medal.cs */

