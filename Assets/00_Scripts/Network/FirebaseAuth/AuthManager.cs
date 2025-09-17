using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;

public sealed class AuthManager
{
    private static readonly Lazy<AuthManager> lazy = new Lazy<AuthManager>(() => new AuthManager());
    public static AuthManager Instance => lazy.Value;

    public bool IsFirebaseReady { get; private set; }
    public bool IsSignInOnProgress { get; private set; }
    public FirebaseAuth auth;

    private AuthManager() { }

    public void Initialize(Action<bool> onReady)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            if (t.Result == DependencyStatus.Available)
            {
                IsFirebaseReady = true;
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                IsFirebaseReady = false;
                PopupManager.Instance.ShowPopup(t.Exception.Message);
            }
            onReady?.Invoke(IsFirebaseReady);
        });
    }

    public void SignIn(string email, string password, Action<string> ErrorAction, Action OnReady, Action SceneAction)
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
                FirebaseException firebaseEx = t.Exception?.Flatten().InnerExceptions[0] as FirebaseException;
                if (firebaseEx != null)
                {
                    ErrorAction($"�������� �ʴ� ID �̰ų� ��й�ȣ�� Ʋ���ϴ�.");
                }
            }
            else if (t.IsCanceled)
            {
                ErrorAction?.Invoke("Sign-in Canceld");
            }
            else
            {
                AccountManager.Instance.user = t.Result.User;
                SceneAction?.Invoke();
            }
        });
    }

    public void SignUp(string email, string password, Action<string> ErrorAction, Action OnReady, Action SceneAction)
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
                FirebaseException firebaseEx = t.Exception?.Flatten().InnerExceptions[0] as FirebaseException;

                if (firebaseEx != null)
                {
                    var code = (AuthError)firebaseEx.ErrorCode;
                    switch (code)
                    {
                        case AuthError.EmailAlreadyInUse:
                            ErrorAction?.Invoke("�̹� ��� �� �̸��� �Դϴ�.");
                            break;
                        case AuthError.WeakPassword:
                            ErrorAction?.Invoke("��й�ȣ�� �����!!!");
                            break;
                        default:
                            ErrorAction($"ȸ������ ���� \n ErrorCode : {code.ToString()}");
                            break;
                    }
                }
            }
            else if (t.IsCanceled)
            {
                ErrorAction?.Invoke("Sign-up Canceld");
            }
            else
            {
                AccountManager.Instance.user = t.Result.User;
                SceneAction?.Invoke();
            }
        });
    }
}
