/* start Gateway.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.results.Gateway {

	/// <summary>
	/// Contains results for calls to 'Gateway'
	/// </summary>
	public class getDatetime : ResultModel 
	{

		/// <summary>
		/// The server's date and time in ISO 8601 format.
		/// </summary>
		public string datetime { get; set; }
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type getDatetime 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<getDatetime>(component, this);
		}
	}
	

	/// <summary>
	/// Contains results for calls to 'Gateway'
	/// </summary>
	public class getVersion : ResultModel 
	{

		/// <summary>
		/// The version number (in X.Y.Z format).
		/// </summary>
		public string version { get; set; }
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type getVersion 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<getVersion>(component, this);
		}
	}
	

	/// <summary>
	/// Contains results for calls to 'Gateway'
	/// </summary>
	public class ping : ResultModel 
	{

		/// <summary>
		/// Will always return a value of 'pong'
		/// </summary>
		public string pong { get; set; }
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type ping 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<ping>(component, this);
		}
	}
	
}

/* end Gateway.cs */

