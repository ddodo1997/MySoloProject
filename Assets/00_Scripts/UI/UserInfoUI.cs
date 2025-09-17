using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private Image roomMasterMark;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private Image readyMark;
    public Player currnetPlayer;
    private void Start()
    { 
        PhotonManager.Instance.InRoomPropChangeAction += OnReady;
    }
    public void Init(Player player)
    {
        currnetPlayer = player;
        userName.text = currnetPlayer.NickName;
        roomMasterMark.gameObject.SetActive(currnetPlayer.IsMasterClient);
    }

    public void OnUserChanged(bool isMaster)
    { 
        roomMasterMark.gameObject.SetActive(currnetPlayer.IsMasterClient);
    }
    public void OnReady(bool isReady)
    {
        if (currnetPlayer.CustomProperties.TryGetValue("isReady", out var isReadyObj))
            readyMark.gameObject.SetActive((bool)isReadyObj);
    }

    private void OnDestroy()
    {
        PhotonManager.Instance.InRoomPropChangeAction -= OnReady;
    }
}
