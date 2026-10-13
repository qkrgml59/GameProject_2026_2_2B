using System;
using UnityEngine;

[Serializable]
public class DungeonStateData
{
    public DungeonPlayerData player;
    public DungeonRoomData room;
}

[Serializable]
public class DungonResponse
{
    public bool success;
    public string code;
    public string message;

    public DungeonStateData data;

}

[Serializable]
public class DungeonActionRequest
{
    public string action;
}

