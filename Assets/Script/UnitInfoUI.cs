using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour {
    private void Awake()
    {//유닛 매니저에 등록하고 꺼진상태로 전환
        GameObject.Find("UnitManager").GetComponent<UnitManager>().SetInstallBtn(gameObject.transform.Find("UnitInstallBtn").gameObject);
    }
}
