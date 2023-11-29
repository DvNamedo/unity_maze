using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("a");
        List<GameObject> children = new List<GameObject>();

        for (int i = 1; i <= 30; i++)
        {
            children.Add(GameObject.Find($"Cube ({i})"));
            Debug.Log("b");
        }

        foreach (GameObject child in children)
        {
            child.AddComponent<timeSetter>();
            Debug.Log("c");
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            gameObject.transform.position -= new Vector3(1*Time.deltaTime, 0, 0);
        if (Input.GetKey(KeyCode.RightArrow))
            gameObject.transform.position += new Vector3(1*Time.deltaTime, 0, 0);
        if (Input.GetKey(KeyCode.DownArrow))
            gameObject.transform.position -= new Vector3(0, 0, 1*Time.deltaTime);
        if (Input.GetKey(KeyCode.UpArrow))
            gameObject.transform.position += new Vector3(0, 0, 1* Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftShift))
            gameObject.transform.position -= new Vector3(0, 1* Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.Space))
            gameObject.transform.position += new Vector3(0, 1 * Time.deltaTime, 0);
    }
}
