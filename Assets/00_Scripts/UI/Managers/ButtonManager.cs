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

        var matchManager = MatchMakingManager.Instance;
        matchManager.CompletedAction += ButtonInteractable;
        makeRoomButton.onClick.AddListener(() => roomMaker.gameObject.SetActive(true));
    }

    private void ButtonInteractable(bool value)
    {
        randomMatchButton.interactable = value;
        makeRoomButton.interactable = value;
    }

    private void OnDestroy()
    {
        MatchMakingManager.Instance.CompletedAction -= ButtonInteractable;
        randomMatchButton.onClick.RemoveAllListeners();
        makeRoomButton.onClick.RemoveAllListeners();
    }

}
