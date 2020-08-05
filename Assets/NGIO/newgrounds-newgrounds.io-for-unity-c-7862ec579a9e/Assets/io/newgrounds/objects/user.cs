using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'user' data.
	/// </summary>
	public class user : Model
	{
	
		/// <summary>
		/// The user's icon images. 
		/// </summary>
		public objects.usericons icons { get; set; }

		/// <summary>
		/// The user's numeric ID. 
		/// </summary>
		public int id { get; set; }

		/// <summary>
		/// The user's textual name. 
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// Returns true if the user has a Newgrounds Supporter upgrade. 
		/// </summary>
		public bool supporter { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public user() 
		{
			icons = new objects.usericons();
		}

	}
}