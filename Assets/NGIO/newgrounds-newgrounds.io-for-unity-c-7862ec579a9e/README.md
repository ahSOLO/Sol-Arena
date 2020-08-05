# Newgrounds.io for Unity3D #

Before using this library, make sure you have read the [Introduction to Newgrounds.io](http://www.newgrounds.io)!

Also be sure to check out the [visual guide (pdf)](https://bitbucket.org/newgrounds/newgrounds.io-for-unity-c/raw/f723ea2e456734b58dfe77804071c294956b9d5a/ng-unity-api-visual-guide.pdf) provided by [Geckomla19097](https://geckomla19097.newgrounds.com/)

### Install the library: ###

**Using Mercurial:** Clone the repo to your project directory.

**Download:** Get the repository from https://bitbucket.org/newgrounds/newgrounds.io-for-unity-c/downloads and unzip it to your project directory.


### Implement an instance of io.newgrounds.core into your game: ###

All communication to and from the server is done using JSON-encoded strings, and several calls use AES encryption to help with obfuscation.
The core class handles all of the encoding/decoding and network communication so you don't have to worry about it.

* Create an Empty GameObject (Ctrl+Shift+N)
* Select the new GameObject and rename it to "Newgrounds.io Object" from the Inspector panel.
* While still on the Inspetor panel, click the 'Add Component' button.
* Select Scripts -> io.newgorunds -> Core
* Fill in the App_id and Aes_base64_key properties with the values found in the '*API Tools*' section of your Newgrounds.com project.


### Linking the core to a MonoBehavior: ###

Before you can use the core, you will need a reference to it in your behavior classes.

* In any MonoBehavior class, add the following line where your public properties are declared:
```
#!c#

public io.newgrounds.core ngio_core;
```
* Select whatever object your MonoBehavior has been added to and, from the Inspector, link the ngio_core property to the core instance you added to the *Newgrounds.io Object*.


### Calling Components and Handling Results ###

To make a call to the server, you have to create a component instance.  All components are found in the io.newgrounds.components namespace and are 
named the same as the corresponding server component (ie, io.newgrounds.components.Medal.unlock). Many components have properties you need to fill in before they can be called.

To call a component, simply call it's 'callWith' method.  This method requires a reference to your core instance and can run an optional callback action when it has completed.

When a component is called, the server response is returned as a result object. All result objects are found in the io.newgrounds.results namespace
and are named the same as the corresponding server component (ie, io.newgrounds.results.Medal.unlock)

Here's an example of how you would use a component to unlock a medal in your game.

```
#!c#	
	
	// call this method whenever you want to unlock a medal.
	void unlockMedal(int medal_id) {
		// create the component
		io.newgrounds.components.Medal.unlock medal_unlock = new io.newgrounds.components.Medal.unlock();
		
		// set required parameters
		medal_unlock.id = medal_id;
		
		// call the component on the server, and tell it to fire onMedalUnlocked() when it's done.
		medal_unlock.callWith(ngio_core, onMedalUnlocked);
	}
	
	// this will get called whenever a medal gets unlocked via unlockMedal()
	void onMedalUnlocked(io.newgrounds.results.Medal.unlock result) {
		io.newgrounds.objects.medal medal = result.medal;
		Debug.Log( "Medal Unlocked: " + medal.name + " (" + medal.value + " points)" );
	}
```
	

### Using Newgrounds Passport: ###

Games submitted directly to Newgrounds.com will be able to see if the player is logged in automatically.  If your game is running on another server, or you want to get a Newgrounds user logged in without leaving your game, you can have them log in via Newgrounds Passport.

Users can log in, or create new logins using a standard form or by signing in with **Facebook** or **Google+**.

You can check if the player is logged in at any time using:


```
#!c#

	void onLoginChecked(bool is_logged_in) 
	{
		// do something
	}
	
	void checkLogin() 
	{
		ngio_core.checkLogin(onLoginChecked);
	}
```


If you want to allow players to log in without leaving your game, you can copy the following example:


```
#!c#

	// Gets called when the player is signed in.
	void onLoggedIn() 
	{
		// Do something. You can access the player's info with:
		io.newgrounds.objects.user player = ngio_core.current_user;
	}
	
	// Gets called if there was a problem with the login (expired sessions, server problems, etc).
	void onLoginFailed() 
	{
		// Do something. You can access the login error with:
		io.newgrounds.objects.error error = ngio_core.login_error;
	}
	
	// Gets called if the user cancels a login attempt.
	void onLoginCancelled() 
	{
		// Do something...
	}
	
	// When the user clicks your log-in button
	void requestLogin() 
	{
		// This opens passport and tells the core what to do when a definitive result comes back.
		ngio_core.requestLogin(onLoggedIn, onLoginFailed, onLoginCancelled);
	}
	
	/*
	 * You should have a 'cancel login' button in your game and have it call this, just to be safe.
	 * If the user simply closes the browser tab rather than clicking a button to cancel, we won't be able to detect that.
	 */
	void cancelLogin() 
	{
		/*
		 * This will call onLoginCancelled because you already added it as a callback via ngio_core.requestLogin()
		 * for ngio_core.onLoginCancelled()
		 */ 
		ngio_core.cancelLoginRequest();
	}
	
	// Check if the user has a saved login when your game starts
	void Start() 
	{
		// Do this after the core has been initialized
		ngio_core.onReady(() => { 
			
			// Call the server to check login status
			ngio_core.checkLogin((bool logged_in) => {
				
				if (logged_in) 
				{
					onLoggedIn();
				}
				else 
				{
					/*
					 * This is where you would ask them if the want to sign in.
					 * If they want to sign in, call requestLogin()
					 */
				}
			});
		});
	}
	
	// And finally, have your 'sign out' button call this
	void logOut() 
	{
		ngio_core.logOut();
	}
```

### Notes: ###

Always keep in mind that contacting the server is always an asynchronous task.  Callback actions will not be executed instantly when you call components or use login functions.

In many cases, the Start() function of your behavior may potentially trigger before the core has been fully instantiated.  Instead of writing code like this:

```
#!c#

	void Start() {
		io.newgrounds.components.App.logView logview = new io.newgrounds.components.App.logView();
		logview.callWith(ngio_core);
	}
```
	
You may be better off doing:

```
#!c#

	void Start() {
		// this will call NgioReady when the core has properly initialized.
		ngio_core.onReady(NgioReady);
	}
	
	void NgioReady() {
		io.newgrounds.components.App.logView logview = new io.newgrounds.components.App.logView();
		logview.callWith(ngio_core);
	}
```

Or, if you want to shorten it up:

```
#!c#

	void Start() {
		ngio_core.onReady(() =>
		{
			io.newgrounds.components.App.logView logview = new io.newgrounds.components.App.logView();
			logview.callWith(ngio_core);
		});
	}
```