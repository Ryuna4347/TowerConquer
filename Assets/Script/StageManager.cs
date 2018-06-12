using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XmlDefUnit
{//xml파일을 읽어서 defunit에 넣기 전에 중간과정(유니티의 xml 파서가 기본형밖에 지원 안 해주기 때문)
    //스테이지별 모든 유닛의 정보가 담겨있다.
    public int lev;
    public int totalWave;

    public string[] unitName;
    public int numOfUnit;
    public int[] unitPos;
    public int[] unitWaveInfo;
}
public class XmlAttUnit
{//xml파일을 읽어서 defunit에 넣기 전에 중간과정(유니티의 xml 파서가 기본형밖에 지원 안 해주기 때문)
    //스테이지별 모든 유닛의 정보가 담겨있다.
    public int lev;
    public int totalWave;

    public string[] unitName;
    public int numOfUnit;
    public string[] unitRoadName; //유닛이 지날 길의 이름들, "없음"으로 끊고 다음 유닛 길정보로 넘어감
    public int[] unitWaveInfo;
}
public class WaveInfo
{ //웨이브 별 유닛상태(우선은 수비만, 나중에 공격추가하게되면 같이쓰던지(안쓰는 속성은 빈칸으로하고 공/수여부 따져서 조절) 아니면 그냥 따로 클래스를 만들던지)
    //StageManager로 옮기는게 좋을거같고 StageManager는 이 클래스를 리스트로 가지고 있을 예정이다.
    public int wave; //웨이브 단계
    public List<GameObject> unitList;
    public List<Vector2> unitPosList;//순서는 유닛리스트와 동일
}

public class StageManager : MonoBehaviour {

    private string stageName = "";

    public UnitManager unitManager;
    public int lev;
    public bool isDefendMode;
    public List<WaveInfo> waveInfo; //웨이브별 유닛들의 정보
    public List<string> unitListForStage; //스테이지에서 사용할 공/수 포함 모든 유닛들(유저 수비기준 수비유닛은 별도의 xml파일에서 리스트 불러오기, 공격유닛은 스테이지 xml파일에 첨부되도록?)

    // Use this for initialization
    void Awake()
    {
        isDefendMode = true; //이건 나중에 attackMode추가하면 scene로딩할때 가져와야됨

        //UI는 따로 불러오기(LoadScene하고 속성은 additive로)
        //ui는 기본 ui, pause ui, 승패ui 총 4개
        
        if (isDefendMode)
        {
            unitManager.LoadAIAttUnitData(lev);
        }
        else
        {
            unitManager.LoadAIDefUnitData(lev); //컴퓨터 유닛 로드 실행(현재는 공격이 없으므로. 공격이 있으면 구별해서 실행시킬것)
        }
    }
}
