using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 cameraOffset = new Vector3(0f, 0.5f, 0f); // �÷��̾ �������� �� ī�޶� ��ġ ����
    public float rotationSpeed = 5.0f; // ȸ�� �ӵ� ����

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