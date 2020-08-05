using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'call' data.
	/// </summary>
	public class call : Model
	{
	
		/// <summary>
		/// The name of the component you want to call, ie 'App.connect'. 
		/// </summary>
		public string component { get; set; }

		/// <summary>
		/// An optional value that will be returned, verbatim, in the #result object. 
		/// </summary>
		public object echo { get; set; }

		private bool _parameters_is_array = false;
		private Model _flat_parameters = new Model();
		private SimpleJSONImportableList _array_parameters = new SimpleJSONImportableList(typeof(Model));

		/// <summary>
		/// An object of parameters you want to pass to the component. 
		/// </summary>
		public object parameters 
		{
			get
			{
				if (_parameters_is_array) return _array_parameters;
				else return _flat_parameters;
			}
			
			set
			{
				if (value == null) {
					_flat_parameters = null;
					_parameters_is_array = false;
				}
				else if (value.GetType() == typeof(SimpleJSONImportableList) || value.GetType().IsSubclassOf(typeof(SimpleJSONImportableList)))
				{
					_array_parameters = (SimpleJSONImportableList)value;
					_parameters_is_array = true;
				}
				else if (value.GetType() == _flat_parameters.GetType() || value.GetType().IsSubclassOf( _flat_parameters.GetType()))
				{
					_flat_parameters = (Model)value;
					_parameters_is_array = false;
				}
				else
				{
					throw new ArgumentException("Can not cast "+value.GetType().ToString()+" to "+ _flat_parameters.GetType().ToString()+" or "+ _array_parameters.ToString());
				}
			}
		}
		
		/// <summary>
		/// A an encrypted #call object or array of #call objects. 
		/// </summary>
		public string secure { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public call() 
		{
			multi_property_map["parameters"] = new List<object>() {_array_parameters, _flat_parameters};
		}
		/// <summary>
		/// Reference to current callback handler.
		/// </summary>
		public Action<ResultModel> callback = null;

		internal override IDictionary<string, object> preparePropertiesForSimpleJSON()
		{
			if (!string.IsNullOrEmpty(secure)) return new Dictionary<string, object>() { { "secure", secure } };
			return base.preparePropertiesForSimpleJSON();
		}

	}
}