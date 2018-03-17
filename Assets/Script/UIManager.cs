using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    public string backSceneName;
    public bool pause = false;


    // Use this for initialization
    void Enable()
    {
        EventManager.StartListening("Victory", Victory);
        EventManager.StartListening("Defeat", Defeat);
        EventManager.StartListening("ButtonPressed", Defeat);
    }
    void Unable()
    {
        EventManager.StopListening("Victory", Victory);
        EventManager.StopListening("Defeat", Defeat);
        EventManager.StopListening("ButtonPressed", Defeat);
    }


    private void Victory(GameObject obj, string param)
    {
        //승리
        //승리UI 띄우기
    }

    private void Defeat(GameObject obj, string param)
    {
        //패배
        //패배UI 띄우기

    }

    private void PauseGame(bool pau)
    {
        pause = pau;
        Time.timeScale = pau ? 0f : 1f; //시간 조절(일시정지면 멈춰야 하므로 0f)
        EventManager.TriggerEvent("Pause", null, pause.ToString());
    }

    private void ShowPauseUI()
    {
        pauseMenu.SetActive(true);
        //여기에 화면의 유닛들 다 끄고싶으면 코드 추가
        PauseGame(true);
    }
    private void HidePauseUI()
    {
        pauseMenu.SetActive(true);
        //여기에 화면의 유닛들 다 끄고싶으면 코드 추가
        PauseGame(true);
    }

    private void ButtonPressed(GameObject obj, string param)
    { //mainmenu하고는 다름
        switch (param)
        {
            case "Back":
                SceneManager.LoadScene(backSceneName);
                break;
            case "Pause":
                ShowPauseUI();
                break;
            case "Resume":
                HidePauseUI();
                break;
        }
    }
}
