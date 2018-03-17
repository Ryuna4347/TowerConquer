using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class MapRoadData{
    RoadDataFraction[] allMapRoad;

    
}

public class RoadDataFraction
{
    private Vector2 start; //직선거리 시작점
    private Vector2 end; //직선거리 도착점
    private Vector2[] roadData; //시작->도착까지의 타일위치(1칸 단위)
    private int lastDataPos;
    private int roadDir; //도로의 방향(1 좌->우,2 하->상,-1 우->좌,-2 상->하) 커지는 방향이면 양수, 작아지는 방향이면 음수
    private Vector2 dir; //도로방향 글로 표현한 것

    public RoadDataFraction(){
        roadData = new Vector2[2];
    }

    public RoadDataFraction(int len)
    { //혹시 맵길이를 최소길이로 줄수도 있지 않을까해서
        roadData = new Vector2[len];
        lastDataPos = 0;
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
            return true;
        }
        return false;

    }

    private void SetDirectory()
    {
        if (Mathf.Abs(roadDir) == 1)
        {
            dir = new Vector2(1, 0);
        }
        else
        {
            dir = new Vector2(0, 1);
        }
        if (roadDir<0)
        {
            dir*=-1;
        }
    }

    public bool CheckDirection(Vector2 data)
    {
        if (end == null)
        {//end가 없을때(처음 생성되었을때는 방향이 없으므로)
            return true; 
        }

        if (dir == null)
        {
            SetDirectory();
        }

        Vector2 dataDir = data - end;

        if (dataDir != dir)
        {//방향이 같아야 추가진행가능
            return false;
        }
        else
        {
            return true;
        }
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
        if (roadDir + other.roadDir == 0)
        { //방향이 상반된 길끼리 붙으려고 하는경우(애초에 안되긴 하는데 혹시몰라서 추가)
            return false;
        }
        if (end != other.start)
        { //후위에서 전위로 요청을 하는것이므로 후위의 출발점이 전위의 도착점과 일치해야한다.
            return false;
        }
        return true;
    }
}