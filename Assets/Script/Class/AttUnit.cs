using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttUnit : UnitBase {

    public int health_now;
    public float range_now;
    private bool hasStartEndRoad;
    private bool isMoving;
    public float movSpeed;
    public float attSpeed;
    
    private UnitRoadData unitPath;
    private UnitManager unitManager;
    public MapRoadManager mapRoadManager;
    private Vector2 nextPathPoint; //유닛이 이동할 다음 점
    private Vector2 normalizedVector; //유닛이 가야할 방향의 벡터

    private void Awake()
    {
        unitManager=GameObject.Find("UnitManager").GetComponent<UnitManager>();
        hasStartEndRoad = isMoving = false;
        nextPathPoint = new Vector2(-1,-1);
    }

    public void StartUnit()
    {
        if (!isMoving)
        {
            bool hasStartEndRoad = mapRoadManager.CheckStartEndRoad(unitPath);
            if (hasStartEndRoad)
            {
                StartCoroutine("MoveUnit");
            }
        }
    }
    private void MoveUnit()
    { //1프레임마다 움직임
        if (nextPathPoint == new Vector2(-1,-1))
        {
            nextPathPoint = unitPath.unitRoadData[1]; //0은 시작점
            normalizedVector = nextPathPoint - (Vector2)gameObject.transform.position;
        }
        Vector2 movMag = (nextPathPoint - (Vector2)gameObject.transform.position) - (normalizedVector * movSpeed * Time.deltaTime); //이동할 양과 현재 다음 위치까지 남은 거리를 비교해서 만약 초과한다면 다음 위치까지만 이동하도록 조정
        if (movMag.x < 0 || movMag.y < 0)
        {
            Vector2 changedVector = (nextPathPoint - (Vector2)gameObject.transform.position) / (movSpeed * Time.deltaTime); //원래는 speed를 변형해야 하지만 Vector2의 빼기를 float로 변환하기가 코드가 길어질 것 같아서 normalizedVector를 수정
            gameObject.transform.Translate(changedVector * movSpeed * Time.deltaTime);

            nextPathPoint = unitPath.unitRoadData[unitPath.unitRoadData.IndexOf(nextPathPoint) + 1]; //다음 점에 도착을 했으므로 다음 갈 곳을 찾아야한다.
            normalizedVector = nextPathPoint - (Vector2)gameObject.transform.position;
        }
        else
        {
            gameObject.transform.Translate(normalizedVector * movSpeed * Time.deltaTime);
        }
        
    }

    public void UnitDead()
    {
        EventManager.TriggerEvent("UnitDead", gameObject,""); //UnitManager에 유닛의 죽음을 알림
    }

    public void SetUnitPath(RoadDataFraction road)
    {
        unitPath = new UnitRoadData(road);
    }
    public void ConnectPath(string roadName)
    {
        unitPath.ConnectWithRoad(roadName);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        ResetUnit();
        //여기
    }
    public void Move()
    {
            //nextPathPoint로 접근
            //nextPathPoint 도착시 nextPathPoint 갱신
    }
    public override void ResetUnit()
    {//오브젝트가 사라질 경우(유닛 제거 및 파괴) 1레벨 기준으로 능력치, 이미지를 되돌리기
        health_now = healthList[0];
        gameObject.GetComponent<SpriteRenderer>().sprite = unitImgList[0];
        range_now = searchRangeList[0];
    }
    /*
     * 유닛의 이동 경로 설정
     */
    public void SetPath(List<string> roadList)
    {
        foreach(string roadName in roadList)
        {
            if (unitPath == null)
            { //유닛 길이 없는 상태면 초기값 지정
                RoadDataFraction roadData = mapRoadManager.SearchData(roadName);
                SetUnitPath(roadData);
            }
            else
            {
                ConnectPath(roadName);
            }
        }
    }
}
