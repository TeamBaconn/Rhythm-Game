using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button retryButton;
    
    private void Awake()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(() => 
            {
                // Logic to restart the game
                GameMode.Instance.RestartGame();
            });
        }
        else
        {
            Debug.LogError("GameResultUI: retryButton is not set.");
        } 
    }
    
    public void SetResult(bool isWin, int score)
    {
        if (resultText == null || scoreText == null)
        {
            Debug.LogError("GameResultUI: resultText or scoreText is not set.");
            return;
        }

        resultText.text = isWin ? "You Win!" : "You Lose!";
        scoreText.text = $"Score: {score}";
        
        gameObject.SetActive(true);
    }
}
