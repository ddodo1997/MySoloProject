using Photon.Pun;
using Photon.Realtime;
using System;

public class MatchMakingManager : MonoBehaviourPunCallbacks
{
    public static MatchMakingManager Instance { get; private set; }
    private readonly string gameVersion = "1.0";
    public Action<bool> CompletedAction;
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

    public void Connect()
    {
        CompletedAction?.Invoke(false);

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(Scenes.InGameScene.ToString());
    }
}
