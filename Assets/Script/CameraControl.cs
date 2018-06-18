using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Camera moving and autoscaling.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraControl : MonoBehaviour
{
	/// <summary>
	/// Control type for camera autoscaling.
	/// </summary>
    public enum ControlType
    {
        ConstantWidth,	// Camera will keep constant width
		ConstantHeight	// Camera will keep constant height
    }

	// Camera control type
    public ControlType controlType;
	// Camera will autoscale to fit this object
	public SpriteRenderer focusObjectRenderer;
	// Horizontal offset from focus object edges
	public float offsetX = 0f;
	// Vertical offset from focus object edges
	public float offsetY = 0f;
	// Camera speed when moving (dragging)
    public float dragSpeed = 2f;

	// Restrictive points for camera moving
	private float maxX, minX, maxY, minY;
	// Camera dragging at now vector
    private float moveX, moveY;
    private float camMovSpeed=0.3f; //카메라를 움직이는 속도
    private float originX, originY;
    private float moveCT=1.0f; //required clicking time to move camera
    private float clickingTime;
    public bool isCameraMove;
	// Camera component from this gameobject
	private Camera cam;
	// Origin camera aspect ratio
	private float originAspect;

    public GameObject mapImage;
    public GameObject unitCreateUI; //유닛 생성을 위한 버튼(생성/취소) 에디터에서 추가하기

    public Transform[] boundaries;
    private float cameraWidth; //카메라 너비와 높이(맵 밖으로 나가지 않게 하기 위해서이므로 절반만)
    private float cameraHeight;
    private float mapWidth; //실제 맵의 크기
    private float mapHeight;

    private float tileWidth=1f;
    private float tileHeight=1f;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Awake()
	{
        Debug.Assert (cam&&mapImage, "Wrong initial settings");
        
        SetBoundaries(); //로드된 이미지에 맞게 카메라 이동범위 설정
        clickingTime = 0;

		originAspect = cam.aspect;        
    }

    /// <summary>
    /// Lates update this instance.
    /// </summary>
    void LateUpdate()
    {
        // Camera aspect ratio is changed
        if (originAspect != cam.aspect)
        {
            originAspect = cam.aspect;
        }
        if (Input.GetMouseButton(0))
        {
            if (clickingTime < moveCT)
            {
                clickingTime += Time.deltaTime;
                if (clickingTime > moveCT)
                {
                    isCameraMove = true;
                    Vector3 temp = gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                    originX = temp.x;
                    originY = temp.y;
                }
            }
            if (isCameraMove)
            {
                Vector3 movMouse = gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                moveX = movMouse.x - originX;
                moveY = movMouse.y - originY;

                Vector2 movNormalize = new Vector2(moveX, moveY).normalized;
                

                bool check=CheckBoundaries(movNormalize);
                if (check)
                { //맵 밖으로는 이동이 불가
                    gameObject.transform.Translate(movNormalize.x*camMovSpeed, movNormalize.y * camMovSpeed, 0); //드래그 방향으로 일정속도만큼 움직임
                    originX = movMouse.x;
                    originY = movMouse.y;
                }
                else
                {
                    //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (clickingTime < moveCT)
            {//일반적인 클릭일 경우
                Vector3 movMouse = gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                Vector3 position = new Vector3(Mathf.Floor(movMouse.x) + tileWidth/2, Mathf.Floor(movMouse.y)+tileHeight/2, -10f);

                if (EventSystem.current.IsPointerOverGameObject() == false)
                {  //UI이 위가 아니면(이건 수비유닛용이니까 공격유닛용 설치는 다른식으로 만들어야됨)
                    //int posForTileX = (int)Mathf.Floor(Mathf.Floor(position.x)+mapWidth); //카메라너비에서 맵너비로 바꿈(카메라가 맵 전체를 보지 않아서 스크린좌표로 바꾸면 카메라 너머에 맵이 더 있는데 카메라 좌하단이 0,0이 되버린다.)
                    //int posForTileY = (int)Mathf.Floor(Mathf.Floor(position.y)+mapHeight);

                    Vector2 tilePos = new Vector2(Mathf.Floor((position.x- tileWidth / 2) /tileWidth),Mathf.Floor((position.y- tileHeight / 2) /tileHeight)); //mousePos는 타일의 중앙을 가리키도록 0.5씩 더했기 때문에 내림을 하고 중앙이 (0,0)이기 때문에 카메라 사이즈 만큼 더해주어야한다.
                    bool canBuildDefUnit = GameObject.Find("MapRoadManager").GetComponent<MapRoadManager>().CheckRoadData(tilePos); //클릭한 곳이 설치가 가능한 곳인지 검사
                    if (canBuildDefUnit)
                    {//지을 수 있다면
                        //유닛매니저에 위치저장
                        GameObject.Find("UnitManager").GetComponent<UnitManager>().SetPosition(position);
                        if (unitCreateUI.activeSelf == false)
                        { //유닛 설치 관련 UI가 꺼진 상태여야함.
                          //설치 관련 UI(유닛 위치 미리 보여주기, 설치 및 취소버튼 생성)
                            OpenCreateUnitUI(movMouse);
                        }
                        else
                        {//켜진상태였다면 꺼버려
                            CloseCreateUnitUI();
                        }
                    }
                    else if(unitCreateUI.activeSelf == true)
                    { //길을 누른 상태인데 설치관련 UI가 켜져있는 상태일 경우 UI를 안보이게
                        CloseCreateUnitUI();
                    }
                }
            }
            isCameraMove = false;
            clickingTime = 0;
            moveX = moveY = originX = originY = 0;
        }
    }

    public void SetFocusObject()
    {
        // Get restrictive points from focus object's corners
        maxX = focusObjectRenderer.bounds.max.x;
        minX = focusObjectRenderer.bounds.min.x;
        maxY = focusObjectRenderer.bounds.max.y;
        minY = focusObjectRenderer.bounds.min.y;
        UpdateCameraSize();
    }
    

	/// <summary>
	/// Updates the size of the camera to fit focus object.
	/// </summary>
	private void UpdateCameraSize()
	{
        if (cam == null)
        { //cam이 사용되는 시기가 CameraControl.cs가 Awake되기 전인듯해서 Awake에 쓰면 오류뜬다
            cam = gameObject.GetComponent<Camera>();
        }
		switch (controlType)
		{
		case ControlType.ConstantWidth:
                cam.orthographicSize = (maxX - minX - 2 * offsetX) / (2.5f*cam.aspect);
			break;
		case ControlType.ConstantHeight:
			cam.orthographicSize = (maxY - minY - 2 * offsetY)/2.5f;
			break;
		}
        cameraHeight= Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    public void LoadMapImage(int lev)
    {
        Sprite image = Resources.Load<Sprite>("Level" + lev);
        if (image)
        {
            mapImage.GetComponent<SpriteRenderer>().sprite = image;
            focusObjectRenderer = mapImage.GetComponent<SpriteRenderer>();
            SetFocusObject(); //focusObject가 카메라 Awake보다 늦게 불러져서 Awake에 사용불가하다. 따라서 호출순서를 맵을 불러오고나서로 변경
            mapWidth = (mapImage.GetComponent<SpriteRenderer>().size.x/2)/tileWidth; //1.3, 1.29인 이유는 MapEditor신에서 만들때 각 타일 크기를 이렇게 맞춰놔서 실제 타일수보다 사이즈가 크게나온다.
            mapHeight = (mapImage.GetComponent<SpriteRenderer>().size.y/2)/tileHeight;
        }
        else {
            Debug.Log("이미지 없음");
        }
    }

    private void SetBoundaries()
    {
        float halfWidth = mapImage.GetComponent<SpriteRenderer>().bounds.extents.x;
        float halfHeight = mapImage.GetComponent<SpriteRenderer>().bounds.extents.y;
        int[] arr ={1, 0, 1, 0}; 

        for (int i=0; i<4; i++)
        { //카메라가 사각형이니 경계도 4개밖에 없음(동,북,서,남 순서대로 설정)
            boundaries[i].localPosition = new Vector3(arr[i]*Mathf.Pow(-1,i/2) * halfWidth,arr[3-i]*Mathf.Pow(-1, i/2) * halfHeight, 0);
        }
    }
    
    private bool CheckBoundaries(Vector2 movNormalized)
    {

        float movePosX = gameObject.transform.position.x+movNormalized.x*camMovSpeed; //카메라를 옮길 x,y위치
        float movePosY = gameObject.transform.position.y+movNormalized.y*camMovSpeed;
        

        if ((movePosX < boundaries[2].position.x+cameraWidth || movePosX > boundaries[0].position.x - cameraWidth) ||
            (movePosY < boundaries[3].position.y + cameraHeight || movePosY > boundaries[1].position.y - cameraHeight))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void OpenCreateUnitUI(Vector2 mousePos)
    {
        if (unitCreateUI.activeSelf == false)
        {
            unitCreateUI.SetActive(true);
        }
    }
    public void CloseCreateUnitUI()
    {
        if (unitCreateUI.activeSelf == true)
        {
            //unitCreateUI의 초기화
            unitCreateUI.SetActive(false);
        }
    }
    public void RegisterInstallUI(GameObject insUI)
    { //다른신이어서 에디터상으로 등록이 안됨
        unitCreateUI = insUI;
    }
    public Vector2 GetWorldPosition(Vector2 pos)
    {
        Vector2 temp = cam.WorldToScreenPoint(pos);
        temp.x -= cam.pixelWidth/2; //그냥 WorldToScreenPoint()를 사용하면 좌하단 좌표가 0,0인데, 이게임은 정가운데 좌표가 0,0이므로 화면의 반만큼을 좌표에서 빼주어야한다.
        temp.y -= cam.pixelHeight/2;
        return temp;
    }
}
