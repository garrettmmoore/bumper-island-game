using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class GameManager : MonoBehaviour
{
    public int score;
    public bool isGameActive;
    [NonSerialized] public const float IdleTimeSetting = 10.0f;
    [NonSerialized] public float lastIdleTime;
    
    public Button retryButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI pauseText;

    public void Awake()
    {
        lastIdleTime = Time.time;
    }
    
    public void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        isGameActive = true;
        scoreText.gameObject.SetActive(true);
    }
    
    // Update score with value from target clicked
    public void UpdateScore( EnemyType enemyType)
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

    public bool IdleCheck()
    {
        var check = Time.time - lastIdleTime > IdleTimeSetting;
        return check;
    }
}