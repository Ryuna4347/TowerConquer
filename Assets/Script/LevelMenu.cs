using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{

    // Name of scene to start on click
    public string stageSceneName;
    public string backSceneName; //돌아가기 버튼 눌렀을때 돌아갈 신이름(메인메뉴)

    void OnEnable()
    {
        EventManager.StartListening("ButtonPressed", ButtonPressed); //메인화면은 UIManager가 담당하지 않는다.
    }
    void OnDisable()
    {
        EventManager.StopListening("ButtonPressed", ButtonPressed); //메인화면은 UIManager가 담당하지 않는다.
    }

    // Update is called once per frame
    void Awake()
    {
        stageSceneName = "";
    }

    private void ButtonPressed(GameObject obj, string param)
    { //mainmenu하고는 다름
        switch (param)
        {
            case "Back":
                SceneManager.LoadScene(backSceneName);
                break;
            case "LoadStage":
                LoadStage(stageSceneName);
                break;
            case "StageSelected":
                LoadStageInfo(obj.name);
                break;
        }
    }

    private void LoadStageInfo(string sceneName)
    {
        //if () 나중에 사용자 data불러와서 락해제된 stage인가도 확인해야함
        //{
        Sprite stageImage = Resources.Load<Sprite>(sceneName);
        Debug.Log(sceneName);
        GameObject.Find("StageImage").GetComponent<Image>().sprite = stageImage;
        GameObject.Find("StageNameText").GetComponent<Text>().text = sceneName;
        //}
    }
    private void LoadStage(string sceneName)
    {
        if (stageSceneName == "" || (stageSceneName == null))
        {
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
