using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SimpleJSON;

namespace io.newgrounds
{
	/// <summary>
	/// Models are objects with properties that can be converted to JSON strings for transmission to the API.
	/// </summary>
	public class Model : SimpleJSONImportable
	{
		
		/// <summary>
		/// Converts the model's properties to a JSON-encoded string.
		/// </summary>
		/// <returns></returns>
		public virtual string toJSON()
		{
			return JSONEncoder.Encode(this.preparePropertiesForSimpleJSON());
        }
		
    }
}