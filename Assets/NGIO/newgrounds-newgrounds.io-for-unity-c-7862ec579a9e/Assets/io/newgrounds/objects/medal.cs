using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'medal' data.
	/// </summary>
	public class medal : Model
	{
	
		/// <summary>
		/// A short description of the medal. 
		/// </summary>
		public string description { get; set; }

		/// <summary>
		/// The difficulty id of the medal. 
		/// </summary>
		public int difficulty { get; set; }

		/// <summary>
		/// The URL for the medal's icon. 
		/// </summary>
		public string icon { get; set; }

		/// <summary>
		/// The numeric ID of the medal. 
		/// </summary>
		public int id { get; set; }

		/// <summary>
		/// The name of the medal. 
		/// </summary>
		public string name { get; set; }

 
		/// <summary>
		/// Property '".$property."'
		/// </summary>
		public bool secret { get; set; }

		/// <summary>
		/// This will only be set if a valid user session exists. 
		/// </summary>
		public bool unlocked { get; set; }

		/// <summary>
		/// The medal's point value. 
		/// </summary>
		public int value { get; set; }

		/// <summary>
		/// Attempts to unlock this medal on the server.
		/// </summary>
		/// <param name="callback">An optional callback handler that will fire when the server responds.</param>
		/// <returns>Returns false if this medal is already unlocked or the player is not logged in.</returns>
		public bool unlock(Action<results.Medal.unlock> callback=null)
		{
			if (this.unlocked || ngio_core == null || ngio_core.getSessionLoader().getStatus() != SessionResult.USER_LOADED) return false;

			components.Medal.unlock component = new components.Medal.unlock();
			component.id = this.id;
			component.callWith(ngio_core, callback);
			return true;
		}

	}
}