using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'debug' data.
	/// </summary>
	public class debug : Model
	{
	
		/// <summary>
		/// The time, in milliseconds, that it took to execute a request. 
		/// </summary>
		public string exec_time { get; set; }

		/// <summary>
		/// A copy of the input object that was posted to the server. 
		/// </summary>
		public objects.input input { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public debug() 
		{
			input = new objects.input();
		}

	}
}