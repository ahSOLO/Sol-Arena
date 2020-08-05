using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'score' data.
	/// </summary>
	public class score : Model
	{
	
		/// <summary>
		/// The score value in the format selected in your scoreboard settings. 
		/// </summary>
		public string formatted_value { get; set; }

		/// <summary>
		/// The tag attached to this score (if any). 
		/// </summary>
		public string tag { get; set; }

		/// <summary>
		/// The user who earned score. If this property is absent, the score belongs to the active user. 
		/// </summary>
		public objects.user user { get; set; }

		/// <summary>
		/// The integer value of the score. 
		/// </summary>
		public int value { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public score() 
		{
			user = new objects.user();
		}

	}
}