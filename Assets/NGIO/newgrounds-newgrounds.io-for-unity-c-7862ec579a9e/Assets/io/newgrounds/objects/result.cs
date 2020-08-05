using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'result' data.
	/// </summary>
	public class result : Model
	{
	
		/// <summary>
		/// The name of the component that was executed (ie 'Medal.unlock'). 
		/// </summary>
		public string component { get; set; }

		private bool _data_is_array = false;
		private Model _flat_data = new Model();
		private SimpleJSONImportableList _array_data = new SimpleJSONImportableList(typeof(Model));

		/// <summary>
		/// An object, or array of one-or-more objects (follows the structure of the corresponding 'call' property), containing any returned properties or errors. 
		/// </summary>
		public object data 
		{
			get
			{
				if (_data_is_array) return _array_data;
				else return _flat_data;
			}
			
			set
			{
				if (value == null) {
					_flat_data = null;
					_data_is_array = false;
				}
				else if (value.GetType() == typeof(SimpleJSONImportableList) || value.GetType().IsSubclassOf(typeof(SimpleJSONImportableList)))
				{
					_array_data = (SimpleJSONImportableList)value;
					_data_is_array = true;
				}
				else if (value.GetType() == _flat_data.GetType() || value.GetType().IsSubclassOf( _flat_data.GetType()))
				{
					_flat_data = (Model)value;
					_data_is_array = false;
				}
				else
				{
					throw new ArgumentException("Can not cast "+value.GetType().ToString()+" to "+ _flat_data.GetType().ToString()+" or "+ _array_data.ToString());
				}
			}
		}
		
		/// <summary>
		/// If you passed an 'echo' value in your call object, it will be echoed here. 
		/// </summary>
		public object echo { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public result() 
		{
			multi_property_map["data"] = new List<object>() {_array_data, _flat_data};
		}

	}
}