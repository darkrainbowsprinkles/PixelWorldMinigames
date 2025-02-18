using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    int score = 0;
    public event Action<int> OnGoalScored;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out Ball ball))
        {
            OnGoalScored.Invoke(score += ball.GetScorePoints());
            ball.Reset();
        }
    }
}
