using System;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnTileTimingResponse(Tile tile, TileTiming timingResponse);

public enum TileTiming
{
    // Missed the node
    MISS,
    // Hit out 25% of the center
    NORMAL,
    // Hit within 25% of the center
    PERFECT,
    // Not in range
    EARLY,
    // For count
    COUNT
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class Tile : MonoBehaviour, IPoolable
{
    public event OnTileTimingResponse OnTileTimingResponseDelegate;
    
    private RectTransform _rectTransform;
    private Button _button;
    
    private bool _initialized = false;

    public void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        
        _button.onClick.AddListener(OnTilePressed);
    }

    public void OnDestroy()
    {
        _button.onClick.RemoveListener(OnTilePressed);
    }

    public bool Init(TrackInstance trackInstance, TileMapping tileMapping, TileData tileData)
    {
        // Set the location based on the tileData laneIndex and tileMapping laneCount
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return false;
        }

        float laneWidth = 1f / tileMapping.laneCount;
        
        float timeOffset = (tileData.startTimeInSubBeat - trackInstance.GetCurrentSubBeat()) / trackInstance.GetTrackProfile().beatSubdivision;
        float screenOffset = timeOffset / ((float)tileMapping.tabCount * trackInstance.GetTrackProfile().beatPerBar);
        
        rectTransform.anchorMin = new Vector2(laneWidth * tileData.laneIndex, screenOffset);
        rectTransform.anchorMax = new Vector2(laneWidth * tileData.laneIndex + laneWidth, screenOffset);

        // NOTE: Currently we only support black note
        const float NOTE_INTERVAL = 1f;
        rectTransform.sizeDelta = NOTE_INTERVAL * new Vector2(0, Screen.height / ((float)tileMapping.tabCount * trackInstance.GetTrackProfile().beatPerBar));
        rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        
        _initialized = true;
        return true;
    }

    public virtual TileTiming GetTileTiming()
    {
        Vector3 pivot = _rectTransform.localPosition;
        
        float HALF_NOTE_HEIGHT = _rectTransform.rect.height * 0.5f;
        float PERFECT_THRESHOLD = HALF_NOTE_HEIGHT * 0.25f;

        if (Mathf.Abs(pivot.y) <= HALF_NOTE_HEIGHT)
        { 
            if (Mathf.Abs(pivot.y) < PERFECT_THRESHOLD)
            {
                return TileTiming.PERFECT;
            } 
            return TileTiming.NORMAL;
        }
        
        if (pivot.y < - HALF_NOTE_HEIGHT)
        {
            return TileTiming.MISS;
        }
        
        return TileTiming.EARLY;
    }

    public void Update()
    {
        if(_initialized == false)
        { 
            return;
        }
        
        if (GetTileTiming() == TileTiming.MISS)
        {
            OnTileTimingResponseDelegate?.Invoke(this, TileTiming.MISS);
        }
    }

    public void OnTilePressed()
    { 
        if(_initialized == false)
        { 
            return;
        }
        
        TileTiming timing = GetTileTiming();  
        OnTileTimingResponseDelegate?.Invoke(this, timing);
    }

    public void OnSpawned()
    {
        _initialized = false;
    }

    public void OnDespawned()
    {
        _initialized = false;
    }
}
