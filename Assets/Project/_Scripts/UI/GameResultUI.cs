using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text scoreText;
    
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
