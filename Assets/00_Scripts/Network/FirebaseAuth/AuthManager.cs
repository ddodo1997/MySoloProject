using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }
    public TMP_InputField email;
    public TMP_InputField password;
    public Button signInButton;
    public Button signUpButton;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth auth;
    public static FirebaseUser user;

    public void Start()
    {
        signInButton.interactable = false;
        signUpButton.interactable = false;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            var result = t.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.Log(result.ToString());
                IsFirebaseReady = false;
            }
            else
            {
                IsFirebaseReady = true;
                firebaseApp = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
            }
            signUpButton.interactable = IsFirebaseReady;
            signInButton.interactable = IsFirebaseReady;
        });

        signInButton.onClick.AddListener(SignIn);
        signUpButton.onClick.AddListener(SignUp);
    }
    public void SignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || user != null)
            return;

        IsSignInOnProgress = true;
        signInButton.interactable = false;
        signUpButton.interactable = false;

        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(t =>
        {
            Debug.Log($"Sign in status : {t.Status}");

            IsSignInOnProgress = false;
            signInButton.interactable = true;
            signUpButton.interactable = true;

            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception.ToString());
            }
            else if (t.IsCanceled)
            {
                Debug.LogError("Sign-in canceled");
            }
            else
            {
                user = t.Result.User;
                Debug.Log(user.Email);
                SceneLoader.Load(Scenes.MatchMakingScene.ToString());
            }
        });

    }
    public void SignUp()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || user != null)
            return;

        IsSignInOnProgress = true;
        signInButton.interactable = false;
        signUpButton.interactable = false;

        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(t =>
        {
            Debug.Log($"Sign Up Status : {t.Status}");

            IsSignInOnProgress = false;
            signInButton.interactable = true;
            signUpButton.interactable = true;

            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception.ToString());
            }
            else if (t.IsCanceled)
            {
                Debug.LogError("Sign Up canceled");
            }
            else
            {
                user = t.Result.User;
                Debug.Log(user.Email);
            }
        });

    }
}
