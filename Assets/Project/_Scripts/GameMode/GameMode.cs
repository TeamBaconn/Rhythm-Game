using System;
using TMPro;
using UnityEngine;

public delegate void OnScoreUpdate(TileTiming timing, int score, int combo);

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }
    
    public event OnScoreUpdate OnScoreUpdateDelegate;
    
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameResultUI gameResultUI;
    
    [Header("Score setting")]
    [SerializeField] private int perfectScore = 20;
    [SerializeField] private int normalScore = 10;
    [SerializeField] private float comboMagnifier = 0.1f;
    
    private int _score;
    private int _combo;
    
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }

        _score = 0;
        ResetCombo();
    }

    private void Start()
    {
        if(Conductor.Instance == null)
        {
            Debug.LogError("Conductor instance is not set. Please ensure Conductor is initialized before GameMode.");
            return;
        }
        
        Conductor.Instance.OnTrackStartDelegate += OnTrackStartHandler;
        Conductor.Instance.OnTrackStopDelegate += OnTrackStopHandler;
        
        Conductor.Instance.PlayTrack();
    }

    private void OnDestroy()
    {
        if (Conductor.Instance == null)
        {
            return;
        }
        
        Conductor.Instance.OnTrackStartDelegate -= OnTrackStartHandler;
        Conductor.Instance.OnTrackStopDelegate -= OnTrackStopHandler;
    }

    protected virtual void ResetCombo()
    {
        _combo = -1;
    }
    
    void OnTrackStartHandler(TrackInstance trackInstance)
    { 
        trackInstance.OnTileSpawnDelegate += OnTileSpawned;
        trackInstance.OnTrackFinishDelegate += OnTrackFinishHandler;
    }
    
    void OnTrackStopHandler(TrackInstance trackInstance)
    { 
         trackInstance.OnTileSpawnDelegate -= OnTileSpawned;
         trackInstance.OnTrackFinishDelegate -= OnTrackFinishHandler;
    }

    private void OnTrackFinishHandler(TrackInstance trackInstance)
    { 
        EndGame(true);
    }
    
    private void OnTileSpawned(Tile tile)
    {
        tile.OnTileTimingResponseDelegate += OnTileTimingResponseHandler;
    }
    
    private void OnTileTimingResponseHandler(Tile tile, TileTiming timingResponse)
    {
        tile.OnTileTimingResponseDelegate -= OnTileTimingResponseHandler;
        
        if (timingResponse == TileTiming.PERFECT || timingResponse == TileTiming.NORMAL)
        { 
            if (timingResponse == TileTiming.NORMAL)
            {
                ResetCombo();
            }
            else
            {
                _combo++;
            }
            
            float scoreToAdd = timingResponse == TileTiming.PERFECT ? perfectScore : normalScore;
            scoreToAdd *= 1 + Mathf.Max(0, _combo) * comboMagnifier;
            
            _score += (int)scoreToAdd;
        }
        
        if (timingResponse == TileTiming.MISS || timingResponse == TileTiming.EARLY)
        {
            EndGame(false);
        }
        
        OnScoreUpdateDelegate?.Invoke(timingResponse, _score, _combo);
    }

    protected virtual void EndGame(bool isWin)
    {
        Conductor.Instance.StopTrack();
        
        gameResultUI.SetResult(isWin, _score); 
    }
}
