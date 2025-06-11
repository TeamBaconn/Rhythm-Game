using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BeatListener : MonoBehaviour
{ 
    [SerializeField] private int beatMod = 1;
    [SerializeField] private float scaleMagnitudeOnBeat = 1.25f;
    
    private Vector3 _initialScale;
    private Tweener _currentTween;
     
    private void Awake()
    { 
        _initialScale = transform.localScale;
    }

    private void Start()
    {
        Conductor.Instance.OnBeatArriveDelegate += OnBeatArriveDelegate;
    }

    private void OnDestroy()
    {
        Conductor.Instance.OnBeatArriveDelegate -= OnBeatArriveDelegate;
        
        if (_currentTween != null)
        {
            _currentTween.Kill();
        }
    }
    
    public void SetScale(float scale)
    {
        if (_initialScale.magnitude == 0)
            _initialScale = transform.localScale;
        
        _initialScale *= scale;
         
        if (_currentTween != null)
        {
            _currentTween.Kill();
        } 
        _currentTween = transform.DOScale(_initialScale, 0.1f).SetEase(Ease.OutExpo);
    }

    private void OnBeatArriveDelegate(float wholeBeat, float subBeat)
    {   
        if ((int)subBeat % beatMod != 0) return;

        var trackInstance = Conductor.Instance.GetCurrentTrackInstance();
        if (trackInstance == null)
        {
            Debug.LogWarning("BeatListener: No valid track instance found.");
            return;
        }
        
        float subBeatInterval = 60f / (trackInstance.GetTrackProfile().GetSubBPM() / (float)beatMod);
        float halfSubBeatInterval = subBeatInterval * 0.5f;
        
        _currentTween.Kill();
         
        transform.localScale = _initialScale * scaleMagnitudeOnBeat;
        
        _currentTween = transform.DOScale(_initialScale, halfSubBeatInterval).SetEase(Ease.OutExpo);
    }
}