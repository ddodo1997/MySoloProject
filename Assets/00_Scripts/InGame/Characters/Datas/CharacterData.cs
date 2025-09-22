using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public int CharacterID { get; set; }
    public string Name { get; set; }

    public uint Hp { get; set; }
    public uint Attack {  get; set; }
    public uint Defense { get; set; }
    public uint Speed { get; set; }
    public uint ActionPoint { get; set; }

}
