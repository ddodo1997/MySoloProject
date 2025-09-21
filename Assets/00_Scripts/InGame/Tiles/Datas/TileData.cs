using System;
using UnityEngine;

public abstract class TileData : ScriptableObject
{
    public string tileModelAddress;
    public bool isMovable;
    public Action<TileBehavior> OnPhaseEndAction;
}
