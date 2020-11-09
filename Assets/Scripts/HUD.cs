using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUD : MonoBehaviour
{
    public Text ScoreText;
    public GameObject Menu;
    public GameObject GameOverText;
    public Text ControlButtonText;
    public GameObject ResumeButton;
    public GameObject Player;

    void Start()
    {
        GameOverText.SetActive(false);
        ResumeButton.SetActive(false); 
        Pause(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Menu.activeInHierarchy)
        {
            Pause(true);
            Menu.SetActive(true);
        }
    }

    void Pause(bool state)
    {
        if (state) Time.timeScale = 0; else Time.timeScale = 1;
        Player.SetActive(!state);
    }

    public void ControlPressed()
    {
        Asteroids.ControllerType ^= true;
        ControlShow();
    }

    public void ControlShow()
    {
        if (Asteroids.ControllerType)
        {
            ControlButtonText.text = "Управление: клавиатура + мышь";
        }
        else
        {
            ControlButtonText.text = "Управление: клавиатура";
        }      
    }

    public void NewGamePressed()
    {
        GameObject ast;
        ast = GameObject.Find("Asteroids");
        ast.GetComponent<Asteroids>().StartNewGame();
        GameOverText.SetActive(false);
        ResumeButton.SetActive(true);
        ScoreText.text = "Score: 0"; 
        ResumePressed();
        Player.gameObject.GetComponent<PlayerControl>().SendMessage("VarReset");
    }

    public void ResumePressed()
    {
        Pause(false);
        Menu.SetActive(false);
    }

    public void ScreenPressed()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Выходим");
    }

    public void GameOver()
    {
        Menu.SetActive(true);  
        GameOverText.SetActive(true);
        ResumeButton.SetActive(false); 
        Pause(true); 
    }
}
