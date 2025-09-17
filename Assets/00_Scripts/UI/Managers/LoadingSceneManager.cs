using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public Button signInButton;
    public Button signUpButton;

    public void Start()
    {
        NotReady();
        AuthManager.Instance.Initialize(x =>
        {
            signInButton.interactable = x;
            signUpButton.interactable = x;
        });
        signInButton.onClick.AddListener(SignIn);
        signUpButton.onClick.AddListener(SignUp);
    }

    public void SignIn()
    {
        NotReady();
        AuthManager.Instance.SignIn(email.text, password.text, PopupManager.Instance.ShowPopup, OnReady, OnAuthComplete);
    }
    public void SignUp()
    {
        NotReady();
        AuthManager.Instance.SignUp(email.text, password.text, PopupManager.Instance.ShowPopup, OnReady, OnAuthComplete);
    }



    private void OnReady()
    {
        signInButton.interactable = true;
        signUpButton.interactable = true;
    }
    private void NotReady()
    {
        signInButton.interactable = false;
        signUpButton.interactable = false;
    }

    private void OnAuthComplete()
    {
        SceneLoader.Load(Scenes.MatchMakingScene);
    }
}
