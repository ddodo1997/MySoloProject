using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;

public sealed class AuthManager
{
    private static readonly Lazy<AuthManager> lazy = new Lazy<AuthManager>(() => new AuthManager());
    public static AuthManager Instance => lazy.Value;

    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress {  get; private set; }
    public FirebaseAuth auth;

    private AuthManager() { } // 외부 new 차단

    public void Initialize(Action<bool> onReady)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            if (t.Result == DependencyStatus.Available)
            {
                IsFirebaseReady = true;
                auth = FirebaseAuth.DefaultInstance;
            }
            else IsFirebaseReady = false;
            onReady?.Invoke(IsFirebaseReady);
        });
    }

    public void SignIn(string email, string password, Action<string> errorAction, Action OnReady)
    {
        if (!IsFirebaseReady || IsSignInOnProgress || AccountManager.Instance.user != null)
            return;

        IsSignInOnProgress = true;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(t =>
        {
            IsSignInOnProgress = false;
            OnReady?.Invoke();

            if (t.IsFaulted)
            {
                errorAction?.Invoke("Sign-in Failed");
            }
            else if (t.IsCanceled)
            {
                errorAction?.Invoke("Sign-in Canceld");
            }
            else
            {
                AccountManager.Instance.user = t.Result.User;
                SceneLoader.Load(Scenes.MatchMakingScene.ToString());
            }
        });
    }

    public void SignUp(string email, string password, Action<string> errorAction, Action OnReady)
    {
        if (!IsFirebaseReady || IsSignInOnProgress || AccountManager.Instance.user != null)
            return;

        IsSignInOnProgress = true;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(t =>
        {
            IsSignInOnProgress = false;
            OnReady?.Invoke();

            if (t.IsFaulted)
            {
                errorAction?.Invoke("Sign-up Failed");
            }
            else if (t.IsCanceled)
            {
                errorAction?.Invoke("Sign-up Canceld");
            }
            else
            {
                AccountManager.Instance.user = t.Result.User;
                SceneLoader.Load(Scenes.MatchMakingScene.ToString());
            }
        });
    }
}
