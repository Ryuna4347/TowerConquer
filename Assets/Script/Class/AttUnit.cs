using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttUnit : UnitBase {

    private UnitRoadData unitPath;
    public MapRoadManager mapRoadManager;

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
