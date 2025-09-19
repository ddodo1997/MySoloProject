using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempStart : MonoBehaviour
{
    void Start()
    {
        PopupManager.Instance.ShowPopup(PhotonManager.Instance.CurrentUser.characterData.imageAddress);  
    }
}
