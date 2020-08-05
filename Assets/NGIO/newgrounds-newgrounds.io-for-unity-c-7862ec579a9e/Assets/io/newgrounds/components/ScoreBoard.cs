/* start ScoreBoard.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.ScoreBoard {


	/// <summary>
	/// Used to contain the results of calls to the 'ScoreBoard.getBoards' component.
	/// </summary>
	public class getBoards : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "ScoreBoard.getBoards";

		private void castCallback(ResultModel result, Action<results.ScoreBoard.getBoards> handler) {
			results.ScoreBoard.getBoards r = (results.ScoreBoard.getBoards)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.ScoreBoard.getBoards> callback) {
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
		public void callWith(core core, Action<results.ScoreBoard.getBoards> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.ScoreBoard.getBoards> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'ScoreBoard.getScores' component.
	/// </summary>
	public class getScores : CallModel	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "ScoreBoard.getScores";

		private void castCallback(ResultModel result, Action<results.ScoreBoard.getScores> handler) {
			results.ScoreBoard.getScores r = (results.ScoreBoard.getScores)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.ScoreBoard.getScores> callback) {
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
		public void callWith(core core, Action<results.ScoreBoard.getScores> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.ScoreBoard.getScores> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter id 
		/// The numeric ID of the scoreboard. 
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


		/// <summary>
		/// Parameter limit 
		/// An integer indicating the number of scores to include in the list. Default = 10. 
		/// </summary>
		public int limit 
		{
			get
			{
				return getPropertyValue<int>("limit");
			}
			set
			{
				_property_values["limit"] = value;
			}
		}


		/// <summary>
		/// Parameter period 
		/// The time-frame to pull scores from (see notes for acceptable values). 
		/// </summary>
		public string period 
		{
			get
			{
				return getPropertyValue<string>("period");
			}
			set
			{
				_property_values["period"] = value;
			}
		}


		/// <summary>
		/// Parameter skip 
		/// An integer indicating the number of scores to skip before starting the list. Default = 0. 
		/// </summary>
		public int skip 
		{
			get
			{
				return getPropertyValue<int>("skip");
			}
			set
			{
				_property_values["skip"] = value;
			}
		}


		/// <summary>
		/// Parameter social 
		/// If set to true, only social scores will be loaded (scores by the user and their friends). This param will be ignored if there is no valid session id and the 'user' param is absent. 
		/// </summary>
		public bool social 
		{
			get
			{
				return getPropertyValue<bool>("social");
			}
			set
			{
				_property_values["social"] = value;
			}
		}


		/// <summary>
		/// Parameter tag 
		/// A tag to filter results by. 
		/// </summary>
		public string tag 
		{
			get
			{
				return getPropertyValue<string>("tag");
			}
			set
			{
				_property_values["tag"] = value;
			}
		}


		/// <summary>
		/// Parameter user 
		/// A user's ID or name.  If 'social' is true, this user and their friends will be included. Otherwise, only scores for this user will be loaded. If this param is missing and there is a valid session id, that user will be used by default. 
		/// </summary>
		public object user 
		{
			get
			{
				return getPropertyValue("user");
			}
			set
			{
				_property_values["user"] = value;
			}
		}

	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'ScoreBoard.postScore' component.
	/// </summary>
	public class postScore : CallModel	{

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
		public const string COMPONENT_NAME = "ScoreBoard.postScore";

		private void castCallback(ResultModel result, Action<results.ScoreBoard.postScore> handler) {
			results.ScoreBoard.postScore r = (results.ScoreBoard.postScore)result;
			handler(r);
		}
		
		private Action<ResultModel> wrapCallback(Action<results.ScoreBoard.postScore> callback) {
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
		public void callWith(core core, Action<results.ScoreBoard.postScore> callback=null) {
			core.callComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}
		
		/// <summary>
		/// Adds this component to the call Queue of the specified core object.
		/// Use core.executeQueuedComponents() to call this queued component on the server.
		/// </summary>
		/// <param name="core">The core instance to queue this component to.</param>
		/// <param name="callback">An optional callback handler that will be called when the server responds.</param>
		public void queueWith(core core, Action<results.ScoreBoard.postScore> callback=null) {
			core.queueComponent(COMPONENT_NAME, this, wrapCallback(callback));
		}

		/// <summary>
		/// Parameter id 
		/// The numeric ID of the scoreboard. 
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


		/// <summary>
		/// Parameter tag 
		/// An optional tag that can be used to filter scores via ScoreBoard.getScores 
		/// </summary>
		public string tag 
		{
			get
			{
				return getPropertyValue<string>("tag");
			}
			set
			{
				_property_values["tag"] = value;
			}
		}


		/// <summary>
		/// Parameter value 
		/// The int value of the score. 
		/// </summary>
		public int value 
		{
			get
			{
				return getPropertyValue<int>("value");
			}
			set
			{
				_property_values["value"] = value;
			}
		}

	}
	
}

/* end ScoreBoard.cs */

