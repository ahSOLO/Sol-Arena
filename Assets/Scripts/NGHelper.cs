using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

public class NGHelper : MonoBehaviour
{
    public io.newgrounds.core ngio_core;
    public static NGHelper nGIO;
    [SerializeField] private Text textOutput;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject); // Persist between scenes
        // Destroy all duplicates in existence
        if (nGIO == null) nGIO = this;
        else Destroy(gameObject);
        textOutput.color = Color.white;

        ngio_core.onReady(() =>
        {
            // Call the server to check login status
            ngio_core.checkLogin((bool logged_in) =>
            {
                if (logged_in)
                {
                    onLoggedIn();
                }
                else
                {
                    // Opens up Newgrounds Passport if they are not logged in
                    requestLogin();
                }
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called on player sign-in
    void onLoggedIn()
    {
        // Access the player's info
        io.newgrounds.objects.user player = ngio_core.current_user;
        textOutput.text = "Scoreboard Active!";
    }

    // When user clicks log-in button
    void requestLogin()
    {
        ngio_core.requestLogin(onLoggedIn, onLoginFailed, onLoginCancelled);
    }

    // Called if there was a problem with the login
    void onLoginFailed()
    {
        // access login error
        io.newgrounds.objects.error error = ngio_core.login_error;
        textOutput.text = "Login to access \nthe Scoreboard!";
    }

    // Called if the user cancels a login attempt
    void onLoginCancelled()
    {
        textOutput.text = "Login to access \nthe Scoreboard!";
    }

    public void unlockMedal(int medal_id)
    {
        // create the component
        io.newgrounds.components.Medal.unlock medal_unlock = new io.newgrounds.components.Medal.unlock();

        // set required parameters
        medal_unlock.id = medal_id;

        // call the component on the server, tell it to fire onMedalUnlocked() when done
        medal_unlock.callWith(ngio_core);
        Debug.Log("Sent a message to the server to unlock a medal");
    }

    public void NGSubmitScore(int score_id, int score)
    {
        io.newgrounds.components.ScoreBoard.postScore submit_score = new io.newgrounds.components.ScoreBoard.postScore();
        submit_score.id = score_id;
        submit_score.value = score;
        submit_score.callWith(ngio_core);
        Debug.Log("Sent a message to the server to submit to the Scoreboard");
    }
}
