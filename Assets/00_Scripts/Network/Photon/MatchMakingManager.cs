using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingManager : MonoBehaviourPunCallbacks
{
    public static MatchMakingManager Instance { get; private set; }
    private readonly string gameVersion = "1.0";
    public Action<bool> CompletedAction;
    public Action<string> TextChangeAction;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        TextChangeAction?.Invoke("Online : Connected to Master server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CompletedAction?.Invoke(true);
        TextChangeAction?.Invoke("Online : Joined Lobby. Ready to Match.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        CompletedAction?.Invoke(false);
        TextChangeAction?.Invoke($"Offline: Connection Disabled { cause.ToString()}");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        CompletedAction?.Invoke(false);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            TextChangeAction?.Invoke($"Connecting to Random Room...");
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            TextChangeAction?.Invoke($"Offline : Connection Disabled - Try Reconnection...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        TextChangeAction?.Invoke($"There is no empty room Please Create New Room");
    }

    public override void OnJoinedRoom()
    {
        TextChangeAction?.Invoke("Connected with Room.");
        PhotonNetwork.LoadLevel(Scenes.InGameScene.ToString());
    }
}
