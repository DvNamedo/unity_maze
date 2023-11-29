using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panelManagementToList : MonoBehaviour
{
    [SerializeField]
    List<GameObject> panels;

    private void Awake()
    {
        if(gameObject.GetComponent<Dropdown>() == null)
        {
            Debug.LogError("해당 UI 형식에서는 사용할 수 없음.");
        }
    }

    private void Start()
    {
        foreach(GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        panels[0].SetActive(true);

    }

    private void Update()
    {

    }

    public void panelActiveProcess()
    {
        for(int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(gameObject.GetComponent<Dropdown>().value == i);
        }
    }

}
