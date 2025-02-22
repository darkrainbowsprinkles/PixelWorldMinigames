using TMPro;
using UnityEngine;

namespace PixelWorld.Pong
{
    public class PongScoreUI : MonoBehaviour
    {
        [SerializeField] TMP_Text player1ScoreText;
        [SerializeField] TMP_Text player2ScoreText;
        [SerializeField] Goal player1Goal;
        [SerializeField] Goal player2Goal;

        void OnEnable()
        {
            player1Goal.OnScoreUpdate += UpdatePlayer1Score;
            player2Goal.OnScoreUpdate += UpdatePlayer2Score;
        }

        void OnDisable()
        {
            player1Goal.OnScoreUpdate -= UpdatePlayer1Score;
            player2Goal.OnScoreUpdate -= UpdatePlayer2Score;
        }

        void Start()
        {
            UpdatePlayer1Score();
            UpdatePlayer2Score();
        }

        void UpdatePlayer1Score()
        {
            player1ScoreText.text = $"{player1Goal.GetScore()}";
        }

        void UpdatePlayer2Score()
        {
            player2ScoreText.text = $"{player2Goal.GetScore()}";
        }
    }
}