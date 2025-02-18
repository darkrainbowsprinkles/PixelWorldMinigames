using TMPro;
using UnityEngine;

public class PongScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text player1ScoreText;
    [SerializeField] TMP_Text player2ScoreText;
    [SerializeField] Goal player1Goal;
    [SerializeField] Goal player2Goal;

    void OnEnable()
    {
        player1Goal.OnGoalScored += UpdatePlayer1Score;
        player2Goal.OnGoalScored += UpdatePlayer2Score;
    }

    void OnDisable()
    {
        player1Goal.OnGoalScored -= UpdatePlayer1Score;
        player2Goal.OnGoalScored -= UpdatePlayer2Score;
    }

    void Start()
    {
        UpdatePlayer1Score(0);
        UpdatePlayer1Score(0);
    }

    void UpdatePlayer1Score(int score)
    {
        player1ScoreText.text = $"{score}";
    }

    void UpdatePlayer2Score(int score)
    {
        player2ScoreText.text = $"{score}";
    }
}
