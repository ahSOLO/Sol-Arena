using System;
using System.Collections.Generic;
using SimpleJSON;

namespace io.newgrounds.objects {

	/// <summary>
	/// A model for 'output' data.
	/// </summary>
	public class output : Model
	{
	
		/// <summary>
		/// If there was an error, this will contain the current version number of the API gateway. 
		/// </summary>
		public string api_version { get; set; }

		/// <summary>
		/// Your application's unique ID 
		/// </summary>
		public string app_id { get; set; }

		/// <summary>
		/// Contains extra information you may need when debugging (debug mode only). 
		/// </summary>
		public objects.debug debug { get; set; }

		/// <summary>
		/// If you passed an 'echo' value in your input object, it will be echoed here. 
		/// </summary>
		public object echo { get; set; }

		/// <summary>
		/// This will contain any error info if the success property is false. 
		/// </summary>
		public objects.error error { get; set; }

		/// <summary>
		/// If there was an error, this will contain the URL for our help docs. 
		/// </summary>
		public string help_url { get; set; }

		private bool _result_is_array = false;
		private objects.result _flat_result = new objects.result();
		private SimpleJSONImportableList _array_result = new SimpleJSONImportableList(typeof(objects.result));

		/// <summary>
		/// This will be a #result object, or an array containing one-or-more #result objects (this will match the structure of the #call property in your #input object). 
		/// </summary>
		public object result 
		{
			get
			{
				if (_result_is_array) return _array_result;
				else return _flat_result;
			}
			
			set
			{
				if (value == null) {
					_flat_result = null;
					_result_is_array = false;
				}
				else if (value.GetType() == typeof(SimpleJSONImportableList) || value.GetType().IsSubclassOf(typeof(SimpleJSONImportableList)))
				{
					_array_result = (SimpleJSONImportableList)value;
					_result_is_array = true;
				}
				else if (value.GetType() == _flat_result.GetType() || value.GetType().IsSubclassOf( _flat_result.GetType()))
				{
					_flat_result = (objects.result)value;
					_result_is_array = false;
				}
				else
				{
					throw new ArgumentException("Can not cast "+value.GetType().ToString()+" to "+ _flat_result.GetType().ToString()+" or "+ _array_result.ToString());
				}
			}
		}
		
		/// <summary>
		/// If false, there was a problem with your 'input' object. Details will be in the #error property. 
		/// </summary>
		public bool success { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		public output() 
		{
			debug = new objects.debug();
			error = new objects.error();
			multi_property_map["result"] = new List<object>() {_array_result, _flat_result};
		}
		
		internal override bool setSpecialPropertyFromJSON(string property, JObject json, core core)
		{
			result rm;

			if (property == "result")
			{

                if (json.Kind == JObjectKind.Array)
				{
					SimpleJSONImportableList list = new SimpleJSONImportableList(typeof(ResultModel));

					foreach(JObject jrow in json.ArrayValue)
					{
						rm = getResultFromJObject(jrow, core);
						list.Add(rm);
					}

					result = list;
				}
				else
				{
					result = getResultFromJObject(json, core);
				}
				return true;
			}

			return base.setSpecialPropertyFromJSON(property, json, core);
		}

		private objects.result getResultFromJObject(JObject json, core core)
		{
			objects.result _result = new objects.result();
			_result.setPropertiesFromSimpleJSON(json, core);

			ResultModel _data = null;

			if (json.Kind != JObjectKind.Object)
			{
				_data = new ResultModel();
				_data.error.message = "Unexpected result format.";
			}
			else if (!json.ObjectValue.ContainsKey("component"))
			{
				_data = new ResultModel();
				_data.error.message = "Missing required component name.";
			}
			else
			{
				_data = ResultModel.getByComponentName(json.ObjectValue["component"].StringValue);
				if (json.ObjectValue.ContainsKey("data"))
				{
					_data.setPropertiesFromSimpleJSON(json.ObjectValue["data"], core);
				}
			}
			
			_result.data = _data;
			
			return _result;
		}

	}
}