using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'scoreboard' data.
	/// </summary>
	public class scoreboard : Model
	{
	
		/// <summary>
		/// The numeric ID of the scoreboard. 
		/// </summary>
		public int id { get; set; }

		/// <summary>
		/// The name of the scoreboard. 
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// Indicates scores are from the current day.
		/// </summary>
		public const string PERIOD_CURRENT_DAY = "D";

		/// <summary>
		/// Indicates scores are from the current week.
		/// </summary>
		public const string PERIOD_CURRENT_WEEK = "W";

		/// <summary>
		/// Indicates scores are from the current month.
		/// </summary>
		public const string PERIOD_CURRENT_MONTH = "M";

		/// <summary>
		/// Indicates scores are from the current year.
		/// </summary>
		public const string PERIOD_CURRENT_YEAR = "Y";

		/// <summary>
		/// Indicates scores are from all-time.
		/// </summary>
		public const string PERIOD_ALL_TIME = "A";

		private void _getScores(string period=PERIOD_CURRENT_DAY, int limit=10, int skip=0, string tag = null, bool social_mode=false, string user_name=null, Action<results.ScoreBoard.getScores> callback=null)
		{
			if (limit > 50)
			{
				limit = 50;
			}
			
			components.ScoreBoard.getScores component = new components.ScoreBoard.getScores();
			component.period = period;
			component.limit = limit;
			if (skip > 0) component.skip = skip;
			if (!string.IsNullOrEmpty(tag)) component.tag = tag;
			if (social_mode) component.social = true;
			if (!string.IsNullOrEmpty(user_name)) component.user = user_name;

			component.callWith(ngio_core, callback);
		}

		/// <summary>
		/// Load global scores by all players.
		/// </summary>
		/// <param name="period">The period to load scores from. Should be a PERIOD_XXXXX constant value.</param>
		/// <param name="limit">The number of scores to load. Max = 50</param>
		/// <param name="skip">The number of scores to skip. For example, limit=10, skip=10 would reurn scores 11-20.</param>
		/// <param name="tag">An optional tag to filter scores by.</param>
		/// <param name="callback">An optional callback handler that will fire when the scores have been loaded.</param>
		public void getGlobalScores(string period = PERIOD_CURRENT_DAY, int limit = 10, int skip = 0, string tag = null, Action<results.ScoreBoard.getScores> callback = null)
		{
			_getScores(period, limit, skip, tag, false, null, callback);
		}

		/// <summary>
		/// Load scores from the player's friends.
		/// </summary>
		/// <param name="period">The period to load scores from. Should be a PERIOD_XXXXX constant value.</param>
		/// <param name="limit">The number of scores to load. Max = 50</param>
		/// <param name="skip">The number of scores to skip. For example, limit=10, skip=10 would reurn scores 11-20.</param>
		/// <param name="tag">An optional tag to filter scores by.</param>
		/// <param name="callback">An optional callback handler that will fire when the scores have been loaded.</param>
		/// <returns>Returns false if the player is not logged in.</returns>
		public bool getSocialScores(string period = PERIOD_CURRENT_DAY, int limit = 10, int skip = 0, string tag = null, Action<results.ScoreBoard.getScores> callback = null)
		{
			if (ngio_core == null || ngio_core.getSessionLoader().getStatus() != SessionResult.USER_LOADED) return false;
			_getScores(period, limit, skip, tag, true, null, callback);
			return true;
		}

		/// <summary>
		/// Loads the player's personal best scores.
		/// </summary>
		/// <param name="period">The period to load scores from. Should be a PERIOD_XXXXX constant value.</param>
		/// <param name="limit">The number of scores to load. Max = 50</param>
		/// <param name="skip">The number of scores to skip. For example, limit=10, skip=10 would reurn scores 11-20.</param>
		/// <param name="tag">An optional tag to filter scores by.</param>
		/// <param name="callback">An optional callback handler that will fire when the scores have been loaded.</param>
		/// <returns>Returns false if the player is not logged in.</returns>
		public bool getPersonalScores(string period = PERIOD_CURRENT_DAY, int limit = 10, int skip = 0, string tag = null, Action<results.ScoreBoard.getScores> callback = null)
		{
			if (ngio_core == null || ngio_core.getSessionLoader().getStatus() != SessionResult.USER_LOADED) return false;

			string username = ngio_core.getSessionLoader().session.user.name;
			_getScores(period, limit, skip, tag, true, username, callback);
			return true;
		}

	}
}