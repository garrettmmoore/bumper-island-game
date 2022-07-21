using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    [NonSerialized] public int score;
    public bool isGameActive;
    public bool isSandboxMode;
    public bool isSandboxModeNoEnemies;

    public Button retryButton;
    public Button resumeButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI pauseText;

    public void Awake()
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
        Time.timeScale = 1;
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseText.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        resumeButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        pauseText.gameObject.SetActive(false);
    }
}