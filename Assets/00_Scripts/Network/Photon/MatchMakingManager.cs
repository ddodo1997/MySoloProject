using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1.0";
    public TextMeshProUGUI connectionInfoText;
    public Button joinButton;

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "Connection to Master Server...";
        joinButton.onClick.AddListener(Connect);
    }

    public override void OnConnectedToMaster()
    {
        connectionInfoText.text = "Online : Connected to Master server";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        joinButton.interactable = true;
        connectionInfoText.text = "Online : Joined Lobby. Ready to Match.";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;
        connectionInfoText.text = $"Offline : Connection Disabled {cause.ToString()}";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            connectionInfoText.text = $"Connecing to Random Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = $"Offline : Connection Disabled - Try Reconnection...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = $"There is no empty room, Creating new Room";
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "Connected with Room.";
        PhotonNetwork.LoadLevel(Scenes.InGameScene.ToString());
    }
}
