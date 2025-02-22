using System;
using UnityEngine;

namespace PixelWorld.Pong
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] Goal oppositeGoal;
        int score = 0;
        public event Action OnScoreUpdate;

        public void UpdateScore(int score)
        {
            this.score += score;
            OnScoreUpdate.Invoke();
        }

        public int GetScore()
        {
            return score;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out Ball ball))
            {
                oppositeGoal.UpdateScore(ball.GetScorePoints());

                if(ball.IsSplitBall())
                {
                    Destroy(ball.gameObject);
                }
                else
                {
                    ball.Reset();
                }
            }
        }
    }
}