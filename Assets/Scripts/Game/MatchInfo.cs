using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MatchInfo
{
    public float StartX;
    public float StartY;
    public short SelfID;
    public byte SelfCharacter;
    public PlayerCharacterDictionary Allies = new PlayerCharacterDictionary();
    public PlayerCharacterDictionary Enemies = new PlayerCharacterDictionary();
}

[Serializable]
public class PlayerCharacterDictionary : SerializableDictionary<string, int> { }
