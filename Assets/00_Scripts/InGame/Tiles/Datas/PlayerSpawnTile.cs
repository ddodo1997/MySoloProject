using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpawnTileSO", menuName = "ScriptableObject/PlayerSpawnTileSO", order = 0)]
public class PlayerSpawnTile : TileData
{
    public PlayerSpawnTile()
    {
        tileModelAddress = "PlayerSpawnTileModel";
        isMovable = true;
    }
}
