using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMove : MonoBehaviour
{
    public float moveRange = 0;
    public float moveSpeed = 0;

    float firstPosY;

    // Start is called before the first frame update
    void Start()
    {
        firstPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, firstPosY, transform.position.z) + new Vector3(0, moveRange * Mathf.Sin(moveSpeed * Time.time), 0); 
    }
}
