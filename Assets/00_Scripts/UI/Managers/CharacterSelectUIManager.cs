using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUIManager : MonoBehaviour
{
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private List<GameObject> characterButtonList = new();

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        var selectedCharacterDatas = DataTableManager.Get<SelectedCharacterTable>(DataTables.SelectedCharacterTable.ToString()).GetSelectedCharacterDatas();
        foreach (var characterData in selectedCharacterDatas)
        {
            var characterButton = Instantiate(characterButtonPrefab, characterButtonParent);
            var characterUI = characterButton.GetComponent<SelectedCharacterInfoUI>();
            characterUI.Init(characterData);
            characterButtonList.Add(characterButton);
        }
    }
}
