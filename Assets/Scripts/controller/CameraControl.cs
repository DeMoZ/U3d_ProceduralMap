using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    //public Transform tempCube;
    Vector3 keyboardAxis = new Vector2(); // input from keyboard
                                          //
    public float lookSpeedHor = 10f;          // speed of camera rotate
    public float lookSpeedVer = 10f;
    public float lookSpeedSlerp = 10;      //speed of smoothing camera rotation

    public Vector3 cameraFollowOffset = new Vector3(1, 1.7f, -2);
    public Vector2 cameraFollowAngleClapm = new Vector2(-72, 85);

    public Vector3 cameraFreeOffset = new Vector3(1, 10, -2);
    public Vector2 cameraFreeAngleClapm = new Vector2(15, 85);

    public Vector3 camOffsetXZ = new Vector2(); // unpublic
    public Vector3 camOffsetXZSlerped = new Vector2();   //unpublic



    Vector2 camAngleClamp = new Vector2();
    Vector3 camAngle = new Vector2();
    Vector3 camAngleSlerped = new Vector2();

    Transform _transform;
    Vector3 targetPos = new Vector2();

    //***********
    Vector3 camForward = new Vector2(); // move relative to camera
    Vector3 camRight = new Vector2();   // move relative to camera
    Vector3 moveDir = new Vector2(); // move relative to camera


    public enumCameraState cameraState = enumCameraState.rest;
    public enum enumCameraState
    {
        rest,
        free,
        follow,
    }
    public bool mouseVertInvert = false;

    public BodyMove bodyMoveComponent;      // link to BodyMove script of controlled body


    void OnEnable()
    {
        StaticEvents.eventCameraMoveLook.AddListener(Free);
        StaticEvents.eventNewPlayerGameObject.AddListener(Follow);
    }

    void OnDisable()
    {
        StaticEvents.eventCameraMoveLook.RemoveListener(Free);
        StaticEvents.eventNewPlayerGameObject.RemoveListener(Follow);
    }
    void Start()
    {
        _transform = transform;

    }

    void Free(Vector3 moveP, Vector3 lookP)
    {
       // camAngleSlerped = _transform.eulerAngles;
        camOffsetXZ.x = cameraFreeOffset.x;
        camOffsetXZ.z = cameraFreeOffset.z;
        camAngleClamp = cameraFreeAngleClapm;
        cameraState = enumCameraState.free;

       // Debug.Log(_transform.eulerAngles);
    }
    void Follow(GameObject target)
    {
       // camAngleSlerped = _transform.eulerAngles;
        camOffsetXZ.x = cameraFollowOffset.x;
        camOffsetXZ.z = cameraFollowOffset.z;
        camAngleClamp = cameraFollowAngleClapm;
        cameraState = enumCameraState.follow;

      //  Debug.Log(_transform.eulerAngles);
    }
    //******************************************************************************************************************
    void DeprecatedFollow()
    {
        //targetPos = targetT.position;
        //mouseAxis.x += mouseDelta.x * lookSpeed;

        //mouseAxis.y += ClampAngle(mouseDelta.y * lookSpeed, _transform.eulerAngles, -85, 85);// minimumY, maximumY);
        //mouseAxisSlerp = Vector3.Slerp(mouseAxisSlerp, mouseAxis, Time.deltaTime * lookSpeedSlerp);

        //targetPosSlerped = Vector3.Slerp(targetPosSlerped, targetPos, Time.deltaTime * targPosSpeedSlerp);
        //targetPosSlerped = new Vector3(targetPos.x, targetPosSlerped.y, targetPos.z);

        //// //RotateVirtualCamera(mouseAxisSlerp, targetPosSlerped, _transform, cameraOffset);
        //Quaternion rotation = Quaternion.Euler(mouseAxisSlerp.y, mouseAxisSlerp.x, 0.0f);
        //_transform.rotation = rotation;
        //Vector3 wantPosition = targetPos + _transform.TransformVector(cameraOffset);
        //wantPosition = CameraCollisionCheck(wantPosition, targetPos);
        //_transform.position = wantPosition;


    }
    void FixedUpdate()
    {
        //keyboardAxis = ProjectIOSingletone.GetKeyWASD();
        keyboardAxis.x = ProjectIOSingletone.Get().InpKeybAxisH;
        keyboardAxis.z = ProjectIOSingletone.Get().InpKeybAxisV;

        switch (cameraState)
        {
            case enumCameraState.rest:
                Rest();
                break;
            case enumCameraState.follow:
                MoveBody();
                break;
            case enumCameraState.free:
                MoveCamera();
                break;
            default:
                break;
        }
    }

    void LateUpdate()
    {
        switch (cameraState)
        {
            case enumCameraState.follow:
                if (!ProjectIOSingletone.Get().ThePlayerB) {//cameraState 
                    StaticEvents.eventCameraMoveLook.Invoke(_transform.position, _transform.forward);
                    break; }
                targetPos = ProjectIOSingletone.Get().ThePlayerB._transform.position;
                targetPos.y += cameraFollowOffset.y;
                break;

            case enumCameraState.free:
                targetPos.y = cameraFreeOffset.y;
                break;
            default:
                break;
        }
        UniversalCam();
    }

    void UniversalCam()
    {
        camAngle.x += ProjectIOSingletone.Get().InpMouseAxisV * lookSpeedVer * ((mouseVertInvert) ? 1 : -1);
        camAngle.x = Mathf.Clamp(camAngle.x, camAngleClamp.x, camAngleClamp.y);

        camAngle.y += ProjectIOSingletone.Get().InpMouseAxisH * lookSpeedHor;

        camOffsetXZ.z += ProjectIOSingletone.Get().InputMouseWhill * lookSpeedVer;

        camAngleSlerped = Vector3.Slerp(camAngleSlerped, camAngle, Time.deltaTime * lookSpeedSlerp);
        camAngleSlerped.z = 0;
        Quaternion rotation = Quaternion.Euler(camAngleSlerped);
        _transform.rotation = rotation;
        camOffsetXZSlerped = Vector3.Slerp(camOffsetXZSlerped, camOffsetXZ, lookSpeedSlerp * Time.deltaTime);

        Vector3 wantPosition = targetPos + _transform.TransformVector(AlwaysShowBody(camOffsetXZSlerped));

        // tempCube.position = wantPosition;
        wantPosition = CameraCollisionCheck(wantPosition, targetPos);
        _transform.position = wantPosition;
    }
    Vector3 AlwaysShowBody(Vector3 offset)
    {

        if (offset.z >= 0)
            offset.x = 0;
        else
            if (offset.x > 0)
            if (offset.x > Mathf.Abs(offset.z))
                offset.x = Mathf.Abs(offset.z);
            else if (offset.x < 0)
                if (offset.x < offset.z) offset.x = offset.z;

        return offset;
    }

    void Rest()
    {

    }
    
    void MoveBody()
    {
        if (ProjectIOSingletone.Get().ThePlayerB)
        {
            camForward = new Vector3(_transform.forward.x, 0, _transform.forward.z).normalized;
            camRight = new Vector3(camForward.z, 0, -camForward.x);
            moveDir = keyboardAxis.x * camRight + keyboardAxis.z * camForward;
            ProjectIOSingletone.Get().ThePlayerB.UpdatePlayer(moveDir, camForward);

            //Debug.DrawRay(targetPos, camForward * 2, Color.red);
           // Debug.DrawRay(targetPos, _transform.forward * 2, Color.blue);
           // Debug.DrawRay(targetPos, moveDir, Color.yellow);
        }
    }
    void MoveCamera()
    {       
        //Vector3 dir = new Vector3();
        //dir.x += ProjectIOSingletone.Get().InpKeybAxisH;
        //dir.z += ProjectIOSingletone.Get().InpKeybAxisV;
        targetPos = Vector3.Lerp(targetPos, targetPos + _transform.TransformDirection(keyboardAxis), Time.deltaTime * _transform.position.y);//.MoveTowards()
                                                                                                                                    //Vector3
    }

    Vector3 CameraCollisionCheck(Vector3 myPos, Vector3 bodyPos)
    {
        Ray rayCollision = new Ray(bodyPos, myPos - bodyPos);
        RaycastHit hitCollision;
        if (Physics.Raycast(rayCollision, out hitCollision, Vector3.Distance(myPos, bodyPos)))//,layerMask
        {//сохраняем z камеры равный расстоянию до обьекта столкновения
         //        if (hitCollision.transform.gameObject.layer != _botLayer)
            if (hitCollision.transform.gameObject.name != "Player")
                if (hitCollision.distance < Vector3.Distance(myPos, bodyPos))
                    return hitCollision.point;
        }
        return myPos;
    }
}
