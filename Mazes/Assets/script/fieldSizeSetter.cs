using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fieldSizeSetter : MonoBehaviour
{
    public float sizeCanstant;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localScale = new Vector3(Centers.instance.mazeSize * sizeCanstant, 1, Centers.instance.mazeSize * sizeCanstant);
        gameObject.transform.position = new Vector3(Centers.instance.mazeSize * sizeCanstant * 5, 1, Centers.instance.mazeSize * sizeCanstant * 5);
    }
}
