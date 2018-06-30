using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * xml에서 mapdata로 전환할때 FromJSON을 사용할 대상이 없기때문에 임시로 받아서 변환시킨후 MapRoadManager에 저장하기 위한 용도
 */
public class XmlMapRoad
{
    public string[] name;
    public int lev;
    public int numOfRoad;
    public int[] roadDataInt;
    public string[] start; //시작지점의 길이름
    public string[] end; //종료지점의 길이름
}
/*
 *게임내에서 길에 관련된 데이터를 총괄
 */
public class MapRoadManager : MonoBehaviour
{
    public List<RoadDataFraction> allMapRoad;
    private List<string> startPoint;
    private List<string> endPoint;

    private void Awake()
    {
        allMapRoad = new List<RoadDataFraction>();
    }

    private RoadDataFraction[] IncreaseArrSize()
    { //배열 오버플로우시
        RoadDataFraction[] temp = new RoadDataFraction[allMapRoad.Count * 2];
        allMapRoad.CopyTo(temp, allMapRoad.Count);
        return temp;
    }

    public RoadDataFraction SearchData(string roadName)
    {
        for (int i = 0; i < allMapRoad.Count; i++)
        {
            if (allMapRoad[i].name == roadName)
            { //해당 좌표에서 선택한 방향으로 진행하는 
                return allMapRoad[i];
            }
        }
        return null;
    }


    /*
     *유저가 유닛설치를 위해 클릭한 좌표에 유닛이 이미 놓여있는지 조사
     */
    public bool CheckRoadData(Vector2 mousePos)
    {
        Debug.Log(mousePos);
        foreach (RoadDataFraction road in allMapRoad)
        {
            string temp = "";
            foreach(Vector2 t in road.roadData)
            {
                temp += t+" ";
            }
            if (road.roadData.Contains(mousePos))
            {//만약 해당 위치를 가지는 길조각이 있다면 그곳은 길이기 때문에 설치를 받아들일 수 없다.
                return false;
            }
        }
        return true;
    }

    /*
     * StageManager가 호출하여 xml파일로부터 맵데이터를 전달받아 저장하는 함수
     */
    public void LoadMapRoadData(int mapLev)
    {
        string[] roadMapDataText = File.ReadAllText("Assets/SaveFolder/RoadData.txt").Split('\n');
        int fileLen = roadMapDataText.Length;

        if (fileLen < 1)
        {  //저장된 정보가 있어야 불러옴
            return;
        }
        for (int i = 0; i < fileLen; i++)
        {
            if (roadMapDataText[i] == "")
            { //빈칸이었을 경우 제외(2중엔터시 나올 수 있음)
                continue;
            }
            XmlMapRoad tempMapRoad = JsonUtility.FromJson<XmlMapRoad>(roadMapDataText[i]);
            if (tempMapRoad.lev == mapLev)
            { //로드하려는 맵과 같을 경우에는 따로 저장해둘것
                int roadIndex = 0;
                for (int j = 0; j < tempMapRoad.numOfRoad; j++)
                {//모든 길에 대한 데이터 저장
                    RoadDataFraction roadDataFrac = new RoadDataFraction();
                    roadDataFrac.name = tempMapRoad.name[j]; //이름은 길 순서대로(길조각 좌표도 길 순서대로)

                    int roadDataLen = tempMapRoad.roadDataInt.Length;
                    List<Vector2> tempRoadData = new List<Vector2>();
                    
                    while (roadIndex<roadDataLen)
                    {//roadIndex 위치의 값이 -1000(있을 수 없는값=길조각 분리 인식을 위한 더미값)일때까지 진행
                        if(tempMapRoad.roadDataInt[roadIndex] == -1000 )
                        {
                            roadIndex += 1; //roadIndex 증가(다음칸이 다음 길조각 시작이기 때문)
                            break;
                        }
                        Vector2 roadFracPos = new Vector2(tempMapRoad.roadDataInt[roadIndex], tempMapRoad.roadDataInt[roadIndex + 1]);
                        tempRoadData.Add(roadFracPos);
                        roadIndex += 2; //roadIndex를 한번에 2개씩(x,y 하나씩)사용하므로 2씩 증가해야한다.
                    }

                    foreach(string startVal in tempMapRoad.start)
                    {
                        startPoint.Add(startVal);
                    }
                    foreach (string endVal in tempMapRoad.end)
                    {
                        endPoint.Add(endVal);
                    }

                    roadDataFrac.roadData = tempRoadData;
                    roadDataFrac.start = roadDataFrac.roadData[0]; //길조각 시작점과 종착점 선언
                    roadDataFrac.end = roadDataFrac.roadData[roadDataFrac.roadData.Count-1]; //-1은 0부터 시작하기때문
                    allMapRoad.Add(roadDataFrac);
                }
            }
        }
    }

    public bool CheckStartEndRoad(UnitRoadData unitRoad)
    {
        if (startPoint.Contains(unitRoad.roadDataNameLIst[0]) && endPoint.Contains(unitRoad.roadDataNameLIst[unitRoad.roadDataNameLIst.Count-1]))
        { //유닛의 첫번째와 마지막 길조각은 시작/끝 길조각이어야 한다.
            return true;
        }
        else
        {
            return false;
        }
    }
}
