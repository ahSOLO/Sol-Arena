using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace io.newgrounds
{
	/// <summary>
	/// A simple model for result data sent from the Newgronds.io server.
	/// </summary>
	public class CallModel : Model
	{
		/// <summary>
		/// An optional echo string that will be returned, verbatim, in any corresponding results object.
		/// </summary>
		public string echo { get; set; }

		/// <summary>
		/// If true, this component requires an active user session to be called.
		/// </summary>
		public virtual bool require_session { get { return false; } }

		/// <summary>
		/// If true, this component needs to have calls encrypted.
		/// </summary>
		public virtual bool secure { get { return false; } }

		/// <summary>
		/// Contains the names of all required properties.
		/// </summary>
		public virtual string[] required_properties { get { return null; } }

		internal IDictionary<string, object> _property_values = new Dictionary<string, object>();

		/// <summary>
		/// If any required properties have not been set, will throw a KeyNotFoundException.
		/// </summary>
		public void validate()
		{
			if (required_properties == null) return;
			for(int i=0; i<required_properties.Length; i++)
			{
				if (!_property_values.ContainsKey(required_properties[i]))
				{
					throw new KeyNotFoundException("Missing required property value '" + required_properties[i] + "' in instance of " + this.GetType().ToString());
				}
			}
		}

		/// <summary>
		/// Ambiguous way to get a property value as an object
		/// </summary>
		/// <param name="property_name"></param>
		/// <returns></returns>
		internal object getPropertyValue(string property_name)
		{
			return _property_values.ContainsKey(property_name) ? _property_values[property_name] : null;
		}

		/// <summary>
		/// Used to get a property value by it's type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="property_name"></param>
		/// <returns></returns>
		internal T getPropertyValue<T>(string property_name) {
			return _property_values.ContainsKey(property_name) ? (T)_property_values[property_name] : default(T);
        }
				
		/// <summary>
		/// Gets the appropriate call model by the associated component name.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public static CallModel getByComponentName(string component)
		{
			CallModel model = null;

			Type modelType = null;
			if (!String.IsNullOrEmpty(component)) modelType = Type.GetType("io.newgrounds.components." + component);

			if (modelType != null) model = (CallModel)Activator.CreateInstance(modelType);

			return model;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public CallModel()
		{
			ignore_properties = new List<string>() { "secure", "required_properties", "require_session" };
		}
		
		internal void setProperties(CallProperties props)
		{
			Type type = this.GetType();
			PropertyInfo[] pi = type.GetProperties();

			foreach (PropertyInfo prop in pi)
			{
				if (props.ContainsKey(prop.Name))
				{
					try {
						prop.SetValue(this, props[prop.Name], null);
					}
					catch (Exception e)
					{
						throw new ArgumentException("Could not convert value of type "+ props[prop.Name].GetType().ToString() +" to "+ prop.PropertyType.ToString()+". (Property name =  "+prop.Name+")\n"+e.Message);
					}
				}
			}
        }
	}

	/// <summary>
	/// An extension of CallModel used by all components requiring a 'host' parameter.
	/// </summary>
	public class EventComponent : CallModel
	{
		/// <summary>
		/// host value will automatically be set for you.
		/// </summary>
		public string host
		{
			get
			{
				return core.getHost();
			}
		}
	}

	/// <summary>
	/// An extension of EventComponent used by loader components that need to open browser windows.
	/// </summary>
	public class LoaderComponent : EventComponent
	{
		/// <summary>
		/// Bypasses the API's native redirect mode.  Unity games will basically send this component to log the event, then open the url internally.
		/// This is necessary due to games built for WebGL not being able to set window targets.
		/// </summary>
		public bool redirect
		{
			get
			{
				return false;
			}
		}

		internal void _doOpenUrlWith(string component, core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null)
		{
			core.callComponent(component, this, (ResultModel _r) =>
				{
					LoaderResult r = (LoaderResult)_r;
					if (callback != null) callback(r);
					if (r.success) core.openUrl(r.url, open_in_new_tab);
				}
			);
		}
	}

	/// <summary>
	/// These models hold data that will be converted to JSON strings for encryption.
	/// </summary>
    public class secureCall : Model
    {
		/// <summary>
		/// The name of the component being called
		/// </summary>
        public string component { get; set; }

		/// <summary>
		/// Any parameters rquired by the component
		/// </summary>
        public object parameters { get; set; }
    }

	/// <summary>
	/// Contains properties for component calls.
	/// </summary>
	public class CallProperties : Dictionary<string, object>
	{
	}
}