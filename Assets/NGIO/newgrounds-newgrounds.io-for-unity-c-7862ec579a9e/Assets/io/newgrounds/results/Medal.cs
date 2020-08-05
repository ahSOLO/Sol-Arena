/* start Medal.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.results.Medal {

	/// <summary>
	/// Contains results for calls to 'Medal'
	/// </summary>
	public class getList : ResultModel 
	{

		private objects.medal __medals_object = new objects.medal();
		private SimpleJSONImportableList __medals_list = new SimpleJSONImportableList(typeof(objects.medal));
		
		/// <summary>
		/// An array of medal objects.
		/// </summary>
		public SimpleJSONImportableList medals {
			get { return __medals_list; }
			set { __medals_list = value; }
		}
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type getList 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<getList>(component, this);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public getList() {

			multi_property_map["medals"] = new List<object>() {__medals_list, __medals_object};
		}
	}
	

	/// <summary>
	/// Contains results for calls to 'Medal'
	/// </summary>
	public class unlock : ResultModel 
	{

		private objects.medal __medal = new objects.medal();
		
		/// <summary>
		/// The medal that was unlocked.
		/// </summary>
		public objects.medal medal { 
			get {
				return __medal;
			}
			
			set {
				__medal = value;
			}
		}

		/// <summary>
		/// The user's new medal score.
		/// </summary>
		public int medal_score { get; set; }
		/// <summary>
		/// overrides default dispatchMe behaviour to dispatch as Type unlock 
		/// </summary>
		public override void dispatchMe(string component)
		{
			ngio_core.dispatchEvent<unlock>(component, this);
		}
	}
	
}

/* end Medal.cs */

