using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 유닛이 가지는/맵 전체의 부분적인 맵데이터 정보
 */

public class RoadDataFraction
{
    protected Vector2 start; //직선거리 시작점
    protected Vector2 end; //직선거리 도착점
    protected List<Vector2> roadData; //시작->도착까지의 타일위치(1칸 단위)
    public int lastDataPos; //데이터 생성시 길 추가해야되는 위치 알기위해 사용

    //길의 방향에 대한 변수들(크기가 2인 배열->전위부분 방향, 후위부분 방향)
    public int[] roadDir; //도로의 방향(1 좌->우,2 하->상,-1 우->좌,-2 상->하) 커지는 방향이면 양수, 작아지는 방향이면 음수
    protected Vector2 nowDir; //현재 도로방향 글로 표현한 것

    protected List<Vector2> variationPoint; //한 길조각이 이어지는 동안 방향이 바뀌는 위치, 다른 길조각과 합성시 사이에 (0,0)을 넣는다.
    protected List<Vector2> connectPoint; //다른 길조각과 연결된 경우 연결된 위치, 분해시 사용하기 위해서

    public RoadDataFraction(){
        roadData = new List<Vector2>();
        variationPoint = new List<Vector2>();
        connectPoint = new List<Vector2>(); 
        roadDir = new int[2];
        nowDir = new Vector2();
        lastDataPos = 0;
    }

    public List<Vector2> GetRoadData()
    {
        return roadData;
    }
    public Vector2 GetStart()
    {
        return start;
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
     * 함수 목적 : 이 데이터의 시작점/끝점이 roadData의 정보와 같은가, 시작점/끝점이 맵의 경계를 넘어서지 않는가 검사.
     *             안되어있으면 여기서 업데이트
     */
    private bool CheckEndPoints() 
    {
        if (roadData.Count == 0)
        { //roadData 자체가 없기 때문에 실패
            return false; 
        }
        if (start != roadData[0]) {
            start = roadData[0];
        }
        if (end == roadData[roadData.Count - 1])
        {
            end = roadData[roadData.Count - 1];
        }
        return true;
    }

    /*
     * 함수 목적 : 위와 동일(시작점, 도착점이 주어지는 경우 사용. 첫 생성시에는 이걸 불러서 맵 좌우에 붙어 있는지 검사해야됨)
     */
    private bool CheckEndPoints(Vector2 up, Vector2 down, Vector2 left, Vector2 right)
    {
        if (roadData.Count == 0)
        { //roadData 자체가 없기 때문에 실패
            return false;
        }
        if (((start.x!=left.x)||((start.y<down.y)||(start.y>up.y)))||
            ((end.x!=right.x)|| ((end.y < down.y) || (end.y > up.y))))
        {
            return false; //false를 받으면 파일 재검사를 하도록 하기
        }
        if (start != roadData[0])
        {
            start = roadData[0];
        }
        if (end == roadData[roadData.Count - 1])
        {
            end = roadData[roadData.Count - 1];
        }
        return true;
    }
    

    /*
     * 목적 : 맵 제작시 또는 맵 로드시에 길데이터 읽고 하나씩 추가하는 과정
     */
    public bool AddData(Vector2 data)
    {
        //여기 변경해야 됨(변곡점에 대한 체크가 없음)
        bool canInsert=CheckDirection(data); //길 조각이 진행 가능한 방향인가?

        if (canInsert)
        {
            if (start == null)
            { //시작점이 없는경우(맨 처음) 추가
                start = data;
            }
            end = data; //마지막 위치가 바뀌었으므로 변경

            SetNowDirectory(data);
            roadData.Add(data);

            return true;
        }
        return false;

    }

    private void SetNowDirectory(Vector2 vec) //현재 추가중인 길의 경로방향을 나타냄(맨 마지막에는 roadDir[1]과 같아야 정상임)
    {
        if (vec == start)
        {
            //맨 처음에는 start만 있어서 방향이 없으므로 pass시킬것
            return;
        }

        Vector2 direction = roadData[roadData.Count - 1] - vec;

        if (direction != nowDir)
        {
            variationPoint.Add(roadData[roadData.Count-1]); //roadData의 마지막 위치에서 변곡점이 발생!
        }

        nowDir = direction;

        if (roadData[lastDataPos] == start)
        { //시작점 바로 다음인 경우 roadDir의 0번(전위 방향)을 지정해줘야한다.
            if (nowDir.x == 1)
            {//상하
                roadDir[0] = 1;
            }
            else
            {// 우
                roadDir[0] = 2;
            }
            if (nowDir.x < 0 || nowDir.y < 0)
            {
                roadDir[0] *= -1;
            }
        }

        //후위 길 방향 갱신
        if (nowDir.x == 1)
        {//상하
            roadDir[1] = 1;
        }
        else
        {// 우
            roadDir[1] = 2;
        }
        if (nowDir.x < 0 || nowDir.y < 0)
        {
            roadDir[1] *= -1;
        }

    }

    public bool CheckDirection(Vector2 data) //방향과 거리 체크(방향은 역방향이 되어서는 안되며, 거리는 1초과가 되면 안된다.)
    {
        if (end == null)
        {//end가 없을때(처음 생성되었을때는 방향이 없으므로)
            return true; 
        }

        Vector2 temp = data - roadData[roadData.Count - 1];

        if (nowDir == null)
        { //없다는건 start점 바로 다음 점이니 전/후위 전부 같은 방향으로 설정
            //어떻게 수정하지?
            if(temp==new Vector2(-1, 0))
            {//시작점에서 좌측이동은 있을 수 없음
                return false;
            }
        }

        if(temp+nowDir==new Vector2(0, 0))
        { //0이 되는 경우는 서로 역방향일 경우
            return false;
        }

        float distance = Mathf.Sqrt(Mathf.Pow(temp.x, 2) + Mathf.Pow(temp.y, 2)); //연결(추가하려는) 위치와의 거리
        if (distance > 1)
        { //붙어있어야 추가가 가능!
            return false;
        }

        return true;
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

        if (!CheckDirection(other.roadData[1]))
        { //후위 길조각의 시작점 다음 위치와 전위 길조각의 종료지점의 방향이 맞는가?
            //1인 이유는 0이 시작점이며, 후위 시작점과 전위 종료점은 같기 때문이다.
            return false;
        }

        return true;
    }

}

public class UnitRoadData : RoadDataFraction
{

    /*
     * 다른 길데이터 조각과 합쳐서 하나의 길로 만드는 함수
     * RoadDataFraction말고 유닛의 RoadDataFraction과 합성 시도할 수도 있으니 제외시킬 방법 생각해야됨
     * roadDir->이어붙일 길의 진행방향(찾기)
     */
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
