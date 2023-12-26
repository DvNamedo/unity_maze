using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyBonus : MonoBehaviour
{
    public GameObject bonusPrefab;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setBonus());
    }

    IEnumerator setBonus()
    {
        yield return new WaitUntil(() => Centers.instance.mapGen);

        foreach (Vector3 pos  in Centers.instance.bonusPoints)
        {
            Instantiate(bonusPrefab, pos, bonusPrefab.transform.rotation, parent);
        }
    }

}
