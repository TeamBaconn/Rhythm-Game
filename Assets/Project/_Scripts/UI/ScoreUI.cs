
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text timingText;

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
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
            AnimatePop(scoreText.transform, 1.5f, 0.2f);
        }
        else
        {
            Debug.LogError("ScoreUI: scoreText is not set.");
        }

        if (comboText != null)
        {
            if (combo > 0)
            {
                comboText.text = $"x {combo}";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }

            AnimatePop(comboText.transform, 1.1f + combo * 0.05f, 0.2f);
        }
        else
        {
            Debug.LogError("ScoreUI: comboText is not set.");
        }

        if (timingText != null)
        {
            timingText.gameObject.SetActive(timing != TileTiming.COUNT);
            timingText.text = timing.ToString();
            
            float scaleValue = timing == TileTiming.PERFECT ? 1.5f : 1.1f;
            AnimatePop(timingText.transform, scaleValue, 0.2f);
        }
        else
        {
            Debug.LogError("ScoreUI: timingText is not set.");
        }
    }

    void AnimatePop(Transform target, float scaleValue, float duration)
    {
        target.DOKill(); 
        target.localScale = Vector3.one;
        target.DOScale(scaleValue, duration).SetEase(Ease.OutQuad)
            .OnComplete(() => target.DOScale(1f, duration).SetEase(Ease.InQuad));
    }

}
