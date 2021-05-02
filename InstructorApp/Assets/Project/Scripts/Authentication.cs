using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.SceneManagement;

public class Authentication : MonoBehaviour
{

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;

            FBAccessToken fbAccessToken = new FBAccessToken();
            fbAccessToken.TokenString = aToken.TokenString;
            fbAccessToken.UserType = "Instructor";

            StartCoroutine(
                HTTPClient.Post("/updateUser", JsonUtility.ToJson(fbAccessToken), statusCode => {
                    if (statusCode == "200")
                        SceneManager.LoadScene("SetupMeditationRoom");
                    else
                        HFTDialog.MessageBox("Authentication error", "Facebook authentication failed.", () => {
                            Debug.Log("statusCode");
                            Debug.Log(statusCode);
                        });

                })
            );
        }
        else
        {
            Debug.Log("Error");
            Debug.Log(result);
            Debug.Log(result.Error);
            Debug.Log("User cancelled login");
        }
    }

    public void LoginWithFacebook()
    {
        FB.LogOut();
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }
}
