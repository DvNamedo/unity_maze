using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goHome : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        // 게임 초기화 또는 리로드 로직을 실행합니다.
        // 예: SceneManager.LoadScene("GameScene");
        var ins = Centers.instance;


        ins.currentHP = ins.maxHP;

        ins.topWalls = new();
        ins.leftWalls = new();
        ins.centers = new();
        ins.bonusPoints = new();
        ins.spikePoints = new();

        ins.startPoint = new(0, 0, 0);
        ins.endPoint = new(1, 1, 1);

        ins.score = 0;

        ins.mapGen = false;

        ins.isGameEnd = false;

    Loading.LoadScene("settings");
    }

}
