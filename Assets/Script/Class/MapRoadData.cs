using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 유닛이 가지는/맵 전체의 부분적인 맵데이터 정보
 */

public class RoadDataFraction
{
    public Vector2 start; //직선거리 시작점(public인 이유 : 에디터에서 좌표 지정해서 프리팹으로 저장할거라)
    public Vector2 end; //직선거리 도착점
    protected List<Vector2> roadData; //시작->도착까지의 타일위치(1칸 단위)


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

public class UnitRoadData : RoadDataFraction
{
    protected List<Vector2> variationPoint; //한 길조각이 이어지는 동안 방향이 바뀌는 위치, 다른 길조각과 합성시 사이에 (0,0)을 넣는다.
    protected List<Vector2> connectPoint; //다른 길조각과 연결된 경우 연결된 위치, 분해시 사용하기 위해서

    public UnitRoadData()
    {
        variationPoint = new List<Vector2>();
        connectPoint = new List<Vector2>();
    }
    public List<Vector2> GetVariation()
    {
        return variationPoint;
    }
    public List<Vector2> GetConnection()
    {
        return connectPoint;
    }

    /*
     * 목적 : 다른 길 조각과 연결이 가능한가?(갈림길에서 유저가 방향 선택시 해당 방향 길 조각과 연결하기 위함)
     * 사용 규칙 : 진행방향에서 후위에 있는 길에서 전위에 있는 길에게 요청
     */
    public bool CanConnectToOtherRoad(RoadDataFraction other)
    {
        if (start == null || end == null)
        {
            return false;
        }

        if (end != other.start)
        { //후위에서 전위로 요청을 하는것이므로 후위의 출발점이 전위의 도착점과 일치해야한다.
            return false;
        }

        if (!CheckRoadOverlap(other))
        { //후위 길조각의 시작점 다음 위치와 전위 길조각의 종료지점의 방향이 맞는가?
            //1인 이유는 0이 시작점이며, 후위 시작점과 전위 종료점은 같기 때문이다.
            return false;
        }

        return true;
    }

    /*
     * 길과 길조각의 겹침조사
     */

    public bool CheckRoadOverlap(RoadDataFraction data)
    { //길과 길조각의 겹침조사
        
        


        return true;
    }


    /*
     * 다른 길데이터 조각과 합쳐서 하나의 길로 만드는 함수
     * RoadDataFraction말고 유닛의 RoadDataFraction과 합성 시도할 수도 있으니 제외시킬 방법 생각해야됨
     * roadDir->이어붙일 길의 진행방향(찾기)
     */


    //수정필요
    //방향성이 필요없도록, int roadDir이 아닌 방향을 선택했을때의 길조각을 인자로 주고 유닛의 경로와 완전 겹치는 구간이 있는지 확인할것(겹치면 길이 겹친다는 소리니까 못감)

    public void ConnectWithRoad(int roadDir, Vector2 pos)
    {
        RoadDataFraction searchResult = GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().SearchData(pos, roadDir);

        if (CanConnectToOtherRoad(searchResult)) //결합이 가능한가?(전/후위의 길조각들이 이어지는 위치가 맞는가)
        {
            //변곡점 사이에 (-1*합성횟수, -1*합성횟수) 넣고 합성
            variationPoint.Add(new Vector2(-1*connectPoint.Count, -1*connectPoint.Count));
            variationPoint.AddRange(searchResult.GetVariation()); //검사필요


            roadData.AddRange(searchResult.GetRoadData());

            connectPoint.Add(pos); //결합점 정보 추가
        }
        else
        {
            return;
        }
    }

    public void SeperateRoadData(int roadDir)
    { 
        if (variationPoint.Count == 0)
        {//결합점이 없으면 결합된 길이 아니므로 취소
            return;
        }
        while (variationPoint[variationPoint.Count-1]!=new Vector2(-1*connectPoint.Count,-1*connectPoint.Count)) //마지막 합성된 장소가 나올때 까지 진행
        {
            variationPoint.RemoveAt(variationPoint.Count - 1);
        }
        Vector2 connectedPoint = connectPoint[connectPoint.Count - 1];
        connectPoint.RemoveAt(connectPoint.Count - 1); //맨 끝에 있는애 삭제

        //roadData 삭제(삭제할때 아예 흔적 안남게 어떻게 지울까?)
    }
}
