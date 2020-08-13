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
    CameraModes _myCurrMode = CameraModes.NONE;
    public CameraModes GetCameraMode { get { return _myCurrMode; } }
    GameObject _objOfInterest;

    Vector3 _spaceDiff, _startPos, _targetPos;

    float _moveSpeed = .3f, _freeCamMoveSpeed = 7.5f;
    float _zoomSpeed = .3f, _zoomDistance = 6f;
    float _showcaseSpeed = 25f, _rotateSpeed = 50f;
    float _startTime, _currTime;

    bool _zoomed, _moving;
    public bool AmZoomed { get { return _zoomed; } }

    BattleUI _uiRef;

    //Initialize
    //focuses on middle of board
    //gets base distance needed
    public void Init(BattleUI reference, GameObject firstOOI)
    {
        _uiRef = reference;
        _spaceDiff = firstOOI.transform.position - transform.position;
        _moving = false;
        _zoomed = false;
    }

    //called when the camera needs to move
    public void FocusObject(GameObject POI, CameraModes howToLook)
    {
        GameUpdate.Subscribe -= PlayerControl;
        _objOfInterest = POI;
        _startPos = transform.position;
        //Debug.Log(POI.name);
        switch (howToLook)
        {
            //if the player left clicks on a character
            //move to that character as the focus
            case CameraModes.MOVING:
                _targetPos = _objOfInterest.transform.position - _spaceDiff;
                _zoomed = false;
                GameUpdate.Subscribe += MoveToPosition;
                break;
            //if the player right clicks on a character
            //move to that character as the focus
            //zoom on characters
            case CameraModes.ZOOMING:
                _targetPos = (_objOfInterest.transform.position - _spaceDiff) + (transform.forward * _zoomDistance);
                _zoomed = true;
                GameUpdate.Subscribe += ZoomOn;
                break;
            case CameraModes.SHOWCASING:
                GameUpdate.Subscribe += Showcase;
                break;
            default:
                break;

        }
        _myCurrMode = howToLook;
        _startTime = Time.time;
        
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

    void CheckBattleState()
    {
        if(_uiRef.GetInsteractionState == UIInteractions.FREE)
        {
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

    //moves 
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
            _spaceDiff = _objOfInterest.transform.position - transform.position;
            GameUpdate.Subscribe -= MoveToPosition;
            GameUpdate.Subscribe += PlayerControl;

            //Debug.Log("Done Moving");
        }
        //Debug.Log("moving");
        transform.position = RandomThings.Interpolate(_currTime, _startPos, _targetPos);
    }

    //called when the cmaera needs to zoom in to show detail on a character
    void ZoomOn()
    {
        _currTime = (Time.time - _startTime) / _zoomSpeed;

        if (_currTime > 1)
        {
            _currTime = 1;
            _myCurrMode = CameraModes.NONE;
            GameUpdate.Subscribe -= ZoomOn;
        }
        //Debug.Log("zooming");

        transform.position = RandomThings.Interpolate(_currTime, _startPos, _targetPos);
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
        RemoveSubscriptions();
        if(_zoomed)
        {
            FocusObject(_objOfInterest, CameraModes.MOVING);
        }
        else
        {
            _myCurrMode = CameraModes.FREE;
            GameUpdate.Subscribe += PlayerControl;
        }
    }

    //Removes all would-be camera movements from update
    void RemoveSubscriptions()
    {
        GameUpdate.Subscribe -= PlayerControl;
        GameUpdate.Subscribe -= MoveToPosition;
        GameUpdate.Subscribe -= ZoomOn;
        GameUpdate.Subscribe -= Showcase;
    }
}
