using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InRoomManager : MonoBehaviour
{
    public Button readyButton;
    public Button startButton;
    public Button exitButton;

    private void Start()
    {
        SwitchButton(PhotonNetwork.IsMasterClient);
        PhotonManager.Instance.OnMasterClientSwitchedAction += SwitchButton;
        PhotonManager.Instance.InRoomPropChangeAction += ReadyButtonUpdate;
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

    private void OnDestroy()
    {
        readyButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        PhotonManager.Instance.OnMasterClientSwitchedAction -= SwitchButton;
        PhotonManager.Instance.InRoomPropChangeAction -= ReadyButtonUpdate;
    }
}
