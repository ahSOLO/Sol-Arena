using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'usericons' data.
	/// </summary>
	public class usericons : Model
	{
	
		/// <summary>
		/// The URL of the user's large icon 
		/// </summary>
		public string large { get; set; }

		/// <summary>
		/// The URL of the user's medium icon 
		/// </summary>
		public string medium { get; set; }

		/// <summary>
		/// The URL of the user's small icon 
		/// </summary>
		public string small { get; set; }


	}
}