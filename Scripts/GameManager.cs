using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UI_Manager UI_Manager;
    
    public Player player;

    private bool gameIsOver;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    private void GameOver()
    {
        UI_Manager.ShowGameOverUI();
    }

    public void GameIsFinish()
    {
        UI_Manager.ShowGameFinishedUI();
    }

    void Update()
    {
        if (gameIsOver)
            return;

        if (Input.GetKeyUp(KeyCode.Escape))
            UI_Manager.TogglePauseUI();

        if (player.currentState == Player.PlayerState.Dead)
        {
            gameIsOver = true;

            GameOver();
        }
    }

    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
