using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 유닛이 가지는/맵 전체의 부분적인 맵데이터 정보
 */

public class RoadDataFraction
{
    public string name; //길조각마다 고유의 명칭을 가진다.(합성후의 길에는 사용하지 않는다.)
    public Vector2 start; //직선거리 시작점(public인 이유 : 에디터에서 좌표 지정해서 프리팹으로 저장할거라)
    public Vector2 end; //직선거리 도착점
    public List<Vector2> roadData; //시작->도착까지의 타일위치(1칸 단위)


    public RoadDataFraction(){
        roadData = new List<Vector2>();
    }

    public List<Vector2> GetRoadData()
    {
        return roadData;
    }
    public Vector2 GetStart()
    {
        return start;
    }



}

public class UnitRoadData : MonoBehaviour
{
    public List<Vector2> unitRoadData;
    protected List<string> connectPoint; //다른 길조각과 연결된 경우 연결된 길조각들의 이름. 분해시 해당 이름의 길조각으로 분리

    public UnitRoadData()
    {
        unitRoadData = new List<Vector2>();
        connectPoint = new List<string>();
    }
    public UnitRoadData(RoadDataFraction road)
    {
        unitRoadData = road.GetRoadData();
        connectPoint = new List<string>();
    }
    public List<string> GetConnection()
    {
        return connectPoint;
    }

    /*
     * 목적 : 다른 길 조각과 연결이 가능한가?(갈림길에서 유저가 방향 선택시 해당 방향 길 조각과 연결하기 위함)
     * 사용 규칙 : 진행방향에서 후위에 있는 길에서 전위에 있는 길에게 요청
     */
    public bool CanConnectToOtherRoad(RoadDataFraction other)
    {
        if (unitRoadData[0] == null)
        {
            return false;
        }

        if (unitRoadData[unitRoadData.Count]==other.start)
        { //후위에서 전위로 요청을 하는것이므로 후위의 출발점이 전위의 도착점과 일치해야한다.
            return false;
        }

        if (!CheckAvailableOverlap(other))
        { 
            return false;
        }

        return true;
    }

    /*
     * 길과 길조각의 겹침조사
     * ConnectWithRoad()에서 실행하는 함수
     */

    public bool CheckAvailableOverlap(RoadDataFraction data)
    { //길과 길조각의 겹침조사 및 data의 시작점와 기존 길의 끝점이 같은지(붙은 상태인지) 조사
        
        //추가하려는 길과 기존의 마지막 길조각을 비교(겹치면 설치불가) 첫번째->출발점(합성길의 경우 connectionPoint의 마지막 부분), 두번째->도착점
        RoadDataFraction unitLastRoadPart=GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().SearchData(connectPoint[connectPoint.Count-1]);
        int overlapNum=0; //겹친 경우의 수(1은 기본으로 나와야하며 2가 나오면 어딘가 겹친다는 소리이므로 false반환)

        if (unitLastRoadPart.end != data.start)
        { //접촉된 길이 아님
            return false;
        }

        if (unitLastRoadPart.name == data.name)
        { //같은 길은 허용이 되지 않는다.
            return false;
        }

        for (int i = 0; i < data.roadData.Count; i++)
        {//두 길사이에 겹치는 부분이 있는가
            if (unitLastRoadPart.roadData.Contains(data.roadData[i]))
            {
                overlapNum++;
                if (overlapNum >= 2)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /*
     * 다른 길데이터 조각과 합쳐서 하나의 길로 만드는 함수
     * RoadDataFraction말고 유닛의 RoadDataFraction과 합성 시도할 수도 있으니 제외시킬 방법 생각해야됨
     * roadDir->이어붙일 길의 진행방향(찾기)
     */

        
    public bool ConnectWithRoad(string roadName)
    {
        RoadDataFraction searchResult = GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().SearchData(roadName); //갈림길의 각 방향버튼은 연결되는 길의 이름을 소지(에디터로 추가)

        if (CanConnectToOtherRoad(searchResult)) //결합이 가능한가?(전/후위의 길조각들이 이어지는 위치가 맞는가)
        {
            unitRoadData.AddRange(searchResult.GetRoadData());
            //여기서 아마 겹쳐지는 점이 2개가 생겨버리게 되버릴것. 수정필요

            connectPoint.Add(roadName); //결합점 정보 추가
            return true;
        }
        else
        { //합성 불가
            return false;
        }
    }

    /*
     * 유저가 길을 다른쪽으로 변경하기 위해서 특정 화살표를 다른 방향으로 클릭시 그 좌표를 기준으로 길을 절단
     * pos->분리하려는 기준점의 위치(여기부터 오른쪽 길 조각들은 잘림), roadName->새로 누른 화살표
     */
    public void SeperateRoadData(Vector2 pos, string roadName)
    { 
        if (connectPoint.Count==0)
        {//결합점이 없으면 결합된 길이 아니므로 취소
            return;
        }

        //pos에 해당하는 길조각을 찾고, 그 위치 이후부터 마지막까지의 길 정보는 삭제한다.
        foreach (string road in connectPoint)
        {
            RoadDataFraction roadInfo=GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().SearchData(road);
            if (roadInfo.start == pos)
            { //찾았으면 0~그 위치까지의 경로와 접합점의 리스트를 따로 빼낸다.
                int connectionIndex = connectPoint.IndexOf(road);
                int roadIndex = unitRoadData.IndexOf(pos);

                unitRoadData = unitRoadData.GetRange(0, roadIndex + 1);//+1인 이유 : pos까지 포함해야 하기때문에
                connectPoint = connectPoint.GetRange(0, connectionIndex+1);
            }
        }
    }
}
