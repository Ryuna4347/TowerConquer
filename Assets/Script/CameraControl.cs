using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private float originX, originY;
    private float moveCT=1.0f; //required clicking time to move camera
    private float clickingTime;
    public bool isCameraMove;
	// Camera component from this gameobject
	private Camera cam;
	// Origin camera aspect ratio
	private float originAspect;

    public GameObject mapImage;

    public Transform[] boundaries;
    private float cameraWidth; //카메라 너비와 높이(맵 밖으로 나가지 않게 하기 위해서이므로 절반만)
    private float cameraHeight;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
	{
		cam = GetComponent<Camera>();
		Debug.Assert (cam&&mapImage, "Wrong initial settings");

        LoadMapImage(1); //LevelManager 코드 만들고 바꿔야됨
        SetBoundaries(); //로드된 이미지에 맞게 카메라 이동범위 설정
        clickingTime = 0;

		originAspect = cam.aspect;
		// Get restrictive points from focus object's corners
		maxX = focusObjectRenderer.bounds.max.x;
		minX = focusObjectRenderer.bounds.min.x;
		maxY = focusObjectRenderer.bounds.max.y;
		minY = focusObjectRenderer.bounds.min.y;
		UpdateCameraSize();

        Debug.Log("우좌제약구간 : " + (boundaries[2].position.x + cameraWidth) + " " + (boundaries[0].position.x - cameraWidth));
        Debug.Log("하상제약구간 : " + (boundaries[3].position.y + cameraHeight) + " " + (boundaries[1].position.y - cameraHeight));
        Debug.Log("카메라 크기 : " + cameraWidth + ", " + cameraHeight);
    }

    /// <summary>
    /// Lates update this instance.
    /// </summary>
    void LateUpdate()
    {
        // Camera aspect ratio is changed
        if (originAspect != cam.aspect)
        {
            UpdateCameraSize();
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
                bool check=CheckBoundaries(moveX, moveY);
                if (check)
                { //맵 밖으로는 이동이 불가
                    gameObject.transform.Translate(moveX, moveY, 0);
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
            isCameraMove = false;
            clickingTime = 0;
            moveX = moveY = originX = originY = 0;
        }
    }
    

	/// <summary>
	/// Updates the size of the camera to fit focus object.
	/// </summary>
	private void UpdateCameraSize()
	{
		switch (controlType)
		{
		case ControlType.ConstantWidth:
			cam.orthographicSize = (maxX - minX - 2 * offsetX) / (2f * cam.aspect);
			break;
		case ControlType.ConstantHeight:
			cam.orthographicSize = (maxY - minY - 2 * offsetY) / 2f;
			break;
		}
        cameraHeight= Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
    }

    private void LoadMapImage(int lev)
    {
        Sprite image = Resources.Load<Sprite>("Level" + lev);
        if (image)
        {
            mapImage.GetComponent<SpriteRenderer>().sprite = image;
            focusObjectRenderer = mapImage.GetComponent<SpriteRenderer>();
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
    
    private bool CheckBoundaries(float moveX, float moveY)
    {
        float movePosX = gameObject.transform.position.x+moveX; //카메라를 옮길 x,y위치
        float movePosY = gameObject.transform.position.y+moveY;
        

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
}
