using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool spaceDown;
    bool isJumping;

    [HideInInspector] public Vector3 moveVec;
    Rigidbody rigid;

    void Awake() 
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(startSetter());
    }

     IEnumerator startSetter()
    {
        yield return new WaitUntil(() => Centers.instance.mapGen);
        transform.position = Centers.instance.startPoint;
    }

    // Update is called once per frame
    void Update()


    {
        GetInput();
        Move();
        Turn();
        Jump();
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        spaceDown = Input.GetButtonDown("Jump");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
    }

    void Move()
    {
        Vector3 movement = moveVec * speed * Time.deltaTime;
        rigid.MovePosition(transform.position + movement);
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveVec);
            rigid.MoveRotation(newRotation);
        }
    }

    void Jump()
    {
        if (spaceDown && !isJumping)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJumping = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("kill"))
        {
            Debug.Log("Collision Detected");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }
}
        
    

