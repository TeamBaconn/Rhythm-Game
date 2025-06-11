using System.Collections.Generic; 
using UnityEngine; 

public delegate void OnBeatArrive(float wholeBeat, float subBeat);
public delegate void TrackEvent(TrackInstance trackInstance);
public delegate void OnTileSpawn(Tile tile);

[System.Serializable]
public class TrackInstance
{
    [Header("Track settings")]
    [SerializeField] private AudioTrackProfile trackProfile;
    
    [Header("Tile settings")]
    [SerializeField] private TileMapping tileMapping;
    [SerializeField] private Transform tileSpawnRoot;
    
    public event OnBeatArrive OnBeatArriveDelegate;
    public event OnTileSpawn OnTileSpawnDelegate;
    public event TrackEvent OnTrackFinishDelegate;

    private AudioSource _source;
    private float _currentSubBeat;
    private uint _currentTileIndex;

    /*
     * When the last note spawn, it will define the end beat
     */
    private float _endSubBeat;
    
    private GenericPool<Tile> _tilePool;

    public void SetTrackProfile(AudioTrackProfile trackProfile)
    {
        StopTrack();
        this.trackProfile = trackProfile; 
    }

    public bool PlayTrack(AudioSource source)
    {
        if (!IsValid())
        {
            Debug.LogError("TrackInstance is not valid. Please ensure trackProfile and tileMapping are set.");
            return false;
        }

        StopTrack();

        int MAX_NOTE_POOL_SIZE = (int)(trackProfile.beatPerBar * trackProfile.beatSubdivision * tileMapping.laneCount *
                                 tileMapping.tabCount);
        _tilePool = new GenericPool<Tile>(tileMapping.tilePrefab, MAX_NOTE_POOL_SIZE, null);
        
        _source = source;
        _currentSubBeat = GetCurrentSubBeat();
        _currentTileIndex = 0;
        _endSubBeat = -1;
        
        _source.clip = trackProfile.audioClip;
        _source.Play();

        return true;
    }

    public bool StopTrack()
    {
        if (!IsPlaying())
            return false;

        _source.Stop();
        return true;
    }

    public bool TickBeat()
    {
        if (!IsPlaying())
        {
            return false;
        }

        float oldSubBeat = _currentSubBeat;
        float newSubBeat = GetCurrentSubBeat();
        if (Mathf.FloorToInt(newSubBeat) != Mathf.FloorToInt(_currentSubBeat))
        {
            _currentSubBeat = newSubBeat;
            OnBeatArriveDelegate?.Invoke(GetCurrentBeat(), newSubBeat);
        }
         
        // Spawning tiles from old sub-beat to (new sub-beat + preSpawnSubBeatInterval)
        int spawnStartSubBeat = Mathf.FloorToInt(oldSubBeat);
        int spawnEndSubBeat = Mathf.FloorToInt(newSubBeat) + (int)tileMapping.preSpawnSubBeatInterval;

        SpawnTile(spawnStartSubBeat, spawnEndSubBeat);
        
        // Checking for end beat
        if (_endSubBeat > 0 && newSubBeat >= _endSubBeat)
        {
            OnTrackFinishDelegate?.Invoke(this);
            StopTrack();
        }
        
        return true;
    }

    protected virtual void SpawnTile(float spawnStartSubBeat, float spawnEndSubBeat)
    { 
        for (; _currentTileIndex < tileMapping.tiles.Count; _currentTileIndex++)
        {   
            TileData tileData = tileMapping.tiles[(int)_currentTileIndex];
            if(tileData.startTimeInSubBeat >= spawnStartSubBeat && tileData.startTimeInSubBeat < spawnEndSubBeat)
            {
                Tile tile = _tilePool.Spawn(tileSpawnRoot.position, Quaternion.identity);
                tile.transform.SetParent(tileSpawnRoot);
                tile.OnTileTimingResponseDelegate += OnTileTimingResponse;
                tile.Init(this, tileMapping, tileData);
                
                OnTileSpawnDelegate?.Invoke(tile);
            }
            else
            {
                break;
            }
        }
        
        if (_currentTileIndex >= tileMapping.tiles.Count)
        { 
            // Define the end beat should be 0.5 window after the last tile spawn
            if (_endSubBeat < 0)
            { 
                _endSubBeat = spawnStartSubBeat + (spawnEndSubBeat - spawnStartSubBeat) * 1.5f;
            }
        }
    }

    private void OnTileTimingResponse(Tile tile, TileTiming timingResponse)
    {
        tile.OnTileTimingResponseDelegate -= OnTileTimingResponse;
        if (timingResponse == TileTiming.PERFECT || timingResponse == TileTiming.NORMAL)
        {
            _tilePool.Despawn(tile);
        }
    }

    public bool IsValid()
    {
        return trackProfile != null && tileMapping != null;
    }

    public bool IsPlaying()
    {
        return IsValid() && _source && _source.isPlaying;
    }

    public float GetCurrentSubBeat()
    {
        if (!IsValid())
        {
            return -1;
        }
        
        float offsetSample = trackProfile.offset * trackProfile.audioClip.frequency * (60f / trackProfile.bpm);
        float samplesPerSubBeat = trackProfile.audioClip.frequency * 60f / (trackProfile.bpm * (float)trackProfile.beatSubdivision);

        int offset = (int)(trackProfile.beatSubdivision * trackProfile.offset);
        return (_source.timeSamples + offsetSample) / samplesPerSubBeat - offset;
    }

    public float GetCurrentBeat()
    {
        if (!IsValid())
        {
            return -1;
        }
        
        float offsetSample = trackProfile.offset * trackProfile.audioClip.frequency * (60f / trackProfile.bpm);
        float samplesPerBeat = trackProfile.audioClip.frequency * (60f / trackProfile.bpm);

        return (_source.timeSamples + offsetSample) / samplesPerBeat;
    }
    
    public AudioTrackProfile GetTrackProfile()
    {
        return trackProfile;
    }
    
    public TileMapping GetTileMapping()
    {
        return tileMapping;
    }
}

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    
    [Header("Level Settings")]
    [SerializeField] private AudioSource trackPlayer; 
    [SerializeField] private TrackInstance currentTrackInstance;

    public event OnBeatArrive OnBeatArriveDelegate;
    public event TrackEvent OnTrackStartDelegate;
    public event TrackEvent OnTrackStopDelegate;

    private void Awake()
    {
        Instance = this;
        currentTrackInstance.OnBeatArriveDelegate += HandleBeatArrive;
    }

    public bool PlayTrack()
    {
        if (currentTrackInstance == null || !currentTrackInstance.IsValid())
        {
            Debug.LogError("Conductor: No valid TrackInstance set.");
            return false;
        }
        
        currentTrackInstance.PlayTrack(trackPlayer);
        OnTrackStartDelegate?.Invoke(currentTrackInstance);
        return true;
    }

    public bool StopTrack()
    {
        if (currentTrackInstance == null || !currentTrackInstance.IsValid())
        {
            Debug.LogError("Conductor: No valid TrackInstance set.");
            return false;
        }
        
        currentTrackInstance.StopTrack();
        OnTrackStopDelegate?.Invoke(currentTrackInstance);
        return true;
    }

    private void Update()
    {
        currentTrackInstance.TickBeat();
    }

    private void HandleBeatArrive(float wholeBeat, float subBeat)
    {
        OnBeatArriveDelegate?.Invoke(wholeBeat, subBeat);
    } 

    public float GetCurrentSubBeat()
    {
        return currentTrackInstance.GetCurrentSubBeat();
    }

    public float GetCurrentBeat()
    {
        return currentTrackInstance.GetCurrentBeat();
    }

    public bool IsPlaying()
    {
        return currentTrackInstance.IsPlaying();
    }

    public TrackInstance GetCurrentTrackInstance()
    {
        if (currentTrackInstance.IsValid())
        {
            return currentTrackInstance;
        }

        return null;
    }
}
