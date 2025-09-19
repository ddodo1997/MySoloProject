using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataTableManager 
{
    private static readonly Dictionary<string, DataTable> tables = new();
    static DataTableManager()
    {
        var selectedCharacterTable = new SelectedCharacterTable();
        selectedCharacterTable.Load();
        tables.Add(DataTables.SelectedCharacterTable.ToString(), selectedCharacterTable);
    }
    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.LogError($"Not found table with id: {id}");
            return null;
        }

        return tables[id] as T;
    }
}
