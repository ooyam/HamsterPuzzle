using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    public enum TargetVegetable
    {
        Broccoli = 0,
        Cabbage,
        Paprika,
        Carrot,
        Pumpkin,
        Corn
    }
    private TargetVegetable[] targetVeg; //目標野菜
    private int vegetableNum;            //目標野菜の個数
    private int[] targetNum;             //種類ごとの目標個数
    private int stageNum;                //ステージ番号
    private bool[] targetAchievement;    //目標達成？
    private int achievementNum = 0;      //目標達成数
    private bool gameClear = false;      //ゲームクリア

    [Header("PanelManager")]
    public PanelManager PanelMan;
    [Header("TagetController")]
    public TagetController TagetCon;
    [Header("数字のスプライト")]
    public Sprite[] Numbers;
    [Header("スコア表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：パプリカ　3:ニンジン　4：カボチャ　5：トウモロコシ
    public GameObject[] scoreObj;
    private RectTransform[] scoreObjTra; //オブジェクトRectTransform
    private RectTransform[] numTra;      //収穫数RectTransform
    private Image[][] displayNum;        //出力数字

    //収穫個数
    private int[] harvestNum = new int[] { 0, 0, 0, 0, 0, 0 };
    //収穫種類
    private string[] harvestVeg = Enum.GetNames(typeof(TargetVegetable));

    // Start is called before the first frame update
    void Start()
    {
        int vegNum = Enum.GetValues(typeof(TargetVegetable)).Length;
        scoreObjTra = new RectTransform[vegNum];
        numTra = new RectTransform[vegNum];
        int displayDigits = 2;
        displayNum = new Image[vegNum][];
        for (int a = 0; a < vegNum; a++)
        {
            scoreObjTra[a] = scoreObj[a].GetComponent<RectTransform>();
            numTra[a] = scoreObjTra[a].GetChild(0).gameObject.GetComponent<RectTransform>();
            displayNum[a] = new Image[displayDigits];
            for (int b = 0; b < displayDigits; b++)
            {
                displayNum[a][b] = numTra[a].GetChild(b).gameObject.GetComponent<Image>();
            }
        }

        vegetableNum = PuzzleMainController.vegetableNum;
        targetNum = PuzzleMainController.targetNum;
        stageNum = PuzzleMainController.stageNum;
        targetAchievement = new bool[vegetableNum];
        targetVeg = new TargetVegetable[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            targetVeg[i] = (TargetVegetable)(int)PuzzleMainController.tartgetVeg[i];
            targetAchievement[i] = false;
        }

        //収穫数表示場所指定
        Vector2[] displayPos;
        float[] posY = new float[] { -65.0f, -200.0f };
        if (stageNum >= 13)
        {
            displayPos = new Vector2[]
            {
                new Vector2(-450.0f, posY[0]),
                new Vector2(-215.0f, posY[0]),
                new Vector2(35.0f, posY[0]),
                new Vector2(-380.0f, posY[1]),
                new Vector2(-145.0f, posY[1]),
                new Vector2(105.0f, posY[1])
            };
        }
        else if (stageNum >= 9)
        {
            displayPos = new Vector2[]
            {
                new Vector2(-450.0f, posY[0]),
                new Vector2(-215.0f, posY[0]),
                new Vector2(35.0f, posY[0]),
                new Vector2(-310.0f, posY[1]),
                new Vector2(-60.0f, posY[1])
            };
            scoreObj[5].SetActive(false);
        }
        else
        {
            displayPos = new Vector2[]
            {
                new Vector2(-350.0f, posY[0]),
                new Vector2(-235.0f, posY[1]),
                new Vector2(-110.0f, posY[0]),
                new Vector2(30.0f, posY[1])
            };
            scoreObj[4].SetActive(false);
            scoreObj[5].SetActive(false);
        }

        int loopTimes = 0;
        foreach (Vector2 disPos in displayPos)
        {
            scoreObjTra[loopTimes].anchoredPosition = disPos;
            loopTimes++;
        }
    }

    //収穫完了野菜
    public void HarvestVegetable(string VegetableName)
    {
        int VegIndex = Array.IndexOf(harvestVeg, VegetableName);
        TargetVegetable targetName = (TargetVegetable)Enum.ToObject(typeof(TargetVegetable), VegIndex);
        harvestNum[VegIndex]++;
        DigitCalculation(harvestNum[VegIndex], displayNum[VegIndex], targetName);
    }

    //スコア表示
    private void DigitCalculation(int referenceNum, Image[] updateScore, TargetVegetable referenceVeg)
    {
        int ten = 10;
        int digitsNum = 2;
        int vegHarvestNum = referenceNum;
        float harvestNumPosX = 40.0f;
        float harvestNumPosY = -40.0f;
        if (referenceNum >= ten * ten)
            referenceNum = 99; //99カンスト
        else if (referenceNum < ten)
        {
            harvestNumPosX = -15.0f;
            digitsNum = 1;
            updateScore[1].sprite = Numbers[ten];
        }

        numTra[(int)referenceVeg].anchoredPosition = new Vector2(harvestNumPosX, harvestNumPosY);

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
                    TagetCon.TargetAchievement(referenceVegIndex);
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
