using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingUIManager : MonoBehaviour
{
    public Button joinButton;

    private void Start()
    {
        joinButton.interactable = false;
        MatchMakingManager.Instance.CompletedAction += ButtonInteractable;
        joinButton.onClick.AddListener(MatchMakingManager.Instance.Connect);
    }

    private void ButtonInteractable(bool value)
    {
        joinButton.interactable = value;
    }

    private void OnDestroy()
    {
        MatchMakingManager.Instance.CompletedAction -= ButtonInteractable;
    }
}
