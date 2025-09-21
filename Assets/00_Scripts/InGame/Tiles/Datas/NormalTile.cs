using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalTileSO", menuName = "ScriptableObject/NormalTileSO", order = 0)]
public class NormalTile : TileData
{
    public NormalTile()
    {
        tileModelAddress = "NormalTileModel";
        isMovable = true;
    }
}
