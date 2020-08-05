using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'error' data.
	/// </summary>
	public class error : Model
	{
	
		/// <summary>
		/// A code indication the error type. 
		/// </summary>
		public int code { get; set; }

		/// <summary>
		/// Contains details about the error. 
		/// </summary>
		public string message { get; set; }


	}
}