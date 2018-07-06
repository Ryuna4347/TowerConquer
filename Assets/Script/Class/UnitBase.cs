using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*공격/수비 유닛의 공통적인 부분
*/
public class UnitBase : MonoBehaviour { //리스트의 경우 수비때 유닛 업그레이드 때문이며, 공격은 1개만 사용한다. xml로 저장하지 않고 에디터에서 편집하기 위함
    public string unitName;
    public int lev_now;
    public List<Sprite> unitImgList;
    public List<int> healthList;
    public List<float> searchRangeList;
    public GameObject targetUnit; //공격할 대상(공격대상 설정은 유닛 오브젝트 하위에 있는 범위 표시 오브젝트에서 설정하게)
    public List<GameObject> enemyInRange; //사정거리 내의 적유닛 리스트
    public GameObject rangeImg; //유닛 하위에 있는 공격범위 스프라이트 오브젝트
    //public BulletInfo bullet;  유닛이 갖는 총알의 정의(수비의 경우 공격력, 공격의 경우 일부가 소지하며 특수능력 발동)

    private void Awake()
    {
        unitImgList = new List<Sprite>();
        healthList = new List<int>();
        searchRangeList = new List<float>();
        enemyInRange = new List<GameObject>();
    }

    public void SetUnitPosition(Vector2 unitPos)
    {
        gameObject.transform.position = unitPos;
    }
    public virtual void ResetUnit()
    {

    }
    public void SetTargetUnit(GameObject target)
    { //유닛의 공격목표 설정
        targetUnit = target;
    }

    private void OnEnable()
    {
        lev_now = 1;
        SetUnitRangeImage();
    }

    private void SetUnitRangeImage()
    {
        Debug.Log(gameObject.name);
        gameObject.transform.Find("SearchRange").GetComponent<SearchRange>().SetRange(searchRangeList[lev_now - 1]); //레벨에 맞는 범위만큼으로 이미지를 조절(lev_now-1인 이유는 레벨이 1부터 시작하기 때문에)
    }

    public void AddTarget(GameObject target)
    {
        if (!enemyInRange.Contains(target))
        { //적이 이미 리스트에 들어가 있으면 안됨
            enemyInRange.Add(target);
            RefreshTargetUnit(); //targetUnit을 갱신
        }
    }
    public void RemoveTarget(GameObject target)
    {
        if (!enemyInRange.Contains(target))
        { //적이 이미 리스트에 들어가 있으면 안됨
            enemyInRange.Remove(target);
            RefreshTargetUnit(); //targetUnit을 갱신
        }
    }

    //공격대상을 갱신할때 사용(수정)
    private void RefreshTargetUnit()
    {
        float dist = float.MaxValue;
        
        foreach(GameObject target in enemyInRange)
        {
            float temp_dist = Mathf.Pow(gameObject.transform.localPosition.x - target.transform.localPosition.x, 2) + Mathf.Pow(gameObject.transform.localPosition.y - target.transform.localPosition.y, 2);
            if (dist > temp_dist)
            {
                dist = temp_dist;
                targetUnit = target;
            }
        }
    }
    
}
