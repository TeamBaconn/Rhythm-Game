using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugger : MonoBehaviour
{ 
    void Update()
    {
        if (GameMode.Instance == null)
        { 
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameMode.Instance.RestartGame();
        }
    }
}
