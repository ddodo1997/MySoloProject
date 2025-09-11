using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingUIManager : MonoBehaviour
{
    public TextMeshProUGUI connectionInfoText;
    public Button joinButton;

    private void Start()
    {
        joinButton.interactable = false;
        connectionInfoText.text = "Connection to Master Server...";
        MatchMakingManager.Instance.TextChangeAction += ChangeString;
        MatchMakingManager.Instance.CompletedAction += ButtonInteractable;
        joinButton.onClick.AddListener(MatchMakingManager.Instance.Connect);
    }

    private void ButtonInteractable(bool value)
    {
        joinButton.interactable = value;
    }
    private void ChangeString(string value = null)
    {
        connectionInfoText.text = value ?? null;
    }

    private void OnDestroy()
    {
        MatchMakingManager.Instance.TextChangeAction -= ChangeString;
        MatchMakingManager.Instance.CompletedAction -= ButtonInteractable;
    }
}
