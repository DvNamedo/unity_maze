using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 cameraOffset = new Vector3(0f, 0.5f, 0f); // 플레이어를 기준으로 한 카메라 위치 조정
    public float rotationSpeed = 5.0f; // 회전 속도 조절

    Movement movementScript;

    void Start()
    {
        movementScript = playerTransform.GetComponent<Movement>();
    }

    void LateUpdate()
    {
        if (movementScript != null && movementScript.moveVec != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(movementScript.moveVec);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 desiredPosition = playerTransform.position + cameraOffset;
        transform.position = desiredPosition;
    }
}