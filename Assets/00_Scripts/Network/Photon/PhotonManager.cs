using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get; private set; }

    private readonly string gameVersion = "1.0";
    private GameUser gameUser = new();
    public GameUser CurrentUser
    {
        get { return gameUser; }
    }

    public Action<bool> CompletedAction;
    public Action<List<RoomInfo>, List<RoomInfo>> RefreshAction;
    public Action<bool> InRoomPropChangeAction;
    public Action<bool> OnMasterClientSwitchedAction;
    public Action<bool> OnEnterRoomIsLocalAction;
    public Action<bool> OnEnterRoomIsMasterAction;
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
    public override void OnJoinedRoom()
    {
        SceneLoader.Load(Scenes.InRoomScene);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    #region 방 내부
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        OnMasterClientSwitchedAction?.Invoke(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("isReady", out var isReadyObj))
                {
                    if (!(bool)isReadyObj)
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

            InRoomPropChangeAction?.Invoke(allReady);
        }
        else
        {
            if (changedProps.TryGetValue("isReady", out var isReadyObj))
            {
                bool isReady = (bool)isReadyObj;
                InRoomPropChangeAction?.Invoke(isReady);
            }
        }
    }

    public void Ready()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            gameUser.isReady = !gameUser.isReady;
            Hashtable props = new Hashtable();
            props["isReady"] = gameUser.isReady;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PopupManager.Instance.ShowPopup("혼자서는 시작 할 수 없어용~~");
                return;
            }
            if (!IsAllReady())
                return;

            gameUser.isReady = false;
            photonView.RPC("RPC_LoadGameScene", RpcTarget.AllBuffered, Scenes.InGameScene);
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
            PopupManager.Instance.ShowPopup("모든 플레이어가 준비되지 않았습니다.");
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
    #region RPC
    [PunRPC]
    private void RPC_LoadGameScene(Scenes scene)
    {
        SceneLoader.Load(scene);
    }
    #endregion
}
