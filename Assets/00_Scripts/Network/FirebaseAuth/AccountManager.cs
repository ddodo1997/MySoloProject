using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AccountManager
{
    private static readonly Lazy<AccountManager> lazy = new Lazy<AccountManager>(() => new AccountManager());
    public static AccountManager Instance => lazy.Value;

    public FirebaseUser user;

}
