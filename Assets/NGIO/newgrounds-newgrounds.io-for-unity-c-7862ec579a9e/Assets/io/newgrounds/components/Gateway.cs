/* start Gateway.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.Gateway {


	/// <summary>
	/// Used to contain the results of calls to the 'Gateway.getDatetime' component.
	/// </summary>
	public class getDatetime : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Gateway.getDatetime";

		private void castCallback(ResultModel result, Action<results.Gateway.getDatetime> handler) {
			results.Gateway.getDatetime r = (results.Gateway.getDatetime)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Gateway.getDatetime> callback) {
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
		public void callWith(core core, Action<results.Gateway.getDatetime> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Gateway.getDatetime> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Gateway.getVersion' component.
	/// </summary>
	public class getVersion : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Gateway.getVersion";

		private void castCallback(ResultModel result, Action<results.Gateway.getVersion> handler) {
			results.Gateway.getVersion r = (results.Gateway.getVersion)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Gateway.getVersion> callback) {
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
		public void callWith(core core, Action<results.Gateway.getVersion> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Gateway.getVersion> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Gateway.ping' component.
	/// </summary>
	public class ping : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Gateway.ping";

		private void castCallback(ResultModel result, Action<results.Gateway.ping> handler) {
			results.Gateway.ping r = (results.Gateway.ping)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.Gateway.ping> callback) {
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
		public void callWith(core core, Action<results.Gateway.ping> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.Gateway.ping> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	
}

/* end Gateway.cs */

