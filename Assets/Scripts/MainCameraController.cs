using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Großteil entnommen aus https://forum.unity.com/threads/third-person-camera-rotate.197592/

public class MainCameraController : MonoBehaviour
{
    public Transform CameraTarget;
    private float x = 0.0f;
    private float y = 0.0f;

    private int mouseXSpeedMod = 5;
    private int mouseYSpeedMod = 5;

    public float MaxViewDistance = 15f;
    public float MinViewDistance = 1f;
    public int ZoomRate = 20;
    private int lerpRate = 5;
    private float distance = 3f;
    private float desireDistance;
    private float correctedDistance;
    private float currentDistance;

    public float cameraTargetHeight = 1.0f;

    // Use this for initialization
    void Start()
    {
        transform.eulerAngles = CameraTarget.eulerAngles;
        transform.Rotate(new Vector3(0, 20));
        Vector3 Angles = transform.eulerAngles;
        x = Angles.x;
        y = Angles.y + 5f;
        currentDistance = distance;
        desireDistance = distance;
        correctedDistance = distance;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float cameraX = transform.rotation.x;
        if (gameObject.name == "CamPlayer2")
        {
            x += Input.GetAxis("Mouse X") * mouseXSpeedMod;
            y += Input.GetAxis("Mouse Y") * mouseYSpeedMod;
            if((Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0) && Input.GetAxis("Mouse X") == 0)
            {
                SetCamera();
            }
            else
            {
                CameraTarget.eulerAngles = new Vector3(cameraX, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else if (gameObject.name == "CamPlayer1")
        {
            x += Input.GetAxis("Mouse X2") * mouseXSpeedMod;
            y += Input.GetAxis("Mouse Y2") * mouseYSpeedMod;
            if((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && Input.GetAxis("Mouse X2") == 0)
            {
                SetCamera();
            }
            else
            {
                CameraTarget.eulerAngles = new Vector3(cameraX, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }

        y = ClampAngle(y, 0, 75);
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        desireDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomRate * Mathf.Abs(desireDistance);
        desireDistance = Mathf.Clamp(desireDistance, MinViewDistance, MaxViewDistance);
        correctedDistance = desireDistance;

        Vector3 position = CameraTarget.position - (rotation * Vector3.forward * desireDistance);

        RaycastHit collisionHit;
        Vector3 cameraTargetPosition = new Vector3(CameraTarget.position.x, CameraTarget.position.y + cameraTargetHeight, CameraTarget.position.z);

        bool isCorrected = false;
        if (Physics.Linecast(cameraTargetPosition, position, out collisionHit))
        {
            position = collisionHit.point;
            correctedDistance = Vector3.Distance(cameraTargetPosition, position);
            isCorrected = true;
        }

        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * ZoomRate) : correctedDistance;

        position = CameraTarget.position - (rotation * Vector3.forward * currentDistance + new Vector3(0, -cameraTargetHeight, 0));

        transform.rotation = rotation;
        transform.position = position;

    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    private void SetCamera()
    {
        float targetRotantionAngle = CameraTarget.eulerAngles.y;
        float cameraRotationAngle = transform.eulerAngles.y;
        x = Mathf.LerpAngle(cameraRotationAngle, targetRotantionAngle, lerpRate * Time.deltaTime);
    }

}
