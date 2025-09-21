using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTable : DataTable
{
    public Dictionary<int, CharacterData> Data = new();

    public CharacterData GetCharacterData(int id)
    {
        return Data[id];
    }

    public override void Load()
    {
        var result = LoadCsv<CharacterData>("characterTable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.characterID)) continue;
            Data.Add(row.characterID, row);
        }
    }
}
