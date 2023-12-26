using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timerText; // static 제거
    public Text scoreText; // static 제거
    public Text Result;

    public GameObject home;
    public GameObject clearPanel; // static 제거
    
    public int scoreModifier = 1;

    private float timer = 0f;
    private int score = 0;

    private void Start()
    {
        clearPanel.SetActive(false);
        StartCoroutine(GameClear());
    }

    void Update()
    {   

        if (!Centers.instance.isGameEnd)
        {
            scoreText.text = "Score: " + Centers.instance.score.ToString();

            timer += Time.deltaTime;
            
            timerText.text = "Time: " + timer.ToString("F2");
            
            //Result = timer - score * scoreModifier * Time.deltaTime;
        }

        
    }


    
    IEnumerator GameClear()
    {
        yield return new WaitUntil(() => Centers.instance.isGameEnd);

        if (clearPanel != null)
        {
            if (!Centers.instance.isDead)
                Result.text = "환산 시간 | " + (timer - (float)Centers.instance.score * scoreModifier).ToString("F2");
            else
                Result.text = "점수 | " + Centers.instance.score.ToString();

            clearPanel.SetActive(true);
            Image panelImage = clearPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                Color panelColor = panelImage.color;

                if (!Centers.instance.isDead)
                    panelColor.a = 0.5f;
                else
                    panelColor = Color.red;
                    panelColor.a = 0.5f;

                panelImage.color = panelColor;
            }
        }

        if(scoreText != null)
            scoreText.text = "Score: " + Centers.instance.score.ToString();
        
        Time.timeScale = 0f;
    }


    //버튼의 onClick 에 놔야지 ㅇㅇ 다른데에서 씀
    public void RestartGame()
    {
        Time.timeScale = 1f;
        // 게임 초기화 또는 리로드 로직을 실행합니다.
        // 예: SceneManager.LoadScene("GameScene");
    }
}
