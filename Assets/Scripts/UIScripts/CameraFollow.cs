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
    GameObject _objOfInterest;

    Vector3 _spaceDiff, _targetPos;

    float _moveSpeed = 4f;
    float _zoomSpeed = 2f, _zoomDistance = 6f;
    float _showcaseSpeed = 25f, _rotateSpeed = 50f;
    float _startTime, _currTime;

    bool _readyToZoom;

    public void Init(GameObject firstOOI)
    {
        _spaceDiff = firstOOI.transform.position - transform.position;
    }

    private void Update()
    {
        switch (_myCurrMode)
        {
            case CameraModes.NONE:
                break;
            case CameraModes.MOVING:
                MoveToPosition();
                break;
            case CameraModes.ZOOMING:
                ZoomOn();
                break;
            case CameraModes.SHOWCASING:
                Showcase();
                break;
            case CameraModes.FREE:
                PlayerControl();
                break;
            default:
                break;
        }
    }

    public void FocusObject(GameObject POI, CameraModes howToLook)
    {
        _objOfInterest = POI;
        Debug.Log(POI.name);
        switch (howToLook)
        {
            case CameraModes.MOVING:
                _targetPos = _objOfInterest.transform.position - _spaceDiff;
                break;
            case CameraModes.ZOOMING:
                _targetPos = (_objOfInterest.transform.position - _spaceDiff) + (transform.forward * _zoomDistance);
                break;
            case CameraModes.SHOWCASING:
                break;
            default:
                break;

        }
        _myCurrMode = howToLook;
        _startTime = Time.time;
        
    }

    void PlayerControl()
    {
        CheckForCameraMovement();
        CheckForCameraRotate();
    }

    void CheckForCameraMovement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            MoveCamera(transform.forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveCamera(-transform.forward);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveCamera(-transform.right);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveCamera(transform.right);
        }
    }

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


    void MoveCamera(Vector3 direction)
    {
        direction.y = 0f;
        transform.position += direction * Time.deltaTime * _moveSpeed;
    }

    void MoveToPosition()
    {
        _currTime = (Time.time - _startTime) / _moveSpeed;

        if (_currTime > 1)
        {
            _currTime = 1;
            _myCurrMode = CameraModes.FREE;
            _spaceDiff = _objOfInterest.transform.position - transform.position;
        }

        transform.position = RandomThings.Interpolate(_currTime, transform.position, _targetPos);
    }


    void ZoomOn()
    {
        _currTime = (Time.time - _startTime) / _zoomSpeed;

        if (_currTime > 1)
        {
            _currTime = 1;
            _myCurrMode = CameraModes.NONE;
        }

        transform.position = RandomThings.Interpolate(_currTime, transform.position, _targetPos);
    }

    void Showcase()
    {
        RotateShowcase(Vector3.up, _showcaseSpeed);
    }

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

    public void ResetCamera()
    {
        FocusObject(_objOfInterest, CameraModes.MOVING);
    }
}
