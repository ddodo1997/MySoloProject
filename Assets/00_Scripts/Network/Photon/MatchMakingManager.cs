using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

public class MatchMakingManager : MonoBehaviourPunCallbacks
{
    public static MatchMakingManager Instance { get; private set; }

    private List<Photon.Realtime.RoomInfo> roomList = new();

    private readonly string gameVersion = "1.0";

    public Action<bool> CompletedAction;
    public Action<List<RoomInfo>> RefreshAction;

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
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        CompletedAction?.Invoke(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        CompletedAction?.Invoke(false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PopupManager.Instance.ShowPopup("빈 방이 없습니다.");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(Scenes.InGameScene.ToString());
    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> newRoomList)
    {
        roomList.Clear();
        foreach (var room in newRoomList)
        {
            if (!room.RemovedFromList)
                roomList.Add(room);
        }

        RefreshAction?.Invoke(roomList);
    }
    #region 방 만들기
    public void CreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            var option = new RoomOptions();
            option.IsVisible = true;
            option.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(roomName, option);
        }
    }
    #endregion
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
