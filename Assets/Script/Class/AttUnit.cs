using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttUnit : UnitBase {

    public int health_now;
    public float range_now;
    public GameObject rangeImg; //하위에 있는 공격범위 오브젝트

    private UnitRoadData unitPath;
    private UnitManager unitManager;
    public MapRoadManager mapRoadManager;
    private Vector2 nextPathPoint; //유닛이 이동할 다음 점

    private void Awake()
    {
        unitManager=GameObject.Find("UnitManager").GetComponent<UnitManager>();
    }

    public void SetUnitPath(RoadDataFraction road)
    {
        unitPath = new UnitRoadData(road);
    }
    public void ConnectPath(string roadName)
    {
        unitPath.ConnectWithRoad(roadName);
    }
    private void Enable()
    {
        //여기
    }
    public void Move()
    {
        //웨이브가 시작되어야 함
        bool HasStartEndRoad = MapRoadManager.CheckStartEndRoad(unitPath);
        if (HasStartEndRoad)
        {
            //nextPathPoint로 접근
            //nextPathPoint 도착시 nextPathPoint 갱신
        }
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
