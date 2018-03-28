using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 유닛이 가지는/맵 전체의 부분적인 맵데이터 정보
 */

public class RoadDataFraction
{
    private Vector2 start; //직선거리 시작점
    private Vector2 end; //직선거리 도착점
    private Vector2[] roadData; //시작->도착까지의 타일위치(1칸 단위)
    private int lastDataPos; //데이터 생성시 길 추가해야되는 위치 알기위해 사용

    //길의 방향에 대한 변수들(크기가 2인 배열->전위부분 방향, 후위부분 방향)
    public int[] roadDir; //도로의 방향(1 좌->우,2 하->상,-1 우->좌,-2 상->하) 커지는 방향이면 양수, 작아지는 방향이면 음수
    private Vector2 nowDir; //현재 도로방향 글로 표현한 것

    private Vector2[] variationPoint; //한 길조각이 이어지는 동안 방향이 바뀌는 위치, 다른 길조각과 합성시 사이에 (0,0)을 넣는다.
    private Vector2[] connectPoint; //다른 길조각과 연결된 경우 연결된 위치, 분해시 사용하기 위해서

    public RoadDataFraction(){
        roadData = new Vector2[2];
        variationPoint = new Vector2[3];
        connectPoint = new Vector2[3]; //부족할 경우 더 늘릴것
        roadDir = new int[2];
        nowDir = new Vector2();
    }

    public RoadDataFraction(int len)
    { //혹시 맵길이를 최소길이로 줄수도 있지 않을까해서
        roadData = new Vector2[len];
        variationPoint = new Vector2[3];
        connectPoint = new Vector2[3]; //부족할 경우 더 늘릴것
        roadDir = new int[2];
        nowDir = new Vector2();
        lastDataPos = 0;
    } 

    public Vector2 GetStart()
    {
        return start;
    }
    /*
     * 함수 목적 : 이 데이터의 시작점/끝점이 roadData의 정보와 같은가, 시작점/끝점이 맵의 경계를 넘어서지 않는가 검사.
     *             안되어있으면 여기서 업데이트
     */
    private bool CheckEndPoints() 
    {
        if (roadData.Length == 0)
        { //roadData 자체가 없기 때문에 실패
            return false; 
        }
        if (start != roadData[0]) {
            start = roadData[0];
        }
        if (end == roadData[roadData.Length - 1])
        {
            end = roadData[roadData.Length - 1];
        }
        return true;
    }

    /*
     * 함수 목적 : 위와 동일(시작점, 도착점이 주어지는 경우 사용. 첫 생성시에는 이걸 불러서 맵 좌우에 붙어 있는지 검사해야됨)
     */
    private bool CheckEndPoints(Vector2 up, Vector2 down, Vector2 left, Vector2 right)
    {
        if (roadData.Length == 0)
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
        if (end == roadData[roadData.Length - 1])
        {
            end = roadData[roadData.Length - 1];
        }
        return true;
    }

    /*
     * 목적 : 맵 제작시 또는 맵 로드시에 길데이터 읽고 하나씩 추가하는 과정
     */
    public bool AddData(Vector2 data)
    {
        //여기 변경해야 됨(변곡점에 대한 체크가 없음)
        bool canInsert=CheckDirection(data); //길 파편이 가는 방향과 일치하는지 체크

        if (canInsert)
        {

            if (roadData[roadData.Length - 1] != null)
            { //길데이터가 가득 찬 상황이면 배열길이를 1.5배(소수점 올림)로 증가
                Vector2[] temp = new Vector2[(int)Mathf.Ceil(roadData.Length * 1.5f)];
                roadData.CopyTo(temp, roadData.Length); //여기는 검사해봐라
                roadData = temp;
            }

            roadData[lastDataPos++] = data;

            //변곡에 대한 정보(변곡점 추가, roadDir[1]의 변경)
            if (start == null)
            { //시작점이 없는경우(맨 처음) 추가
                start = data;
            }
            end = data; //마지막 위치가 바뀌었으므로 변경

            SetNowDirectory(data);

            return true;
        }
        return false;

    }

    private void SetNowDirectory(Vector2 vec) //현재 추가중인 길의 경로방향을 나타냄(맨 마지막에는 roadDir[1]과 같아야 정상임)
    {
        //맨 처음에는 start만 있어서 방향이 없으므로 pass시킬것
    }

    public bool CheckDirection(Vector2 data)
    {
        if (end == null)
        {//end가 없을때(처음 생성되었을때는 방향이 없으므로)
            return true; 
        }

        if (nowDir == null)
        { //없다는건 start점 바로 다음 점이니 전/후위 전부 같은 방향으로 설정
            //어떻게 수정하지?
            Vector2 temp = data - start;
            if(temp==new Vector2(-1, 0))
            {//시작점에서 좌측이동은 있을 수 없음
                return false;
            }

            //roadDir 0,1 설정하기
            //여기부터
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
        if (roadDir[1] + other.roadDir[0] == 0)
        { //방향이 상반된 길끼리 붙으려고 하는경우(현재 길 조각의 후위방향, 후위 길조각의 전위방향)
            return false;
        }
        if (end != other.start)
        { //후위에서 전위로 요청을 하는것이므로 후위의 출발점이 전위의 도착점과 일치해야한다.
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
    public UnitRoadData ConnectWithRoad(int roadDir)
    {
        if (CanConnectToOtherRoad(other)) //결합이 가능한가?(전/후위의 길조각들이 이어지는 위치가 맞는가)
        {
            //변곡점 사이에 (0,0) 넣고 합성
        }
        else
        {
            return null;
        }
    }
}