using System;
using UnityEngine;

public class GameOverState : MonoBehaviour
{
    [SerializeField] private Container container;
    [SerializeField] private ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(container._gameOver) return;
        if (other.CompareTag("Bubble"))
        {
            container.GameOver();
        }
    }
    
}
