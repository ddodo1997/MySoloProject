using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealTileSO", menuName = "ScriptableObject/HealTileSO", order = 0)]
public class HealTile : TileData
{
    public int healValue;

    public HealTile()
    {
        tileModelAddress = "HealTileModel";
        isMovable = true;
        OnPhaseEndAction += Heal;
    }

    private void Heal(TileBehavior tile)
    {
        if (tile.isUsed && tile.currentCharacter == null)
            return;
    }
}
