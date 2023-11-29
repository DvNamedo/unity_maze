using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class panelManagement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    GameObject settingPanel;

    [SerializeField]
    bool isOnPanel = true;

    void Start()
    {
        settingPanel.GetComponent<RectTransform>().SetAsLastSibling();
        if(isOnPanel)
            settingPanel.SetActive(false);
        else
            settingPanel.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isOnPanel)
            settingPanel.SetActive(true);
        else
            settingPanel.SetActive(false);
    }
}
