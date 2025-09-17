using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMaker : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private Button createButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        createButton?.onClick.AddListener(CreateRoom);
        cancelButton?.onClick.AddListener(Cancel);
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void CreateRoom()
    {
        if(roomName.text == "")
        {
            PopupManager.Instance.ShowPopup("방 이름을 설정 해 주세요.");
        }
        PhotonManager.Instance.CreateRoom(roomName.text);
    }

    private void OnDestroy()
    {
        createButton?.onClick.RemoveAllListeners();
        cancelButton?.onClick.RemoveAllListeners();
    }
}
