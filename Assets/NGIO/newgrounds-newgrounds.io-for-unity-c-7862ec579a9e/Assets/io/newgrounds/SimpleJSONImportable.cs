using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleJSON;
using UnityEngine;

namespace io.newgrounds
{
	

	/// <summary>
	/// Used to apply the keys of a SimpleJson.JSONNode to a model object. 
	/// This class should be extended by all model classes and never used directly.
	/// </summary>
	public abstract class SimpleJSONImportable
	{
		internal IList<string> ignore_properties = null;
		internal IDictionary<string, List<object>> multi_property_map = new Dictionary<string, List<object>>();

		public const int MultiValueArray = 0;
		public const int MultiValueFlat = 1;

		public core ngio_core = null;

		/// <summary>
		///  Extracts keys from a SimpleJSON.JSONNode matching public properties in this model object.
		/// </summary>
		/// <param name="json">A SimpleJSON.JObject that has been processed from a JSON string.</param>
		/// <param name="core">The core instance to attach to this object.</param>
		internal void setPropertiesFromSimpleJSON(JObject json, core core=null)
		{
            if (core != null) this.ngio_core = core;
			else core = this.ngio_core;

			Type rType = this.GetType();
			PropertyInfo[] rProps = rType.GetProperties();

            Dictionary<string, JObject> jdict = json.ObjectValue;
            
            SimpleJSONImportable pval;
			Type ptype;

           foreach (PropertyInfo property in rProps)
			{
                if (!jdict.ContainsKey(property.Name)) continue;

				bool is_model = property.PropertyType.IsSubclassOf(typeof(SimpleJSONImportable));
				JObject jprop = jdict[property.Name];
                
                // subclasses can override this method to handle special case properties
				if (setSpecialPropertyFromJSON(property.Name, jprop, core)) continue;

                if (jprop.Kind == JObjectKind.Null)
                {
                    property.SetValue(this, null, null);
                }
                else if (is_model)
				{
                    pval = (SimpleJSONImportable)property.GetValue(this, null);
                    pval.setPropertiesFromSimpleJSON(jprop, core);
				}
				else if (isMultiTypeProperty(property.Name) && multi_property_map[property.Name][MultiValueFlat].GetType().IsSubclassOf(typeof(SimpleJSONImportable)))
				{
                    switch (jprop.Kind)
					{
						case JObjectKind.Array:
							SimpleJSONImportableList plist = new SimpleJSONImportableList();
							ptype = multi_property_map[property.Name][MultiValueFlat].GetType();
							plist.type = ptype;
							foreach(JObject jrow in jprop.ArrayValue)
							{
								pval = (SimpleJSONImportable)Activator.CreateInstance(ptype);
								pval.setPropertiesFromSimpleJSON(jrow, core);
								plist.Add(pval);
							}
							property.SetValue(this, plist, null);
                            break;

						case JObjectKind.Object:
							pval = (SimpleJSONImportable)property.GetValue(this, null);
							pval.setPropertiesFromSimpleJSON(jprop, core);
							break;

						default:
							Debug.LogWarning("Unexpected property value for "+property.Name+". Setting to null.");
							property.SetValue(this, null, null);
							break;
					}
                }
				else
				{
                    JObjectKind jk = jprop.Kind;
                    string ppt = property.PropertyType.ToString();

                    // some bools and numbers may get passed in as strings, this should catch them.
                    if (jk == JObjectKind.String && ppt != "System.String")
                    {
                        
                        switch (ppt)
                        {
                            case "System.Boolean":
                                jk = JObjectKind.Boolean;
                                break;
                            default:
                                jk = JObjectKind.Number;
                                break;
                        }
                    }
                    // and some bools may be set as 0/1 integers too
                    else if (jk == JObjectKind.Number && ppt == "System.Boolean")
                    {
                       jk = JObjectKind.Boolean;
                    }

                    switch (jk)
					{
						case JObjectKind.Null:
							property.SetValue(this, null, null);
							break;

						case JObjectKind.Boolean:
							property.SetValue(this, jprop.BooleanValue, null);
							break;

						case JObjectKind.String:
							property.SetValue(this, jprop.StringValue, null);
							break;

						case JObjectKind.Number:
                            
                            // now to figure out what number type to use...
                            if (ppt.StartsWith("System.Int"))
							{
								property.SetValue(this, jprop.IntValue, null);
							} else
							{
								property.SetValue(this, jprop.DoubleValue, null);
							}
							break;
						default:
							Debug.LogWarning("Unexpected property value for " + property.Name + " (Kind="+jk.ToString()+"). Setting to null.");
							property.SetValue(this, null, null);
							break;
					}
				}
			}
		}

		internal virtual bool setSpecialPropertyFromJSON(string property, JObject json, core core)
		{
			return false;
		}

		/// <summary>
		/// Converts this model to a SimpeleJSON compatible dictionary.
		/// </summary>
		/// <returns></returns>
		internal virtual IDictionary<string, object> preparePropertiesForSimpleJSON()
		{
			IDictionary<string, object> obj = new Dictionary<string, object>();
			Type rType = this.GetType();
			PropertyInfo[] rProps = rType.GetProperties();

			foreach (PropertyInfo property in rProps)
			{
				string key = property.Name;
				if (ignore_properties != null && ignore_properties.Contains(key)) continue;
                var value = property.GetValue(this, null);
				if (value == null) continue;
				
				if (value.GetType() == typeof(SimpleJSONImportableList))
				{
					//if (((IList)value).Count < 1) continue;

					if (((IList)value).Count > 0 && ((IList)value)[0].GetType().IsSubclassOf(typeof(SimpleJSONImportable)))
					{
						List<IDictionary<string, object>> preparedList = new List<IDictionary<string, object>>();

						foreach(SimpleJSONImportable model in (IList)value)
						{
							preparedList.Add(model.preparePropertiesForSimpleJSON());
						}
						obj[key] = preparedList;

					} else
					{
						obj[key] = value;
					}
                }
				else
				{
					if (value.GetType().IsSubclassOf(typeof(SimpleJSONImportable)))
					{
						obj[key] = ((SimpleJSONImportable)value).preparePropertiesForSimpleJSON();
					}
					else
					{
						obj[key] = value;
					}
				}
            }
			return obj;
		}
		
		/// <summary>
		/// Checks if a model property can be multi type (some models can accept flat object types, or arrays of flat objects)
		/// </summary>
		/// <param name="property_name"></param>
		/// <returns></returns>
		public bool isMultiTypeProperty(string property_name)
		{
			return multi_property_map.ContainsKey(property_name);
        }
		
		private T GetTypeInstance<T>(string typename)
		{
			return (T)Activator.CreateInstance(Type.GetType(typename));
		}

		private List<T> GetTypeInstanceArray<T>()
		{
			return new List<T>();
		}
	}

	/// <summary>
	/// This class is used when we need to import arrays of model data from JSON results.
	/// </summary>
	public class SimpleJSONImportableList : List<object>
	{
		public SimpleJSONImportableList(Type t = null)
		{
			if (t != null) type = t;
		}

		private Type _type = null;
		public Type type
		{
			get
			{
				return _type;
			}

			set
			{
				if (_type != null) throw new ArgumentException("SimpleJsonImportableArray can only be set once");
				_type = value;
			}

		}

		public void insert(object value)
		{
			if (_type == null) throw new ArgumentException("SimpleJsonImportableArray must have a type set before insert can be invoked.");
			else if (value.GetType() != _type) throw new ArgumentException("Attempted to insert illegal value type.");

			this.Add(value);
		}
	}
}