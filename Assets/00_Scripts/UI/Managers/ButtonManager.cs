using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private RoomMaker roomMaker;
    public Button randomMatchButton;
    public Button makeRoomButton;

    private void Start()
    {
        randomMatchButton.interactable = false;
        makeRoomButton.interactable = false;

        var matchManager = PhotonManager.Instance;
        matchManager.CompletedAction += ButtonInteractable;
        makeRoomButton.onClick.AddListener(() => roomMaker.gameObject.SetActive(true));
        randomMatchButton.onClick.AddListener(PhotonManager.Instance.JoinRandomRoom);
    }

    private void ButtonInteractable(bool value)
    {
        randomMatchButton.interactable = value;
        makeRoomButton.interactable = value;
    }

    private void OnDestroy()
    {
        PhotonManager.Instance.CompletedAction -= ButtonInteractable;
        randomMatchButton.onClick.RemoveAllListeners();
        makeRoomButton.onClick.RemoveAllListeners();
    }

}
