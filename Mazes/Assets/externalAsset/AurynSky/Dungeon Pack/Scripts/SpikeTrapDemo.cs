using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapDemo : MonoBehaviour {

    //This script goes on the SpikeTrap prefab;

    public Animator spikeTrapAnim; //Animator for the SpikeTrap;
    public bool isSafe = false;
    float random1 = 2f;
    float random2 = 2f;

    // Use this for initialization
    void Awake()
    {
        //get the Animator component from the trap;
        spikeTrapAnim = GetComponent<Animator>();
        //start opening and closing the trap for demo purposes;
        StartCoroutine(OpenCloseTrap());

        random1 = Random.Range(1.5f, 3.0f);
        random2 = Random.Range(1.5f, 3.0f);

    }


    IEnumerator OpenCloseTrap()
    {
        //play open animation;
        spikeTrapAnim.SetTrigger("open");
        isSafe = false;
        //wait 2 seconds;
        yield return new WaitForSeconds(random1);
        //play close animation;
        spikeTrapAnim.SetTrigger("close");
        isSafe = true;
        //wait 2 seconds;
        yield return new WaitForSeconds(random2);
        //Do it again;
        StartCoroutine(OpenCloseTrap());

    }
}