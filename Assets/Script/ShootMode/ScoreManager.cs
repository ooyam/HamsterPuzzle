using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ShootMode.ShootModeDefine;
using static ShootMode.ShootModeManager;

namespace ShootMode
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("スコア用数字スプライト")]
        [SerializeField]
        Sprite[] scoreNumSpr;

        [Header("ターゲット用数字スプライト")]
        [SerializeField]
        Sprite[] targetNumSpr;

        [Header("スコア表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：パプリカ　3:ニンジン　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //オブジェクトRectTransform
        RectTransform[] scoreNumTra; //収穫数RectTransform
        Image[][] scoreNumIma;       //出力数字

        [Header("ターゲット表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：パプリカ　3:ニンジン　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] targetObj;
        RectTransform[] targetObjTra; //オブジェクトRectTransform
        RectTransform[] targetNumTra; //収穫数RectTransform
        Image[][] targetNumIma;       //出力数字
        int maxDigit = 2;             //最大桁数
        float tarNumPosFixX = -26.0f; //ターゲット数量表示座標修正値

        VegetableType[] tarVeg;  //目標野菜
        int tarVegNum;           //目標野菜の個数
        int usingVegNum;         //使用する野菜の個数
        int[] tarNum;            //種類ごとの目標個数
        int staNum;              //ステージ番号

        string[] vegName;  //野菜の名前
        int vegTypeNum;    //野菜の数
        int[] harvestNum;  //収穫個数
        int[] targetIndex; //目標野菜のインデクス番号
        bool[] clearJudge; //クリア判定

        void Start()
        {
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegTypeNum = vegetableType.Length;
            vegName = new string[vegTypeNum];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            usingVegNum = useVegNum;
            Vector2[] scoreVegPos = new Vector2[usingVegNum];
            float[] scoreVegPosY  = new float[] { -65.0f, -135.0f, -200.0f };
            switch (usingVegNum)
            {
                case 3:
                    scoreVegPos[0] = new Vector2(-450.0f, scoreVegPosY[1]);
                    scoreVegPos[1] = new Vector2(-245.0f, scoreVegPosY[1]);
                    scoreVegPos[2] = new Vector2(-25.0f,  scoreVegPosY[1]);
                    break;
                case 4:
                    scoreVegPos[0] = new Vector2(-400.0f, scoreVegPosY[0]);
                    scoreVegPos[1] = new Vector2(-145.0f, scoreVegPosY[0]);
                    scoreVegPos[2] = new Vector2(-260.0f, scoreVegPosY[2]);
                    scoreVegPos[3] = new Vector2(20.0f,   scoreVegPosY[2]);
                    break;
                case 5:
                    scoreVegPos[0] = new Vector2(-300.0f, scoreVegPosY[0]);
                    scoreVegPos[1] = new Vector2(-85.0f,  scoreVegPosY[0]);
                    scoreVegPos[2] = new Vector2(-380.0f, scoreVegPosY[2]);
                    scoreVegPos[3] = new Vector2(-175.0f, scoreVegPosY[2]);
                    scoreVegPos[4] = new Vector2(45.0f,   scoreVegPosY[2]);
                    break;
            }

            scoreObjTra  = new RectTransform[usingVegNum];
            scoreNumTra  = new RectTransform[usingVegNum];
            scoreNumIma  = new Image[usingVegNum][];
            targetObjTra = new RectTransform[usingVegNum];
            targetNumTra = new RectTransform[usingVegNum];
            targetNumIma = new Image[usingVegNum][];
            for (int index = 0; index < usingVegNum; index++)
            {
                //ターゲットオブジェクト
                targetObjTra[index]    = targetObj[index].GetComponent<RectTransform>();
                targetNumTra[index]    = targetObjTra[index].GetChild(1).GetComponent<RectTransform>();
                targetNumIma[index]    = new Image[maxDigit];
                targetNumIma[index][0] = targetNumTra[index].GetChild(0).GetComponent<Image>();
                targetNumIma[index][1] = targetNumTra[index].GetChild(1).GetComponent<Image>();

                //スコアオブジェクト
                scoreObjTra[index]    = scoreObj[index].GetComponent<RectTransform>();
                scoreNumTra[index]    = scoreObjTra[index].GetChild(0).GetComponent<RectTransform>();
                scoreNumIma[index]    = new Image[maxDigit];
                scoreNumIma[index][0] = scoreNumTra[index].GetChild(0).GetComponent<Image>();
                scoreNumIma[index][1] = scoreNumTra[index].GetChild(1).GetComponent<Image>();

                //スコアオブジェクト座標指定
                scoreObj[index].SetActive(true);
                if (vegTypeNum > usingVegNum) scoreObjTra[index].anchoredPosition = scoreVegPos[index];
            }
            harvestNum = new int[usingVegNum];

            //ステージ設定
            tarVeg      = tartgetVeg;          //目標野菜
            tarVegNum   = targetVegNum;        //目標野菜の個数
            tarNum      = targetNum;           //種類ごとの目標個数
            staNum      = stageNum;            //ステージ番号
            targetIndex = new int[tarVegNum];  //目標野菜のインデクス番号
            clearJudge  = new bool[tarVegNum]; //クリア判定
            int ten     = 10;
            for (int index = 0; index < tarVegNum; index++)
            {
                int tensPlace = (int)Mathf.Floor(tarNum[index] / ten); //10の位
                int onesPlace = tarNum[index] % ten;                   //1の位
                targetIndex[index] = (int)tarVeg[index];
                targetNumIma[targetIndex[index]][0].sprite = targetNumSpr[onesPlace];
                targetNumIma[targetIndex[index]][1].sprite = (tensPlace == 0) ? targetNumSpr[ten] : targetNumSpr[tensPlace];
                if (tensPlace == 0) targetNumTra[targetIndex[index]].anchoredPosition = new Vector2(tarNumPosFixX, 0.0f);
                clearJudge[index] = false;
            }

            //ターゲットオブジェクト座標指定
            float valueX = 130.0f;
            float valueY = -20.0f;
            float fixX   = valueX / 2.0f * (tarVegNum - 1);
            for (int index = 0; index < tarVegNum; index++)
            {
                targetObjTra[targetIndex[index]].anchoredPosition = new Vector2(valueX * index - fixX, valueY);
                targetObj[targetIndex[index]].SetActive(true);
            }

        }


        //========================================================================
        //野菜収穫
        //========================================================================
        //vegetableName; 野菜の名前(タグ名)
        //========================================================================
        public void HarvestVegetable(string vegetableName)
        {
            int vegIndex = Array.IndexOf(vegName, vegetableName);
            if (vegIndex >= 0)
            {
                if (harvestNum[vegIndex] < 99) harvestNum[vegIndex]++;  //カンスト99

                //桁数判断
                int ten = 10;
                int tensPlace = (int)Mathf.Floor(harvestNum[vegIndex] / ten); //10の位
                int onesPlace = harvestNum[vegIndex] % ten;                   //1の位

                //数字更新
                scoreNumIma[vegIndex][0].sprite = scoreNumSpr[onesPlace];
                scoreNumIma[vegIndex][1].sprite = (tensPlace == 0) ? scoreNumSpr[ten] : scoreNumSpr[tensPlace];

                //目標野菜収穫完了判断
                int targetIndexIndex = Array.IndexOf(targetIndex, vegIndex);
                if (targetIndexIndex >= 0 && !clearJudge[targetIndexIndex] && tarNum[targetIndexIndex] <= harvestNum[vegIndex])
                {
                    //収穫完了判定
                    clearJudge[targetIndexIndex] = true;
                    targetNumIma[vegIndex][0].sprite = targetNumSpr[11];
                    targetNumIma[vegIndex][1].sprite = targetNumSpr[10];
                    targetNumTra[vegIndex].anchoredPosition = new Vector2(0.0f, 0.0f);

                    //クリア判定
                    if (!GAME_CLEAR && Array.IndexOf(clearJudge, false) < 0)
                        GAME_CLEAR = true;
                }
            }
        }
    }
}
