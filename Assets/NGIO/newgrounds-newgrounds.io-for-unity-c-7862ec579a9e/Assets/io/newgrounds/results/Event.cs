/* start Event.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.results.Event {

	/// <summary>
	/// Contains results for calls to 'Event'
	/// </summary>
	public class logEvent : ResultModel 
	{

		/// <summary>
		/// Property 'event_name'
		/// </summary>
		public string event_name { get; set; }
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type logEvent 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<logEvent>(component, this);
		}
	}
	
}

/* end Event.cs */

