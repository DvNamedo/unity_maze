using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool isJump;

    Vector3 moveVec;
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
        wDown = Input.GetButton("Walk"); 
        jDown = Input.GetButtonDown("Jump");
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        Vector3 movement = moveVec * speed * Time.deltaTime;

        if (wDown)
            movement *= 0.3f;

        rigid.MovePosition(transform.position + movement);
    }

    void Turn()
    {

        transform.LookAt(transform.position + moveVec);
    }
    void Jump()
    {
        if (jDown && !isJump){
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision) {
            if(collision.gameObject.tag == "kill") {
                Debug.Log("Collide Detect");
    
            }
        }
}
        
    

