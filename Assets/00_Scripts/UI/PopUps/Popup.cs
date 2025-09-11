using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI message;
   [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
    }

    public void ChangeMassage(string massage)
    {
        this.message.text = massage;
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
