using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'session' data.
	/// </summary>
	public class session : Model
	{
	
		/// <summary>
		/// If true, the session_id is expired. Use App.startSession to get a new one. 
		/// </summary>
		public bool expired { get; set; }

		/// <summary>
		/// A unique session identifier 
		/// </summary>
		public string id { get; set; }

		/// <summary>
		/// If the session has no associated user but is not expired, this property will provide a URL that can be used to sign the user in. 
		/// </summary>
		public string passport_url { get; set; }

		/// <summary>
		/// If true, the user would like you to remember their session id. 
		/// </summary>
		public bool remember { get; set; }

		/// <summary>
		/// If the user has not signed in, or granted access to your app, this will be null 
		/// </summary>
		public objects.user user { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public session() 
		{
			user = new objects.user();
		}

	}
}