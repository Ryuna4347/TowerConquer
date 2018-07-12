using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefUnit : UnitBase {
    public int health_now;
    public float range_now;
    public List<int> powerList; //유닛 레벨별 공격력
    public int power_now;


    public override void OnEnable()
    {
        ResetUnit();
        //여기
    }

    public override void ResetUnit()
    {//오브젝트가 사라질 경우(유닛 제거 및 파괴) 1레벨 기준으로 능력치, 이미지를 되돌리기
        lev_now = 1;
        health_now = healthList[lev_now];
        gameObject.GetComponent<SpriteRenderer>().sprite = unitImgList[0];
        power_now = powerList[lev_now];
        range_now = searchRangeList[lev_now];
    }

    public void CheckTarget()
    {//범위 내에 있던 타겟이 죽을경우 유닛매니저에서 전체 유닛에 타겟 재검사를 요청한다.(target이 살아있는지 확인)
        //bool isTargetAlive=UnitManager.CheckTargetStatus(targetUnit); 유닛매니저에게 상태요청(unused인가 used인가)
        //if (!isTargetAlive)
        //{//죽어있다면

        //}
    }
}
