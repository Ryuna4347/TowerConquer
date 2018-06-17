using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectBtn : MonoBehaviour {

    public string unitName;

    public void InsButtonPressed()
    { //버튼 클릭시 UnitInstallUI에서 한번에 이벤트를 모아서 실행시키는 걸로(버튼마다 UnitManager를 읽어들이기에는 자원이 좀)
        EventManager.TriggerEvent("UnitSelected", gameObject, unitName);
    }

}
