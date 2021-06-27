using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    public enum TargetVegetable
    {
        Broccoli,
        Cabbage,
        Paprika,
        Carrot
    }
    private TargetVegetable[] targetVeg; //目標野菜(enumである必要はないがenumを配列で使う練習を行ってみたた)
    private int vegetableNum;            //目標野菜の個数
    private int[] targetNum;             //種類ごとの目標個数
    private bool[] targetAchievement;    //目標達成？
    private int achievementNum = 0;      //目標達成数
    private bool gameClear = false;      //ゲームクリア

    [Header("数字のスプライト")]
    public Sprite[] Numbers;
    [Header("スコア表示オブジェクト")]
    public GameObject broccoliScoreObj;
    public GameObject cabbageScoreObj;
    public GameObject paprikaScoreObj;
    public GameObject carrotScoreScoreObj;

    private PanelManager PanelMan;  //PanelManager

    private int displayDigits = 3;  //表示桁数
    private Image[] broccoliDisNum; //ブロッコリー数字表示オブジェクト
    private Image[] cabbageDisNum;  //キャベツ
    private Image[] paprikaDisNum;  //パプリカ
    private Image[] carrotDisNum;   //ニンジン

    private int broccoliScore = 0;  //ブロッコリー収穫個数
    private int cabbageScore = 0;   //キャベツ
    private int paprikaScore = 0;   //パプリカ
    private int carrotScore = 0;    //ニンジン

    private const string broccoli = "Broccoli"; //ブロッコリー
    private const string cabbage = "Cabbage";   //キャベツ
    private const string paprika = "Paprika";   //パプリカ
    private const string carrot = "Carrot";     //ニンジン

    // Start is called before the first frame update
    void Start()
    {
        PanelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();

        Transform broccoliScoreTra = broccoliScoreObj.transform;
        Transform cabbageScoreTra = cabbageScoreObj.transform;
        Transform paprikaScoreTra = paprikaScoreObj.transform;
        Transform carrotScoreScoreTra = carrotScoreScoreObj.transform;

        broccoliDisNum = new Image[displayDigits];
        cabbageDisNum = new Image[displayDigits];
        paprikaDisNum = new Image[displayDigits];
        carrotDisNum = new Image[displayDigits];
        for (int i = 0; i < displayDigits; i++)
        {
            broccoliDisNum[i] = broccoliScoreTra.GetChild(i).gameObject.GetComponent<Image>();
            cabbageDisNum[i] = cabbageScoreTra.GetChild(i).gameObject.GetComponent<Image>();
            paprikaDisNum[i] = paprikaScoreTra.GetChild(i).gameObject.GetComponent<Image>();
            carrotDisNum[i] = carrotScoreScoreTra.GetChild(i).gameObject.GetComponent<Image>();
        }

        vegetableNum = PuzzleMainController.vegetableNum;
        targetNum = PuzzleMainController.targetNum;
        targetAchievement = new bool[vegetableNum];
        targetVeg = new TargetVegetable[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            targetVeg[i] = (TargetVegetable)(int)PuzzleMainController.tartgetVeg[i];
            targetAchievement[i] = false;
        }
    }

    //収穫完了野菜
    public void HarvestVegetable(string VegetableName)
    {
        switch (VegetableName)
        {
            case broccoli:
                broccoliScore++;
                DigitCalculation(broccoliScore, broccoliDisNum, TargetVegetable.Broccoli);
                break;
            case cabbage:
                cabbageScore++;
                DigitCalculation(cabbageScore, cabbageDisNum, TargetVegetable.Cabbage);
                break;
            case paprika:
                paprikaScore++;
                DigitCalculation(paprikaScore, paprikaDisNum, TargetVegetable.Paprika);
                break;
            case carrot:
                carrotScore++;
                DigitCalculation(carrotScore, carrotDisNum, TargetVegetable.Carrot);
                break;
        }
    }

    //スコア表示
    private void DigitCalculation(int referenceNum, Image[] updateScore, TargetVegetable referenceVeg)
    {
        int ten = 10;
        int digitsNum = 1;
        int vegHarvestNum = referenceNum;
        if (referenceNum >= ten * ten) digitsNum = 3;
        else if (referenceNum >= ten)
        {
            digitsNum = 2;
            updateScore[digitsNum].sprite = Numbers[ten];
        }
        else
        {
            updateScore[1].sprite = Numbers[ten];
            updateScore[2].sprite = Numbers[ten];
        }

        int SpriteIndex = 0;
        for (int i = 0; i < digitsNum; i++)
        {
            SpriteIndex = referenceNum % ten;
            updateScore[i].sprite = Numbers[SpriteIndex];
            referenceNum /= ten;
        }

        //クリア判定
        if (!gameClear)
        {
            int referenceVegIndex = Array.IndexOf(targetVeg, referenceVeg);
            if (0 <= referenceVegIndex)
            {
                if (!targetAchievement[referenceVegIndex] && targetNum[referenceVegIndex] <= vegHarvestNum)
                {
                    targetAchievement[referenceVegIndex] = true;
                    achievementNum++;
                }
                if (achievementNum == vegetableNum)
                {
                    gameClear = true;
                    PanelMan.gameClear = true;
                }
            }
        }
    }
}
