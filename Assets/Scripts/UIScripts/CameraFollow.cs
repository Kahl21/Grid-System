using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public enum CameraModes
{
    NONE,
    FREE,
    MOVING,
    ZOOMING,
    SHOWCASING
}

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    CameraModes _myCurrMode = CameraModes.NONE;
    public CameraModes GetCameraMode { get { return _myCurrMode; } set { _myCurrMode = value; } }

    Camera _camComponent;
    RenderTexture _textureforgridcam;

    Vector3 _objOfInterest;

    Vector3 _spaceDiff, _startPos, _targetPos;

    [SerializeField]
    float _moveSpeed = .3f, _freeCamMoveSpeed = 7.5f;
    [SerializeField]
    float _zoomSpeed = .3f, _zoomDistance = 6f;
    [SerializeField]
    float _showcaseSpeed = 25f, _rotateSpeed = 50f;
    float _startTime, _currTime;

    bool _zoomed, _moving, _initialized;
    public bool AmZoomed { get { return _zoomed; } }
    public bool IsMoving { get { return _moving; } }

    BattleUI _uiRef;

    //Initialize
    //focuses on middle of board
    //gets base distance needed
    public void Init(GameObject firstOOI)
    {
        if (!_initialized)
        {
            _camComponent = GetComponent<Camera>();
            _uiRef = UIHolder.UIInstance.GetBattleUI;
            _spaceDiff = firstOOI.transform.position - transform.position;
            _moving = false;
            _zoomed = false;
            _initialized = true;
        }
        else
        {
            _camComponent.targetTexture = _textureforgridcam;
        }
    }

    public void ChangeToBattleMode()
    {
        _textureforgridcam = _camComponent.targetTexture;
        _camComponent.targetTexture = null;
        _camComponent.enabled = true;
    }

    //called when the camera needs to move
    public void FocusPosition(GameObject POI, CameraModes howToLook)
    {
        FocusPosition(POI.transform.position, howToLook);
    }

    public void FocusPosition(Vector3 POI, CameraModes howToLook)
    {
        if (!_moving)
        {
            GameUpdate.PlayerSubscribe -= PlayerControl;
            _objOfInterest = POI;
            _startPos = transform.position;
            //Debug.Log(POI.name);
            switch (howToLook)
            {
                //if the player left clicks on a character
                //move to that character as the focus
                case CameraModes.MOVING:
                    _targetPos = _objOfInterest - _spaceDiff;
                    _zoomed = false;
                    GameUpdate.UISubscribe += MoveToPosition;
                    break;
                //if the player right clicks on a character
                //move to that character as the focus
                //zoom on characters
                case CameraModes.ZOOMING:
                    _targetPos = (_objOfInterest - _spaceDiff) + (transform.forward * _zoomDistance);
                    _zoomed = true;
                    GameUpdate.UISubscribe += ZoomOn;
                    break;
                //called when the player tries to rotate the camera around the battlefield
                //used in the GridSetup to rotate the grid preview
                case CameraModes.SHOWCASING:
                    GameUpdate.UISubscribe += Showcase;
                    break;
                default:
                    break;

            }
            _moving = true;
            _myCurrMode = howToLook;
            _startTime = Time.time;
        }
    }
    public void CameraFullStop()
    {
        _myCurrMode = CameraModes.NONE;
        GameUpdate.UISubscribe -= PlayerControl;
    }

    //called for player manual movement
    void PlayerControl()
    {
        CheckForCameraMovement();
        CheckForCameraRotate();
    }

    //move camera forward, backward, left, right 
    //depending on keys pressed on
    void CheckForCameraMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveCamera(transform.forward);
            CheckBattleState();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveCamera(-transform.forward);
            CheckBattleState();
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveCamera(-transform.right);
            CheckBattleState();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveCamera(transform.right);
            CheckBattleState();
        }
    }

    //called when the UIState changes
    //checks if menus need to be hidden or not
    void CheckBattleState()
    {
        if(_uiRef.GetInteractionState == UIInteractions.MENUOPEN)
        {
            _uiRef.GetInteractionState = UIInteractions.FREE;
            _uiRef.HideActionsMenu();
        }
    }

    //rotate camera around the focus left or right
    //left or right depending on the buttons pressed
    void CheckForCameraRotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Showcase(false);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Showcase(true);
        }
    }

    //math to move camera left right forward backward
    void MoveCamera(Vector3 direction)
    {
        direction.y = 0f;
        transform.position += direction * Time.deltaTime * _freeCamMoveSpeed;
    }

    //called when the camers needs to focus on a certain character
    void MoveToPosition()
    {
        _currTime = (Time.time - _startTime) / _moveSpeed;

        if (_currTime > 1)
        {
            _currTime = 1;
            _myCurrMode = CameraModes.FREE;
            GameUpdate.UISubscribe -= MoveToPosition;
            GameUpdate.PlayerSubscribe += PlayerControl;
            _moving = false;
            //Debug.Log("Done Moving");
        }
        //Debug.Log("moving");
        transform.position = RandomThings.Interpolate(_currTime, _startPos, _targetPos);
    }

    //called when the cmaera needs to zoom in to show detail on a character
    void ZoomOn()
    {
        _currTime = (Time.time - _startTime) / _zoomSpeed;

        
        //Debug.Log("zooming");

        transform.position = RandomThings.Interpolate(_currTime, _startPos, _targetPos); 
        
        if (_currTime > 1)
        {
            transform.position = _targetPos;
            _currTime = 1;
            _myCurrMode = CameraModes.NONE;
            GameUpdate.UISubscribe -= ZoomOn;
            _moving = false;
        }
    }

    //rotates the camera
    //used for GridSetupUI grid slow spin
    void Showcase()
    {
        RotateShowcase(Vector3.up, _showcaseSpeed);
    }

    //rotates the camera
    //used in battle for player camera control
    public void Showcase(bool spinLeft)
    {
        if (spinLeft)
        {
            RotateShowcase(Vector3.down, _rotateSpeed);
        }
        else
        {
            RotateShowcase(Vector3.up, _rotateSpeed);
        }
    }

    //calculates the math for rotating the camera
    //camera pivot and looks at an empty point
    void RotateShowcase(Vector3 pivotdir, float speed)
    {
        Vector3 angles = Quaternion.Euler(0, 60, 0) * (pivotdir * Time.deltaTime * speed);


        Vector3 pivot = transform.position + _spaceDiff;

        transform.LookAt(pivot);
        Vector3 point = transform.position;
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        transform.position = point; // return it
        _spaceDiff = pivot - transform.position;

    }

    //resets camera to allow for plater interaction
    //unless zoomed in
    public void ResetCamera()
    {
        if (!_moving)
        {
            HardResetCamera();
        }
    }

    //called when camera needs to be sent to base functionality
    //called when turns end
    public void HardResetCamera()
    {
        _moving = false;
        RemoveSubscriptions();
        if (_zoomed)
        {
            FocusPosition(_objOfInterest, CameraModes.MOVING);
        }
        else
        {
            _myCurrMode = CameraModes.FREE;
            GameUpdate.PlayerSubscribe += PlayerControl;
        }
    }

    //Removes all would-be camera movements from update
    void RemoveSubscriptions()
    {
        GameUpdate.PlayerSubscribe -= PlayerControl;
        GameUpdate.UISubscribe -= MoveToPosition;
        GameUpdate.UISubscribe -= ZoomOn;
        GameUpdate.UISubscribe -= Showcase;
    }
}
