using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'input' data.
	/// </summary>
	public class input : Model
	{
	
		/// <summary>
		/// Your application's unique ID. 
		/// </summary>
		public string app_id { get; set; }

		private bool _call_is_array = false;
		private objects.call _flat_call = new objects.call();
		private SimpleJSONImportableList _array_call = new SimpleJSONImportableList(typeof(objects.call));

		/// <summary>
		/// A #call object, or array of one-or-more #call objects. 
		/// </summary>
		public object call 
		{
			get
			{
				if (_call_is_array) return _array_call;
				else return _flat_call;
			}
			
			set
			{
				if (value == null) {
					_flat_call = null;
					_call_is_array = false;
				}
				else if (value.GetType() == typeof(SimpleJSONImportableList) || value.GetType().IsSubclassOf(typeof(SimpleJSONImportableList)))
				{
					_array_call = (SimpleJSONImportableList)value;
					_call_is_array = true;
				}
				else if (value.GetType() == _flat_call.GetType() || value.GetType().IsSubclassOf( _flat_call.GetType()))
				{
					_flat_call = (objects.call)value;
					_call_is_array = false;
				}
				else
				{
					throw new ArgumentException("Can not cast "+value.GetType().ToString()+" to "+ _flat_call.GetType().ToString()+" or "+ _array_call.ToString());
				}
			}
		}
		
		/// <summary>
		/// If set to true, calls will be executed in debug mode. 
		/// </summary>
		public bool debug { get; set; }

		/// <summary>
		/// An optional value that will be returned, verbatim, in the #output object. 
		/// </summary>
		public object echo { get; set; }

		/// <summary>
		/// An optional login session id. 
		/// </summary>
		public string session_id { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public input() 
		{
			multi_property_map["call"] = new List<object>() {_array_call, _flat_call};
		}
		
		internal override IDictionary<string, object> preparePropertiesForSimpleJSON()
		{
			if (debug)
			{
				ignore_properties = null;
            } else
			{
				ignore_properties = new List<string>() { "debug" };
			}
			return base.preparePropertiesForSimpleJSON();
		}


	}
}