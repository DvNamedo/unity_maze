using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class runAwayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float runAwayDirection_radian = Mathf.PI;
    public float runAwaySpeed = 1.0f;
    public float offset = 1.0f;
    public bool isDirectionRandom = false;

    bool isOnUI = false;
    Vector2 initPos;
    
    void Start()
    {
        initPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }

    void Update()
    {
        if(isOnUI)
            gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(runAwaySpeed * Mathf.Cos(runAwayDirection_radian), runAwaySpeed * Mathf.Sin(runAwayDirection_radian));

        else if (!((gameObject.GetComponent<RectTransform>().anchoredPosition.x < initPos.x + offset && gameObject.GetComponent<RectTransform>().anchoredPosition.x > initPos.x - offset) &&
                 (gameObject.GetComponent<RectTransform>().anchoredPosition.y < initPos.y + offset && gameObject.GetComponent<RectTransform>().anchoredPosition.y > initPos.y - offset))) 
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition -= new Vector2(runAwaySpeed * Mathf.Cos(runAwayDirection_radian), runAwaySpeed * Mathf.Sin(runAwayDirection_radian));
        }
            
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDirectionRandom)
        {
            if((gameObject.GetComponent<RectTransform>().anchoredPosition.x < initPos.x + offset && gameObject.GetComponent<RectTransform>().anchoredPosition.x > initPos.x - offset) &&
               (gameObject.GetComponent<RectTransform>().anchoredPosition.y < initPos.y + offset && gameObject.GetComponent<RectTransform>().anchoredPosition.y > initPos.y - offset))
                runAwayDirection_radian = Random.Range(0, 2 * Mathf.PI);
        }

        isOnUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(waitingExit());
    }

    IEnumerator waitingExit()
    {
        yield return new WaitForSeconds(1.5f);
        isOnUI = false;
    }
}
