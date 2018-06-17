using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInstallUI : MonoBehaviour
{
    public GameObject unitInfoUI;
    private GameObject preloadImage;
    UnitManager unitManager; //설치 및 취소 버튼에서 요청한 이벤트를 여기에서 등록하기 위함

    // Use this for initialization
    void Awake()
    {
        GameObject.Find("Main Camera").GetComponent<CameraControl>().RegisterInstallUI(gameObject);
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
        preloadImage = GameObject.Find("PreLoadImage");
        gameObject.SetActive(false); //처음에 active가 true인 이유 : 메인카메라에 등록을 하려면 Awake가 돌아가야해서 false상태로는 카메라에 등록이 되지 않는다
        EventManager.StartListening("UnitSelected",UnitSelected);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("UnitSelected",UnitSelected);
    }

    private void OnDisable()
    {
        unitInfoUI.SetActive(false);
        preloadImage.SetActive(false);
    }

    public void UnitBtnPressed(string command)
    {
        if (command == "Ins")
        {//설치버튼
            unitManager.InstallUnit();
        }
        else if (command == "cancel")
        {//설치 취소버튼
            unitManager.CancelInstallProcess();
        }
        gameObject.SetActive(false); //UI도 닫아버리기
    }

    /*
     * UnitInstallUI 하위에 있는 유닛버튼을 누를경우 발생하는 함수
     */
    public void UnitSelected(GameObject obj, string command)
    {
        unitInfoUI.SetActive(true);//설치/취소 버튼의 부모여서 이게 켜져야 한다.(따로 관리하는 이유는 차후에 업그레이드나 삭제버튼 UI도 이거 하위로 둘것이기 때문)
        unitInfoUI.transform.localPosition = unitManager.GetSelectMousePos();
        unitManager.SelectUnit(command);
        PreLoadUnitImage(command);
    }

    /*
     * 유닛 사전 설치 이미지 관련 함수
     */
    public void PreLoadUnitImage(string command)
    {
        preloadImage.SetActive(true);
        Sprite unitImage = unitManager.GetUnitImage(command);
        preloadImage.GetComponent<Image>().sprite = unitImage;
        Vector2 clickPos= unitManager.GetSelectMousePos();
        clickPos.y -= gameObject.transform.localPosition.y; //installUI의 y축 값을 더하는 이유 : 얘가 부모라서 사전이미지의 localPosition을 설정하면 부모의 위치에 따라 바뀌기 때문에 부모의 위치로 인해 바뀌는 정도를 보정해주어야한다.
        preloadImage.transform.localPosition = clickPos;
    }
}
