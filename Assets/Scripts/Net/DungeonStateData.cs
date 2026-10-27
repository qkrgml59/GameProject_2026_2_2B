using System;
using UnityEngine;

[Serializable]
public class DungeonStateData
{
    public DungeonPlayerData player;
    public DungeonRunData run;
}

[Serializable]
public class DungeonResponse
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

