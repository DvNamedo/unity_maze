using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<TrailRenderer>().time = 100;
    }
}
