using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    public int score;
    public bool isGameActive;

    public Button retryButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI pauseText;

    public void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        isGameActive = true;
        scoreText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        UpdateLevel(1);
    }

    // Update score with value from target clicked
    public void UpdateScore(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Easy:
                score += 100;
                break;
            case EnemyType.Medium:
                score += 200;
                break;
            case EnemyType.Hard:
                score += 500;
                break;
        }

        scoreText.text = $"Score: {score}";
    }
    
    public void UpdateLevel(int waveNumber)
    {
        levelText.text = $"Level: {waveNumber}";
    }

    public void GameOver()
    {
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        retryButton.onClick.AddListener(RestartGame);
    }

    public void RestartGame()
    {
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseText.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseText.gameObject.SetActive(false);
    }
}