using UnityEngine;

public enum Scenes
{
    LoadingScene = -1,
    LobbyScene,
    ChooseCharactorScene,
    MatchMakingScene,
    InRoomScene,
    InGameScene,
}

public enum DataTables
{
    SelectedCharacterTable,
}

public enum GameEvents : byte
{
    LoadGameScene = 1,
}
public enum TileType
{
    Wall = 0,
    Normal,
    Heal,
    Trap,
    Spawn
}

public static class Defines
{
    public static readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
}