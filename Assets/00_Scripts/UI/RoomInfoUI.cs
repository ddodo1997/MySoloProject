using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfoUI : MonoBehaviour
{
    private Button roomButton;
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI userCnt;
    [SerializeField] private Image blockingImage;
    public bool isConnectable;
    private void Start()
    {
        roomButton = GetComponent<Button>();
        roomButton.onClick.AddListener(() => PhotonManager.Instance.JoinRoom(roomName.text));
    }
    public void Refresh(string roomName, int userCnt)
    {
        this.roomName.text = roomName;
        this.userCnt.text = $"{userCnt} / 2";
        isConnectable = userCnt < 2;
        Blocking(!isConnectable);
    }
    private void Blocking(bool isConnectable)
    {
        blockingImage.gameObject.SetActive(isConnectable);
    }
    private void OnDestroy()
    {
        roomButton.onClick.RemoveAllListeners();
    }
}
