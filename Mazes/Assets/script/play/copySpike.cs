using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class copySpike : MonoBehaviour
{
    public GameObject spikePrefab;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setBonus());
    }

    IEnumerator setBonus()
    {
        yield return new WaitUntil(() => Centers.instance.mapGen);

        foreach (Vector3 pos in Centers.instance.spikePoints.Union(Centers.instance.bonusPoints).ToList())
        {
            Instantiate(spikePrefab, pos - new Vector3(0, 1.2f, 0), spikePrefab.transform.rotation, parent);
        }
    }
}
