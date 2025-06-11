using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public struct TimingColorPair
{
    public TileTiming timing;
    public Color color;
}

public class LightUI : MonoBehaviour
{
    [SerializeField] private Image lightImage;

    [Header("Color Settings")]
    [SerializeField] private List<TimingColorPair> colorSettings = new();
    private Dictionary<TileTiming, Color> lightColors = new();

    [SerializeField] private float tweenDuration = 0.3f;
    [SerializeField] private float returnDelay = 0.2f;

    private Color defaultColor => lightColors.ContainsKey(TileTiming.COUNT) ? lightColors[TileTiming.COUNT] : Color.black;

    private void Awake()
    {
        foreach (var pair in colorSettings)
        {
            if (!lightColors.ContainsKey(pair.timing))
                lightColors[pair.timing] = pair.color;
        }

        lightImage.color = defaultColor;
    }

    private void Start()
    {
        if (GameMode.Instance == null)
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
        if (lightImage == null || !lightColors.ContainsKey(timing))
            return;

        lightImage.DOKill();  

        Color targetColor = lightColors[timing];

        if (timing == TileTiming.COUNT)
        { 
            lightImage.color = defaultColor;
            return;
        }
 
        Sequence colorSeq = DOTween.Sequence();
        colorSeq.Append(lightImage.DOColor(targetColor, tweenDuration).SetEase(Ease.OutQuad));
        colorSeq.AppendInterval(returnDelay);
        colorSeq.Append(lightImage.DOColor(defaultColor, tweenDuration).SetEase(Ease.InQuad));
    }
}
