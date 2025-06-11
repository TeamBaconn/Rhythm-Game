using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource perfectTapAudio; 
    [SerializeField] private AudioSource normalTapAudio;
    
    private void Awake()
    {
        OnScoreUpdateHandler(TileTiming.COUNT, 0, 0);
    }

    private void Start()
    {
        if(GameMode.Instance == null)
        {
            Debug.LogError("GameMode instance is not set. Please ensure GameMode is initialized before ComboUI.");
            return;
        }
        
        GameMode.Instance.OnScoreUpdateDelegate += OnScoreUpdateHandler;
    }

    private void OnDestroy()
    {
        if (GameMode.Instance != null)
        {
            GameMode.Instance.OnScoreUpdateDelegate -= OnScoreUpdateHandler;
        }
    }
    
    void OnScoreUpdateHandler(TileTiming timing, int score, int combo)
    {  
        if (timing == TileTiming.PERFECT)
        {
            perfectTapAudio?.Play();
        }
        else if (timing == TileTiming.NORMAL)
        {
            normalTapAudio?.Play();
        } 
    }
}
