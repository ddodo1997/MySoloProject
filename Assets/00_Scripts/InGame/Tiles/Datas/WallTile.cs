using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WallTileSO", menuName = "ScriptableObject/WallTileSO", order = 0)]
public class WallTile : TileData
{
    public WallTile()
    {
        tileModelAddress = "WallTileModel";
        isMovable = false;
    }
}