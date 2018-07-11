using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;


public class XmlStageInfo
{//스테이지의 정보(레벨, 공수여부, 초기자금, 유닛정보)
    public int lev;
    public string stageMode; //공수모드
    public string unitInfo;
}
public class XmlDefUnit
{//xml파일을 읽어서 defunit에 넣기 전에 중간과정(유니티의 xml 파서가 기본형밖에 지원 안 해주기 때문)
    //스테이지별 모든 유닛의 정보가 담겨있다.
    public int totalWave;

    public string[] unitName;
    public int numOfUnit;
    public int[] unitPos;
    public int[] unitWaveInfo;
}
public class XmlAttUnit
{//xml파일을 읽어서 defunit에 넣기 전에 중간과정(유니티의 xml 파서가 기본형밖에 지원 안 해주기 때문)
    //스테이지별 모든 유닛의 정보가 담겨있다.
    public int totalWave;

    public string[] unitName;
    public int numOfUnit;
    public string[] unitRoadName; //유닛이 지날 길의 이름들, "없음"으로 끊고 다음 유닛 길정보로 넘어감
    public string[] unitStartPoint; //유닛이 시작하는 곳의 위치(숫자로 구성되어있으며, 각 숫자는 길이 시작되는 곳들을 모은 리스트(MapRoadManager에 생성)에서의 번호를 나타낸다.)
    public int[] unitWaveInfo; //유닛이 등장할 웨이브(0부터 시작)
}
public class WaveInfo
{ //웨이브 별 유닛상태(우선은 수비만, 나중에 공격추가하게되면 같이쓰던지(안쓰는 속성은 빈칸으로하고 공/수여부 따져서 조절) 아니면 그냥 따로 클래스를 만들던지)
    //StageManager로 옮기는게 좋을거같고 StageManager는 이 클래스를 리스트로 가지고 있을 예정이다.
    public int wave; //웨이브 단계
    public List<GameObject> unitList;
    public List<Vector2> unitPosList;//순서는 유닛리스트와 동일

    public WaveInfo()
    {
        unitList = new List<GameObject>();
        unitPosList = new List<Vector2>();
    }
}

public class StageManager : MonoBehaviour {

    private string stageName = "";

    public UnitManager unitManager;
    public int lev;
    public bool isDefendMode;
    private List<WaveInfo> waveInfo; //웨이브별 유닛들의 정보
    private List<string> unitListForStage; //스테이지에서 사용할 공/수 포함 모든 유닛들(유저 수비기준 수비유닛은 별도의 xml파일에서 리스트 불러오기, 
                                           //공격유닛은 스테이지 xml파일에 첨부되도록?)
                                           //사용처 : 설치UI에서 사용되는 유닛만 가능표시할때, 설치시 이 리스트 안에 있는 유닛인지 검사할때
    private int money; //현재 자금, 시작 로드시에는 초기 자금(스테이지 정보 얻을 때 같이 얻어와야 함)

    public CameraControl cameraControl;

    private int wave_now;
    private bool isGameStarting;


    // Use this for initialization
    void Awake()
    {
        SceneManager.LoadScene("StageUI", LoadSceneMode.Additive);

        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();

        waveInfo = new List<WaveInfo>();
        unitListForStage = new List<string>();

        GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().LoadMapRoadData(1); //lev으로 변경해야됨
        LoadStageData();


        wave_now = 1;
        isGameStarting = false;
    }

    private void Start()
    {
        if (!isGameStarting)
        {
            WaveStart();
        }
    }

    public List<WaveInfo> GetWaveInfo()
    {
        return waveInfo;
    }
    public List<string> GetUnitList()
    {
        return unitListForStage;
    }
    public int GetMoney()
    {
        return money;
    }

    private void LoadStageData()
    {
        string[] AIDataText = File.ReadAllText("Assets/SaveFolder/AIData_Att.txt").Split('\n');
        int fileLen = AIDataText.Length;

        if (fileLen < 1)
        {  //저장된 정보가 있어야 불러옴
            return;
        }
        for (int i = 0; i < fileLen; i++)
        {
            if (AIDataText[i] == "")
            { //빈칸이었을 경우 제외(2중엔터시 나올 수 있음)
                continue;
            }
            Debug.Log(AIDataText[i]);
            XmlStageInfo tempStageInfo = JsonUtility.FromJson<XmlStageInfo>(AIDataText[i]);
            if (tempStageInfo.lev == lev)
            { //원하는 스테이지일 경우
                isDefendMode = tempStageInfo.stageMode == "Def" ? true : false; //공수여부 판단
                

                if (isDefendMode)
                {
                    unitManager.LoadAIAttUnitData(tempStageInfo.unitInfo);
                }
                else
                {
                    unitManager.LoadAIDefUnitData(tempStageInfo.unitInfo); //컴퓨터 유닛 로드 실행(현재는 공격이 없으므로. 공격이 있으면 구별해서 실행시킬것)
                }
                cameraControl.LoadMapImage(lev);
                return; //레벨을 찾았으므로 뒤는 안봐도 된다
            }
        }
    }

    public void UseMoney(int cost)
    {
        if (money - cost < 0)
        {

        }
    }
    public void WaveStart()
    {
        unitManager.ReadyForWave(waveInfo[wave_now]); //웨이브에 해당하는 유닛들 지정위치에
        //5초 뒤 시작 안내
        Invoke("StartWave", 5);
    }
    public void StartWave()
    {
        EventManager.TriggerEvent("WaveStart",gameObject,"");
    }
}
