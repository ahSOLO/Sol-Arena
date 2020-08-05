/* start ScoreBoard.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.results.ScoreBoard {

	/// <summary>
	/// Contains results for calls to 'ScoreBoard'
	/// </summary>
	public class getBoards : ResultModel 
	{

		private objects.scoreboard __scoreboards_object = new objects.scoreboard();
		private SimpleJSONImportableList __scoreboards_list = new SimpleJSONImportableList(typeof(objects.scoreboard));
		
		/// <summary>
		/// An array of scoreboard objects.
		/// </summary>
		public SimpleJSONImportableList scoreboards {
			get { return __scoreboards_list; }
			set { __scoreboards_list = value; }
		}
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type getBoards 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<getBoards>(component, this);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public getBoards() {

			multi_property_map["scoreboards"] = new List<object>() {__scoreboards_list, __scoreboards_object};
		}
	}
	

	/// <summary>
	/// Contains results for calls to 'ScoreBoard'
	/// </summary>
	public class getScores : ResultModel 
	{

		/// <summary>
		/// The query skip that was used.
		/// </summary>
		public int limit { get; set; }
		/// <summary>
		/// The time-frame the scores belong to. See notes for acceptable values.
		/// </summary>
		public string period { get; set; }
		private objects.scoreboard __scoreboard = new objects.scoreboard();
		
		/// <summary>
		/// The scoreboard being queried.
		/// </summary>
		public objects.scoreboard scoreboard { 
			get {
				return __scoreboard;
			}
			
			set {
				__scoreboard = value;
			}
		}

		private objects.score __scores_object = new objects.score();
		private SimpleJSONImportableList __scores_list = new SimpleJSONImportableList(typeof(objects.score));
		
		/// <summary>
		/// An array of score objects.
		/// </summary>
		public SimpleJSONImportableList scores {
			get { return __scores_list; }
			set { __scores_list = value; }
		}
		/// <summary>
		/// Will return true if scores were loaded in social context ('social' set to true and a session or 'user' were provided).
		/// </summary>
		public bool social { get; set; }
		private objects.user __user = new objects.user();
		
		/// <summary>
		/// The user the score list is associated with (either as defined in the 'user' param, or extracted from the current session when 'social' is set to true)
		/// </summary>
		public objects.user user { 
			get {
				return __user;
			}
			
			set {
				__user = value;
			}
		}

		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type getScores 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<getScores>(component, this);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public getScores() {

			multi_property_map["scores"] = new List<object>() {__scores_list, __scores_object};
		}
	}
	

	/// <summary>
	/// Contains results for calls to 'ScoreBoard'
	/// </summary>
	public class postScore : ResultModel 
	{

		private objects.score __score = new objects.score();
		
		/// <summary>
		/// The score that was posted to the board.
		/// </summary>
		public objects.score score { 
			get {
				return __score;
			}
			
			set {
				__score = value;
			}
		}

		private objects.scoreboard __scoreboard = new objects.scoreboard();
		
		/// <summary>
		/// The scoreboard that was posted to.
		/// </summary>
		public objects.scoreboard scoreboard { 
			get {
				return __scoreboard;
			}
			
			set {
				__scoreboard = value;
			}
		}

		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type postScore 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<postScore>(component, this);
		}
	}
	
}

/* end ScoreBoard.cs */

