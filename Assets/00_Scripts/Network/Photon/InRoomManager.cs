using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviour
{
    public Button readyButton;
    public Button startButton;
    public Button exitButton;
    public Transform userInfoUIParent;
    public GameObject userInfoPrefab;
    public Dictionary<int, GameObject> userUIDict = new();

    private void Start()
    {
        SwitchButton(PhotonNetwork.IsMasterClient);
        RefreshUserList(PhotonNetwork.CurrentRoom.Players);
        PhotonManager.Instance.OnMasterClientSwitchedAction += SwitchButton;
        PhotonManager.Instance.InRoomPropChangeAction += ReadyButtonUpdate;
        PhotonManager.Instance.OnPlayerEnteredRoomAction += RefreshUserList;
        PhotonManager.Instance.OnPlayerLeftRoomAction += RefreshUserList;
        readyButton.onClick.AddListener(PhotonManager.Instance.Ready);
        startButton.onClick.AddListener(PhotonManager.Instance.GameStart);
        exitButton.onClick.AddListener(PhotonManager.Instance.ExitRoom);
    }

    public void SwitchButton(bool isMaster)
    {
        startButton.gameObject.SetActive(isMaster);
        readyButton.gameObject.SetActive(!isMaster);
    }

    private void ReadyButtonUpdate(bool isReady)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (isReady)
            {
                readyButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                readyButton.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void RefreshUserList(Dictionary<int, Player> players)
    {
        // 기존 UI 중 없어진 플레이어 제거
        var toRemove = new List<int>();
        foreach (var kvp in userUIDict)
        {
            if (!players.ContainsKey(kvp.Key))
            {
                Destroy(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var id in toRemove)
            userUIDict.Remove(id);

        // 새로 들어온 플레이어 UI 생성
        foreach (var kvp in players)
        {
            if (!userUIDict.ContainsKey(kvp.Key))
            {
                var player = kvp.Value;
                var ui = Instantiate(userInfoPrefab, userInfoUIParent);
                ui.GetComponent<UserInfoUI>().Init(player);
                userUIDict[kvp.Key] = ui;
            }
            else
            {
                // 이미 존재하는 UI 갱신
                userUIDict[kvp.Key].GetComponent<UserInfoUI>().Init(kvp.Value);
            }
        }
    }


    private void OnDestroy()
    {
        readyButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        PhotonManager.Instance.OnMasterClientSwitchedAction -= SwitchButton;
        PhotonManager.Instance.InRoomPropChangeAction -= ReadyButtonUpdate;
    }
}
