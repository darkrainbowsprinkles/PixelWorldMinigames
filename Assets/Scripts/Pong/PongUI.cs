using TMPro;
using UnityEngine;

namespace PixelWorld.Pong
{
    public class PongUI : MonoBehaviour
    {
        [SerializeField] Goal player1Goal;
        [SerializeField] Goal player2Goal;
        [SerializeField] TMP_Text player1ScoreText;
        [SerializeField] TMP_Text player2ScoreText;
        [SerializeField] TMP_Text timerText;
        [SerializeField] TMP_Text winnerText;
        [SerializeField] GameObject winnerPanel;
        [SerializeField] float gameTime = 180;
        float remainingTime = 0;

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
            remainingTime = gameTime;
            UpdatePlayer1Score();
            UpdatePlayer2Score();
        }

        void Update()
        {
            remainingTime = Mathf.Max(0, remainingTime - Time.deltaTime);

            if(remainingTime == 0)
            {
                Time.timeScale = 0;
                ShowWinner();
            }

            UpdateTimerUI();
        }

        void UpdateTimerUI()
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes}:{seconds:00}";
        }

        void ShowWinner()
        {
            winnerPanel.SetActive(true);

            int player1Score = player1Goal.GetScore();
            int player2Score = player2Goal.GetScore();

            if(player1Score > player2Score)
            {
                winnerText.text = "¡EQUIPO 1 GANA!";
            }
            else if(player1Score < player2Score)
            {
                winnerText.text = "¡EQUIPO 2 GANA!";
            }
            else
            {
                winnerText.text = "¡EMPATE!";
            }
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