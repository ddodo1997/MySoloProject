using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region bools
    public bool IsMaster
    {
        get
        {
            return PhotonNetwork.LocalPlayer.IsMasterClient;
        }
    }
    public string NickName => PhotonNetwork.NickName;
    #endregion
    public static PhotonManager Instance { get; private set; }

    private readonly string gameVersion = "1.0";
    private GameUser gameUser = new();
    public GameUser CurrentUser
    {
        get { return gameUser; }
    }

    #region Actions
    public Action<bool> CompletedAction;
    public Action<List<RoomInfo>, List<RoomInfo>> RefreshAction;
    public Action<bool> InRoomPropChangeAction;
    public Action<bool> OnMasterClientSwitchedAction;
    public Action<bool> OnEnterRoomIsLocalAction;
    public Action<bool> OnEnterRoomIsMasterAction;
    public Action<Dictionary<int, Player>> OnPlayerEnteredRoomAction;
    public Action<Dictionary<int, Player>> OnPlayerLeftRoomAction;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = AccountManager.Instance.Email;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region 로비
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
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PopupManager.Instance.ShowPopup("빈 방이 없습니다.");
    }


    public override void OnRoomListUpdate(List<RoomInfo> newRoomList)
    {
        List<RoomInfo> removedRooms = new();
        List<RoomInfo> addedRooms = new();
        foreach (var room in newRoomList)
        {
            if (room.RemovedFromList)
                removedRooms.Add(room);
            else
                addedRooms.Add(room);
        }

        RefreshAction?.Invoke(removedRooms, addedRooms);
    }
    #endregion
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
    #region 방 입장    
    public void SeedRegist()
    {
        int seed = DateTime.Now.GetHashCode();
        Hashtable props = new Hashtable { { "mapSeed", seed } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
    public override void OnJoinedRoom()
    {
        SceneLoader.Load(Scenes.InRoomScene);
        if(IsMaster)
            SeedRegist();
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    #region 방 내부
    public int ReturnSeed()
    {
        return (int)PhotonNetwork.CurrentRoom.CustomProperties["mapSeed"];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerEnteredRoomAction?.Invoke(PhotonNetwork.CurrentRoom.Players);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerLeftRoomAction?.Invoke(PhotonNetwork.CurrentRoom.Players);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnMasterClientSwitchedAction?.Invoke(IsMaster);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        InRoomPropChangeAction?.Invoke(IsAllReady());
    }

    public void Ready()
    {
        if (!IsMaster)
        {
            gameUser.isReady = !gameUser.isReady;
            Hashtable props = new Hashtable();
            props["isReady"] = gameUser.isReady;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
    public void GameStart()
    {
        if (IsMaster)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PopupManager.Instance.ShowPopup("혼자서는 시작 할 수 없어용~~");
                return;
            }
            if (!IsAllReady())
            {
                PopupManager.Instance.ShowPopup("모든 플레이어가 준비되지 않았습니다.");
                return;
            }
            gameUser.isReady = false;
            object content = Scenes.InGameScene;
            RaiseEventOptions options = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };
            PhotonNetwork.RaiseEvent((byte)GameEvents.LoadGameScene, content, options, SendOptions.SendReliable);
        }
    }

    private bool IsAllReady()
    {
        bool allReady = true;

        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            if (player.CustomProperties.TryGetValue("isReady", out var isReadyObj))
            {
                bool isReady = (bool)isReadyObj;
                if (!isReady)
                {
                    allReady = false;
                    break;
                }
            }
            else
            {
                allReady = false;
                break;
            }
        }

        if (!allReady)
        {
            return false;
        }
        return true;
    }
    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Load(Scenes.MatchMakingScene);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }
    #endregion
    #region 인게임


    #endregion
}
