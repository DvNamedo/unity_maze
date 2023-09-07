using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationText : MonoBehaviour
{
    [SerializeField]
    Text massage;

    public List<string> Textpage = null;

    public bool isSameDelay = true;


    public float pageDelaySolo = 0.5f;
    public List<float> pageDelay = null;

    int count = 0;

    private void Awake()
    {
        if(Textpage.Count == 0)
        {
            isSameDelay = true;
        }

        if(Textpage.Count != pageDelay.Count)
        {
            isSameDelay = true;
        }
    }


    private void OnEnable()
    {
        StartCoroutine(massageChange());
    }

    private void OnDisable()
    {
        StopCoroutine(massageChange());
    }

    IEnumerator massageChange()
    {

        while (true)
        {
            if (isSameDelay)
            {
                yield return new WaitForSeconds(pageDelaySolo);

                massage.text = Textpage[count % pageDelay.Count];
            }
            else
            {
                yield return new WaitForSeconds(pageDelay[count % pageDelay.Count]);
                massage.text = Textpage[count % pageDelay.Count];
            }

            count++;
        }
    }
}
