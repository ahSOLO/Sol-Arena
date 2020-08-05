/* start Loader.cs */

using System;
using System.Collections.Generic;

namespace io.newgrounds.components.Loader {


	/// <summary>
	/// Used to contain the results of calls to the 'Loader.loadAuthorUrl' component.
	/// </summary>
	public class loadAuthorUrl : LoaderComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Loader.loadAuthorUrl";

		/// <summary>
		/// Logs a referral event to the server and opens the URL.
		/// </summary>
		/// <param name="core">The core instance used to log the referral.</param>
		/// <param name="open_in_new_tab">This is WebGL specific. If true, the user will be prompted to load the URL in a new browser tab. This is necessary to get around popup blockers.</param>
		/// <param name="callback">An optional callback action to call when the URL has been loaded.</param>
		public void openUrlWith(core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null) {
			_doOpenUrlWith(COMPONENT_NAME, core, open_in_new_tab, callback);
		}
		
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Loader.loadMoreGames' component.
	/// </summary>
	public class loadMoreGames : LoaderComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Loader.loadMoreGames";

		/// <summary>
		/// Logs a referral event to the server and opens the URL.
		/// </summary>
		/// <param name="core">The core instance used to log the referral.</param>
		/// <param name="open_in_new_tab">This is WebGL specific. If true, the user will be prompted to load the URL in a new browser tab. This is necessary to get around popup blockers.</param>
		/// <param name="callback">An optional callback action to call when the URL has been loaded.</param>
		public void openUrlWith(core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null) {
			_doOpenUrlWith(COMPONENT_NAME, core, open_in_new_tab, callback);
		}
		
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Loader.loadNewgrounds' component.
	/// </summary>
	public class loadNewgrounds : LoaderComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Loader.loadNewgrounds";

		/// <summary>
		/// Logs a referral event to the server and opens the URL.
		/// </summary>
		/// <param name="core">The core instance used to log the referral.</param>
		/// <param name="open_in_new_tab">This is WebGL specific. If true, the user will be prompted to load the URL in a new browser tab. This is necessary to get around popup blockers.</param>
		/// <param name="callback">An optional callback action to call when the URL has been loaded.</param>
		public void openUrlWith(core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null) {
			_doOpenUrlWith(COMPONENT_NAME, core, open_in_new_tab, callback);
		}
		
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Loader.loadOfficialUrl' component.
	/// </summary>
	public class loadOfficialUrl : LoaderComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Loader.loadOfficialUrl";

		/// <summary>
		/// Logs a referral event to the server and opens the URL.
		/// </summary>
		/// <param name="core">The core instance used to log the referral.</param>
		/// <param name="open_in_new_tab">This is WebGL specific. If true, the user will be prompted to load the URL in a new browser tab. This is necessary to get around popup blockers.</param>
		/// <param name="callback">An optional callback action to call when the URL has been loaded.</param>
		public void openUrlWith(core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null) {
			_doOpenUrlWith(COMPONENT_NAME, core, open_in_new_tab, callback);
		}
		
	}
	

	/// <summary>
	/// Used to contain the results of calls to the 'Loader.loadReferral' component.
	/// </summary>
	public class loadReferral : LoaderComponent	{
		/// <summary>
		/// The name of the server component this class is modelled after.
		/// </summary>
		public const string COMPONENT_NAME = "Loader.loadReferral";

		/// <summary>
		/// Logs a referral event to the server and opens the URL.
		/// </summary>
		/// <param name="core">The core instance used to log the referral.</param>
		/// <param name="open_in_new_tab">This is WebGL specific. If true, the user will be prompted to load the URL in a new browser tab. This is necessary to get around popup blockers.</param>
		/// <param name="callback">An optional callback action to call when the URL has been loaded.</param>
		public void openUrlWith(core core, bool open_in_new_tab=false, Action<LoaderResult> callback=null) {
			_doOpenUrlWith(COMPONENT_NAME, core, open_in_new_tab, callback);
		}
		
	}
	
}

/* end Loader.cs */

