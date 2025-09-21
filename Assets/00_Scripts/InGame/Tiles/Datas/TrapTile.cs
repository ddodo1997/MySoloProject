using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapTileSO", menuName = "ScriptableObject/TrapTileSO", order = 0)]
public class TrapTile : TileData
{
    public int trapDamage;
    public TrapTile()
    {
        tileModelAddress = "TrapTileModel";
        isMovable = true;
        OnPhaseEndAction += TrapOn;
    }

    private void TrapOn(TileBehavior tile)
    {
        if (tile.isUsed && tile.currentCharacter == null)
            return;
    }
}
