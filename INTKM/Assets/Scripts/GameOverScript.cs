using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{

    public static bool IsOver = false;
    public GameObject gameoverMenu;

    public PierrePlayer player1;
    public PierrePlayer player2;

    


    void Update()
    {
        if (player1.IsDead() && player2.IsDead())
        {
            Over();
        }
        else return;
    }

    public void Retry()
    {
        gameoverMenu.SetActive(false);
        Time.timeScale = 1f;
        IsOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void Over()
    {
        gameoverMenu.SetActive(true);
        Time.timeScale = 0f;
        IsOver = true;
    }

    public void LoadMenu()
    {
        gameoverMenu.SetActive(false);
        Time.timeScale = 1f;
        IsOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}
