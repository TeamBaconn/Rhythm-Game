using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScroller : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private RawImage scrollImage;
    [SerializeField] private Vector2 scrollDirection = new Vector2(1, 0);

    private float _scrollSpeed;
    private Rect _initialRect;

    private void Start()
    {
        if (scrollImage == null || Conductor.Instance == null)
        {
            return;
        }

        Conductor.Instance.OnBeatArriveDelegate += OnBeatHandler;
        Conductor.Instance.OnTrackStartDelegate += OnTrackStartHandler;
        Conductor.Instance.OnTrackStopDelegate += OnTrackStopHandler;

        if (Conductor.Instance.IsPlaying())
        {
            OnTrackStartHandler(Conductor.Instance.GetCurrentTrackInstance());
        }
    }

    private void OnDestroy()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeatArriveDelegate -= OnBeatHandler;
            Conductor.Instance.OnTrackStartDelegate -= OnTrackStartHandler;
            Conductor.Instance.OnTrackStopDelegate -= OnTrackStopHandler;
        }
    }

    private void OnTrackStartHandler(TrackInstance trackInstance)
    {
        var profile = trackInstance.GetTrackProfile();
        var tileMapping = trackInstance.GetTileMapping();

        // Setting the lane count and tab count for the UV
        Rect rect = scrollImage.uvRect;

        rect.width = tileMapping.laneCount;
        rect.height = tileMapping.tabCount;
        
        scrollImage.uvRect = rect;
        
        _initialRect = scrollImage.uvRect;
        
        // Add the scroll offset
        float offsetRatio = 1f - MathHelper.SafeDivide(profile.offset, profile.beatPerBar * tileMapping.tabCount);
        AddScrollOffset(-offsetRatio * scrollDirection.x, -offsetRatio * scrollDirection.y);

        // Initialize scroll speed
        float secondsPerBar = ((float) (profile.beatPerBar * tileMapping.tabCount) / profile.bpm) * 60f;
        _scrollSpeed = 1f / secondsPerBar;
    }
    
    private void OnTrackStopHandler(TrackInstance trackInstance)
    { 
        _scrollSpeed = 0f;  
    }

    private void Update()
    {
        if (_scrollSpeed <= 0f || scrollImage == null)
        {
            return;
        }

        float offsetX = _scrollSpeed * Time.deltaTime * scrollDirection.x;
        float offsetY = _scrollSpeed * Time.deltaTime * scrollDirection.y;

        AddScrollOffset(offsetX, offsetY);
    }

    private void OnBeatHandler(float wholeBeat, float subBeat)
    {
        var trackInstance =  Conductor.Instance.GetCurrentTrackInstance();
        if (trackInstance == null)
        {
            Debug.LogError("TileScroller: No valid TrackInstance found.");
            return;
        }
        
        var profile = trackInstance.GetTrackProfile();
        
        float beatPerBar = profile.beatPerBar * trackInstance.GetTileMapping().tabCount;
        float barRatio = (wholeBeat % beatPerBar) / beatPerBar;

        float currentX = Mathf.Repeat(scrollImage.uvRect.x - _initialRect.x, 1f);
        float currentY = Mathf.Repeat(scrollImage.uvRect.y - _initialRect.y, 1f);

        float offsetX = GetShortestOffset(currentX, barRatio) * scrollDirection.x;
        float offsetY = GetShortestOffset(currentY, barRatio) * scrollDirection.y;

        AddScrollOffset(offsetX, offsetY);
    }

    private float GetShortestOffset(float from, float to)
    {
        return Mathf.Repeat(to - from + 1.5f, 1f) - 0.5f;
    }

    private void AddScrollOffset(float offsetX, float offsetY)
    {
        var rect = scrollImage.uvRect;
        rect.x += offsetX;
        rect.y += offsetY;
        scrollImage.uvRect = rect;
        OffsetChildren(offsetX, offsetY);
    }

    private void OffsetChildren(float offsetX, float offsetY)
    {
        float width = scrollImage.rectTransform.rect.width;
        float height = scrollImage.rectTransform.rect.height;
        Vector3 move = new Vector3(offsetX * width, offsetY * height, 0f);

        foreach (Transform child in transform)
        {
            var rt = child as RectTransform;
            if (rt != null)
            {
                rt.localPosition -= move;
            }
        }
    }
}
