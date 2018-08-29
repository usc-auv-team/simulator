using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SplitScreenController : MonoBehaviour {

	// reference of our AUV
	public GameObject AUV;

	// the cameras set in the editor
	public Camera MainCamera;
	public Camera FollowCamera;
	public Camera TopCamera;
	public Camera BackCamera;
	public Camera LeftCamera;

	// The dropdown to control how the screen is splited
	public Dropdown modeSelection;
	// The dropdown to select the target view to change (at most 4)
	public Dropdown viewSelection;
	// The dropdown to set the camera for the current selected view
	public Dropdown cameraSelection;

	float translationSpeed = 0.1f;
	float rotationSpeed = 0.1f;
	float zoomSpeed = 0.5f;

	bool isMouseOnUI;

	enum CameraType {
		MAIN,
		FOLLOW,
		TOP,
		BACK,
		LEFT,
	}

	// The actrual camera used to project stuff
	Camera[] ViewPoints;
	CameraType[] ViewTypes;

	// the current sellected camera and it's type
	Camera CurrentCamera;
	CameraType CurrentCameraType;

	// preset view modes
	static readonly Rect RECT_EMPTY = new Rect(0,0,0,0);
	static readonly Rect RECT_LEFT = new Rect(0,0,0.5f,1);
	static readonly Rect RECT_RIGHT = new Rect(0.5f,0,0.5f,1);
	static readonly Rect RECT_TOP = new Rect(0,0.5f,1,0.5f);
	static readonly Rect RECT_BOTTOM = new Rect(0,0,1,0.5f);
	static readonly Rect RECT_TOP_LEFT = new Rect(0,0.5f,0.5f,0.5f);
	static readonly Rect RECT_TOP_RIGHT = new Rect(0.5f,0.5f,0.5f,0.5f);
	static readonly Rect RECT_BOTTOM_LEFT = new Rect(0,0,0.5f,0.5f);
	static readonly Rect RECT_BOTTOM_RIGHT = new Rect(0.5f,0,0.5f,0.5f);
	static readonly Rect RECT_FULL = new Rect(0,0,1,1);

	// used for drag and drop camera control opperation
	Vector3 mouseOrigin;

	// Use this for initialization
	void Start () {

		isMouseOnUI = false;

		// we don't use these cameras to project the view for several reasons
		// e.g. we might sometimes need several subviews using the main camera with different
		// view points and if we directly use the origin MainCamera instance change of one view
		// will reflect on all the others.
		MainCamera.rect = RECT_EMPTY;
		FollowCamera.rect = RECT_EMPTY;
		TopCamera.rect = RECT_EMPTY;
		BackCamera.rect = RECT_EMPTY;
		LeftCamera.rect = RECT_EMPTY;


		ViewPoints = new Camera[4];
		ViewTypes = new CameraType[4];

		ViewPoints [0] = (Camera)Camera.Instantiate (MainCamera);
		ViewTypes [0] = CameraType.MAIN;
		ViewPoints [1] = (Camera)Camera.Instantiate (TopCamera);
		ViewTypes [1] = CameraType.TOP;
		ViewPoints [2] = (Camera)Camera.Instantiate (BackCamera);
		ViewTypes [2] = CameraType.BACK;
		ViewPoints [3] = (Camera)Camera.Instantiate (LeftCamera);
		ViewTypes [3] = CameraType.LEFT;

		ViewPoints [0].rect = RECT_FULL;

		CurrentCamera = ViewPoints [0];
		CurrentCameraType = ViewTypes [0];

	}
	
	// Update is called once per frame
	void Update () {

		// Zoom in and out
		float _scroll = Input.GetAxis ("Mouse ScrollWheel");
		if (CurrentCameraType != CameraType.FOLLOW) {
			CurrentCamera.transform.Translate (0, 0, _scroll * zoomSpeed, Space.Self); 
			// TODO: for follow mode, I'm considering adding another way to zoom in and out: changing the fieldOfView
		}

		if (Input.GetMouseButtonDown (0)) {
			if (EventSystem.current.IsPointerOverGameObject ()) {
				isMouseOnUI = true;
			} else {
				isMouseOnUI = false;
			}
		}

		if (!isMouseOnUI) {
			if (Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) {
				// record the current mouse position as our base point
				mouseOrigin = Input.mousePosition;
				int _index = 0;
				// which view are we opperating
				switch (modeSelection.value) {
				case 0:
					// single view
					break;
				case 1:
					// split horizontally
					_index = Input.mousePosition.x<Screen.width/2 ? 0 : 1;
					break;
				case 2:
					// split vertically
					_index = Input.mousePosition.y<Screen.height/2 ? 1 : 0;
					break;
				case 3:
					// split into 4 subviews
					_index = Input.mousePosition.x < Screen.width / 2 ? 0 : 1;
					_index = Input.mousePosition.y < Screen.height / 2 ? _index + 2 : _index;
					break;
				}
				CurrentCamera = ViewPoints [_index];
				CurrentCameraType = ViewTypes [_index];
			}
			// Rotation
			if (Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
				// to avoid choas, only work when there's exactly one mouse key pressed down
				// other views should never rotate
				if (CurrentCameraType != CameraType.MAIN) 
					return;
				float _deltaX = Input.mousePosition.x - mouseOrigin.x;
				float _deltaY = Input.mousePosition.y - mouseOrigin.y;
				Vector3 _temptOrientation = CurrentCamera.transform.eulerAngles;
				_temptOrientation.x -= _deltaY * rotationSpeed;
				_temptOrientation.y += _deltaX * rotationSpeed;
				CurrentCamera.transform.eulerAngles = _temptOrientation;
				mouseOrigin = Input.mousePosition;
			}
			// Translation
			if (Input.GetMouseButton(1) && !Input.GetMouseButton(0)) {
				// basically the same as rotation, note that the follow view should never move around
				if (CurrentCameraType == CameraType.FOLLOW)
					return;
				float _deltaX = (Input.mousePosition.x - mouseOrigin.x) * translationSpeed;
				float _deltaY = (Input.mousePosition.y - mouseOrigin.y) * translationSpeed;
				CurrentCamera.transform.Translate (_deltaX, _deltaY, 0, Space.Self);
				mouseOrigin = Input.mousePosition;
			}
		}

		// Update the actrual projecting cameras' parameters
		for (int i = 0; i < 4; i++) {
			Rect _temptRC = ViewPoints [i].rect;
			if (ViewTypes [i] == CameraType.FOLLOW) {
				ViewPoints [i].CopyFrom (FollowCamera);
			}
			ViewPoints [i].rect = _temptRC;
		}
	}

	public void SetMode() {
		// desides how to use the screen
		for (int i = 0; i < 4; i++) {
			ViewPoints [i].rect = RECT_EMPTY;
		}

		switch (modeSelection.value) {
		case 0:
			// single view
			ViewPoints[0].rect = RECT_FULL;
			break;
		case 1:
			// split horizontally
			ViewPoints[0].rect = RECT_LEFT;
			ViewPoints[1].rect = RECT_RIGHT;
			break;
		case 2:
			// split vertically
			ViewPoints[0].rect = RECT_TOP;
			ViewPoints[1].rect = RECT_BOTTOM;
			break;
		case 3:
			// split into 4 subviews
			ViewPoints[0].rect = RECT_TOP_LEFT;
			ViewPoints[1].rect = RECT_TOP_RIGHT;
			ViewPoints[2].rect = RECT_BOTTOM_LEFT;
			ViewPoints[3].rect = RECT_BOTTOM_RIGHT;
			break;
		}
	}

	public void SetCamera () {
		// decide which camera is responsible to render the selected view
		int _viewIndex = viewSelection.value;
		Rect _tempRC = ViewPoints[_viewIndex].rect;
		ViewPoints [_viewIndex].rect = RECT_EMPTY;

		switch (cameraSelection.value) {
		case 0:
			if (ViewTypes [_viewIndex] != CameraType.MAIN) {
				ViewPoints [_viewIndex].CopyFrom (MainCamera);
				ViewTypes [_viewIndex] = CameraType.MAIN;
			}
			break;
		case 1:
			if (ViewTypes [_viewIndex] != CameraType.FOLLOW) {
				ViewPoints [_viewIndex].CopyFrom (FollowCamera);
				ViewTypes [_viewIndex] = CameraType.FOLLOW;
			}
			break;
		case 2:
			if (ViewTypes [_viewIndex] != CameraType.TOP) {
				ViewPoints [_viewIndex].CopyFrom (TopCamera);
				ViewTypes [_viewIndex] = CameraType.TOP;
			}
			break;
		case 3:
			if (ViewTypes [_viewIndex] != CameraType.BACK) {
				ViewPoints [_viewIndex].CopyFrom (BackCamera);
				ViewTypes [_viewIndex] = CameraType.BACK;
			}
			break;
		case 4:
			if (ViewTypes [_viewIndex] != CameraType.LEFT) {
				ViewPoints [_viewIndex].CopyFrom (LeftCamera);
				ViewTypes [_viewIndex] = CameraType.LEFT;
			}
			break;
		}
		ViewPoints[_viewIndex].rect = _tempRC;
		CurrentCamera = ViewPoints [_viewIndex];
		CurrentCameraType = ViewTypes [_viewIndex];
	}

	public void SetView () {
		// this is just to update the camera selection dropdown to match the current view.
		cameraSelection.value = (int)ViewTypes [viewSelection.value];
	}
}
