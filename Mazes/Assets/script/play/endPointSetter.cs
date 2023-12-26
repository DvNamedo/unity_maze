using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endPointSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(endSetter());   
    }

    IEnumerator endSetter()
    {
        
        yield return new WaitUntil(() => Centers.instance.mapGen);
        Debug.Log($"Gen at {Centers.instance.endPoint}");
        transform.position = Centers.instance.endPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
