using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/*
 * 유닛 및 설지 전 유닛 이미지를 보여주는 것을 관리 
 */
public class UnitManager : MonoBehaviour {

    //public AttUnit[] attUnitList; 공격 유닛 리스트
    public List<GameObject> unusedDefUnitList;
    public List<GameObject> usingDefUnitList;
    public GameObject attUnitParent; //유닛 instantiate 할때 부모설정할 오브젝트들
    public GameObject defUnitParent;

    private List<Sprite> unitSpriteList;

    private string selectedUnit; //설치를 위해 유저가 선택한 유닛의 이름
    private Vector2 selectMousePos;//설치를 위해 유저가 마우스로 클릭했던 곳

    private string AIselectedUnit; //스테이지 초반 컴퓨터유닛 로드 시 사용(predicate 때문에 쓸 수 밖에 없는듯)
    public StageManager stageManager;
    public CameraControl cameraControl; //맵이미지 로드를 위해서 받아놓기(focusObjectRenderer 때문)
    private GameObject installBtn; //유닛 설치/취소버튼

    // Use this for initialization
    void Awake () {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        cameraControl = GameObject.Find("Main Camera").GetComponent<CameraControl>();

        unitSpriteList = new List<Sprite>();
	}

    /*
     *인자로 들어온 값의 위치에 유닛 소환
     */
    public void BuildUnit(Vector3 movMouse)
    {

    }
    /*
     *유저가 유닛선택 ui에서 유닛을 선택할시 설정
     */
    public void SelectUnit(GameObject unit)
    {
        //gameManager에서 플레이어의 공/수 여부와 유닛의 공/수여부가 일치하는지 파악할 것(수정)

        Debug.Assert(installBtn, "cannot find install/cancle button");

    
        selectedUnit = unit.name;

        installBtn.SetActive(true);
        installBtn.transform.localPosition = selectMousePos;
        //여기서 unitCreateUI 안에 있는 설치/취소버튼의 active와 위치를 조정할 것
    }
    public void SelectUnit(string unitName)
    {
        //걸러내는 조건 있으면 붙이기(유닛이 사용가능한 스테이지인가?)
        Debug.Assert(installBtn, "cannot find install/cancle button");

        selectedUnit = unitName;

        installBtn.SetActive(true);
    }
    public GameObject SearchNSelectUnit(string unitName)
    {
        int installUnit = unusedDefUnitList.FindIndex(findName=>findName.name.Contains(unitName)); //이거 한개가 나오나 여러개가 나오나 봐야됨
        GameObject useUnit = unusedDefUnitList[installUnit];
        if (useUnit == null)
        {
            Debug.Log("유닛이 남지 않았습니다.");
            return null;
        }

        unusedDefUnitList.Remove(useUnit); //이제 사용할 유닛이 되므로 미사용유닛리스트에서 삭제 후 사용유닛리스트에 추가
        usingDefUnitList.Add(useUnit);

        return useUnit;
    }

    public void SetPosition(Vector2 movMouse)
    {
        selectMousePos = movMouse;
    }

    /*
     *유닛 생성 전 UI에서 유닛 선택 시 미리보여주기 및 설치/취소 관련 함수
     */
    public void SetSemiInstall()
    {
        if (selectMousePos == null || selectedUnit == null)
        { //유닛이나 조표를 지정하지 않으면 쓸 수 없음
            return;
        }

        //유닛 이미지를 해당 위치에
        //selectedUnit에서 이미지만 가진 오브젝트를 어떻게 검색해야하는지


    }

    public void InstallUnit()
    {
        if (selectMousePos == null || selectedUnit == null)
        { //유닛이나 조표를 지정하지 않으면 쓸 수 없음
            return;
        }

        int installUnit = unusedDefUnitList.FindIndex(ContainName); //이거 한개가 나오나 여러개가 나오나 봐야됨
        if (installUnit == -1)
        {//설치 불가능(사용 안 하고 있는 유닛 중에 그 유닛이 없음=더 지을 수 없음)
            Debug.Log("유닛을 설치 할 수 없습니다.(유닛 수 제한 or 유닛 설치 불가)");
            return;
        }
        GameObject useUnit = unusedDefUnitList[installUnit];
        unusedDefUnitList.Remove(useUnit); //이제 사용할 유닛이 되므로 미사용유닛리스트에서 삭제 후 사용유닛리스트에 추가
        usingDefUnitList.Add(useUnit);

        useUnit.SetActive(true);
        useUnit.transform.position = selectMousePos;

        cameraControl.CloseCreateUnitUI(); //유닛 생성 진행 이후 설치관련 UI 제거

    }

    public bool ContainName(GameObject unit)
    {
        if (unit.name.Contains(selectedUnit))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CancelInstallProcess()
    {
        selectedUnit = null;
        selectMousePos = new Vector2(-300,-300);
    }


    public void LoadAIDefUnitData(int mapLev)
    { //수비시 유닛정보(유닛명, 유닛 위치, 웨이브)를 얻어서 저장(따로 웨이브 정보를 담는 클래스 생성할 것)
        string[] AIDataText = File.ReadAllText("Assets/SaveFolder/AIData_Def.txt").Split('\n');
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
            XmlDefUnit tempAIDefUnit = JsonUtility.FromJson<XmlDefUnit>(AIDataText[i]);
            if (tempAIDefUnit.lev == mapLev)
            { //로드하려는 레벨과 같을 경우에는 따로 저장해둘것
                LoadUnitResources(tempAIDefUnit.unitName); //불러온 유닛들 설치
                cameraControl.LoadMapImage(mapLev);

                int numWave = tempAIDefUnit.totalWave; //해당 스테이지의 총 웨이브 수(웨이브에 따라 유닛이 다르기 때문)
                for (int k = 0; k < numWave; k++)
                {//모든 웨이브 정보가 들어갈 때까지 반복
                    WaveInfo tempWaveInfo = new WaveInfo();
                    tempWaveInfo.wave = k + 1;//0부터 시작이므로 1을 더해주고 웨이브 단계를 명시

                    int numUnit = tempAIDefUnit.numOfUnit;
                    for (int j = 0; j < numUnit; j++)
                    { //스테이지에 적힌 모든 공격 유닛에 대한 정보를 저장
                        if (tempAIDefUnit.unitWaveInfo[j] == k)
                        {//유닛이 들어가야할 웨이브 순서와 현재 저장중인 웨이브가 맞는다면 진행
                            GameObject tempDefUnit = SearchNSelectUnit(tempAIDefUnit.unitName[j]);
                            Vector2 tempDefUnitPos = new Vector2(tempAIDefUnit.unitPos[2 * j], tempAIDefUnit.unitPos[2 * j + 1]); //위 유닛의 위치
                            tempDefUnit.name +="_"+j; //이름 수정(유닛 구별 위함)
                            //웨이브 정보
                            tempWaveInfo.unitList.Add(tempDefUnit); //유닛과 유닛의 좌표를 리스트에 추가
                            tempWaveInfo.unitPosList.Add(tempDefUnitPos);
                        }
                    }
                    stageManager.waveInfo.Add(tempWaveInfo); //작성완료된 웨이브 정보를 리스트에 추가
                }
            }
        }
    }
    public void LoadAIAttUnitData(int mapLev)
    { //수비시 유닛정보(유닛명, 유닛 위치, 웨이브)를 얻어서 저장(따로 웨이브 정보를 담는 클래스 생성할 것)
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
            XmlAttUnit tempAIAttUnit = JsonUtility.FromJson<XmlAttUnit>(AIDataText[i]);
            if (tempAIAttUnit.lev == mapLev)
            { //로드하려는 레벨과 같을 경우에는 따로 저장해둘것
                LoadUnitResources(tempAIAttUnit.unitName); //불러온 유닛들 설치
                cameraControl.LoadMapImage(mapLev);

                int numWave = tempAIAttUnit.totalWave; //해당 스테이지의 총 웨이브 수(웨이브에 따라 유닛이 다르기 때문)
                for (int k = 0; k < numWave; k++)
                {//모든 웨이브 정보가 들어갈 때까지 반복
                    WaveInfo tempWaveInfo = new WaveInfo();
                    tempWaveInfo.wave = k + 1;//0부터 시작이므로 1을 더해주고 웨이브 단계를 명시
                    int count = 0;
                    int unitRoadLen = tempAIAttUnit.unitRoadName.Length;

                    int numUnit = tempAIAttUnit.numOfUnit;
                    for (int j = 0; j < numUnit; j++)
                    { //스테이지에 적힌 모든 수비 유닛에 대한 정보를 저장
                        if (tempAIAttUnit.unitWaveInfo[j] == k)
                        {//유닛이 들어가야할 웨이브 순서와 현재 저장중인 웨이브가 맞는다면 진행
                            GameObject tempAttUnit = SearchNSelectUnit(tempAIAttUnit.unitName[j]);
                            tempAttUnit.name += "_" + j; //이름 수정(유닛 구별 위함)

                            List<string> tempUnitPath = new List<string>(); //해당 유닛

                            while (tempAIAttUnit.unitRoadName[count]=="없음")
                            {
                                tempUnitPath.Add(tempAIAttUnit.unitRoadName[count]);
                                count++;
                            }
                            count++; //지금 위치가 "없음"위치일것이기 때문에 1 더 늘려야 다음 유닛때 길 추가가 가능하다.
                            
                            tempAttUnit.GetComponent<AttUnit>().SetPath(tempUnitPath);
                            tempWaveInfo.unitList.Add(tempAttUnit); //유닛과 유닛의 좌표를 리스트에 추가
                        }
                    }
                    stageManager.waveInfo.Add(tempWaveInfo); //작성완료된 웨이브 정보를 리스트에 추가
                }
            }
        }
    }

    public void LoadUnitResources(string[] unitNameList)
    { //미리 유닛 게임오브젝트 로드해서 스테이지 중 메모리 사용 줄이기
        //(나중에는 xml파일에 사용가능한 유닛들 넘겨버리기) 지금은 우선 다 불러와

        foreach (string unitName in unitNameList)
        {
            GameObject unit=null;
            if (unitName.Contains("Att"))
            {//공수별로 따로 유닛보관 
                unit=Instantiate(Resources.Load<GameObject>("Unit/" + unitName), attUnitParent.transform);
            }
            else if(unitName.Contains("Def"))
            {
                unit =Instantiate(Resources.Load<GameObject>("Unit/" + unitName), defUnitParent.transform);
            }
            else
            {
                Debug.Log("Loading Problem");
                continue;
            }
            Debug.Log(unit.name);
            unit.transform.localPosition = new Vector2(-1000, -1000);
            unit.name = unitName + "_" + unusedDefUnitList.Count;
            unusedDefUnitList.Add(unit);

            unit.SetActive(false);//리소스 낭비이므로 우선 지워둠

            Sprite tempUnitImage = unit.GetComponent<SpriteRenderer>().sprite; //이미 유닛 이름이 지정이 되어있어서 여기서 임시로 수정(나중에 스프라이트 구해서 내가 쓰게되면 그때는 그냥 이미지 자체의 이름으로 설정할 것)
            tempUnitImage.name = unitName;
            unitSpriteList.Add(tempUnitImage); //우선은 여기에하고 나중에 유닛선택창 때문에 전체유닛을 불러와야되면 따로 부르자.
        }
    }

    public Sprite GetUnitImage(string unitName)
    {
        foreach(Sprite image in unitSpriteList)
        {
            if (image.name == unitName)
            {
                return image;
            }
        }

        return null; //이미지가 없을경우
    }

    public void SetInstallBtn(GameObject obj)
    {
        installBtn = obj;
    }

    public Vector2 GetSelectMousePos()
    {
        return cameraControl.GetWorldPosition(selectMousePos);
    }

}
