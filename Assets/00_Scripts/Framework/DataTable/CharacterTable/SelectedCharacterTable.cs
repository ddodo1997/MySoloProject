using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCharacterTable : DataTable
{
    public Dictionary<int, SelectedCharacterData> Data = new();

    public List<SelectedCharacterData> GetSelectedCharacterDatas()
    {
        List<SelectedCharacterData> datas = new List<SelectedCharacterData>();
        foreach (SelectedCharacterData data in Data.Values)
        {
            datas.Add(data);
        }
        return datas;
    }

    public override void Load()
    {
        var result = LoadCsv<SelectedCharacterData>("selectedCharacterTable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.characterID)) continue;
            Data.Add(row.characterID, row);
        }
    }
}
