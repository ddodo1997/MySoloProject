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
        signInButton.interactable = false;
        signUpButton.interactable = false;
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
        signInButton.interactable = false;
        signUpButton.interactable = false;
        AuthManager.Instance.SignIn(email.text, password.text, Debug.LogError, OnReady);
    }

    private void OnReady()
    {
        signInButton.interactable = true;
        signUpButton.interactable = true;
    }
    public void SignUp()
    {
        signInButton.interactable = false;
        signUpButton.interactable = false;
        AuthManager.Instance.SignUp(email.text, password.text,Debug.LogError, OnReady);
    }
}
