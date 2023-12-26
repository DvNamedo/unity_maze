using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSetter : MonoBehaviour
{
    [Header("FoundationDifficulty")]
    public Centers.Difficulty difficulty = Centers.Difficulty.none;

    [Header("Custom")]
    public List<GameObject> childUI;

    Centers ins = null;

    public void Start()
    {
        ins = Centers.instance;
    }

    public void setAndStartGame()
    {
        setDifficultElement();
        startMapGenerate();
    }

    public void startMapGenerate()
    {
        Loading.LoadScene("MapProcess");
    }

    public void setDifficultElement()
    {
        ins.difficulty = difficulty;
        switch (difficulty)
        {
            case Centers.Difficulty.Easy :
                ins.mazeSize = 40;                // CLEAR
                ins.personView = Centers.PV.TPV;  // CLEAR?
                ins.trapFrequecy = 0.005f;        // X
                ins.distanceForEnd = 0.5f;        // CLEAR
                ins.bonusFrequency = 0.01f;          // X
                break;

            case Centers.Difficulty.Normal :
                normalSettings();
                break;

            case Centers.Difficulty.Hard :
                hardSettings();
                break;

            case Centers.Difficulty.Normal_CH :
                normalSettings();

                ins.challanges.Add(Centers.Challanges.HardControl);
                break;

            case Centers.Difficulty.Hard_CH :
                hardSettings();

                ins.challanges.Add(Centers.Challanges.HardControl);
                ins.challanges.Add(Centers.Challanges.FallDown);
                break;

            case Centers.Difficulty.VeryHard_CH :
                ins.mazeSize = 300;
                ins.personView = Centers.PV.FPV;
                ins.trapFrequecy = 0.075f;
                ins.distanceForEnd = 0.9f;
                ins.bonusFrequency = 0.05f;

                ins.challanges.Add(Centers.Challanges.HardControl);
                ins.challanges.Add(Centers.Challanges.WallDeath);
                break;
            
            default :
                break;
        }
    }

    void normalSettings()
    {
        ins.mazeSize = 100;
        ins.personView = Centers.PV.FPV;
        ins.trapFrequecy = 0.02f;
        ins.distanceForEnd = 0.75f;
        ins.bonusFrequency = 0.01f;
    }

    void hardSettings()
    {
        ins.mazeSize = 200;
        ins.personView = Centers.PV.FPV;
        ins.trapFrequecy = 0.03f;
        ins.distanceForEnd = 0.80f;
        ins.bonusFrequency = 0.02f;
    }

}
