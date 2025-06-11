using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileData
{ 
    [Header("Lane")]
    public uint laneIndex; 
    
    [Header("Timing")]
    public uint startTimeInSubBeat;
    public uint durationInSubBeat;
}

[CreateAssetMenu(fileName = "TileMapping", menuName = "Music/Tile Mapping", order = 1)]
public class TileMapping : ScriptableObject
{
    [Header("Spawn Settings")] 
    public Tile tilePrefab;

    /*
     * Tabs visible in screen
     */
    public uint tabCount = 1;
    
    /*
     * Lanes visible in screen
     */
    public uint laneCount = 4;
    public uint preSpawnSubBeatInterval = 4;
    
    [Header("Tile Mapping")]
    public List<TileData> tiles = new List<TileData>();
}