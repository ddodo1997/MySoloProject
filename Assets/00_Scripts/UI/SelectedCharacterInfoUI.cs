using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class SelectedCharacterInfoUI : MonoBehaviour
{
    public SelectedCharacterData characterData;
    [SerializeField] private Image characterImage;
    
    public void Init(SelectedCharacterData data)
    {
        characterData = data;
        var handle = Addressables.LoadAssetAsync<Sprite>(characterData.imageAddress);
        handle.WaitForCompletion();
        characterImage.sprite = handle.Result;
        GetComponent<Button>().onClick.AddListener(SelectCharacter);
    }

    private void SelectCharacter()
    {
        PhotonManager.Instance.CurrentUser.characterData = characterData;
        //Å×½ºÆ®
        Debug.Log(characterData.imageAddress);
    }
    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
