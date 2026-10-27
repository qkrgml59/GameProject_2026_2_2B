using System;
using UnityEngine;

[Serializable]
public class DungeonRunData
{
    public string id;
    public string state;
    public int currentRoomIndex;
    public int runGold;
    public DungeonRoomData currentRoom;
}

