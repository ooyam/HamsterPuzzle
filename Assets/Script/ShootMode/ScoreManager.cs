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

        [Header("スコア表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：ニンジン　3:パプリカ　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //オブジェクトRectTransform
        RectTransform[] scoreNumTra; //収穫数RectTransform
        Image[][] scoreNumIma;       //出力数字

        [Header("ターゲット表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：ニンジン　3:パプリカ　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] targetObj;
        RectTransform[] targetObjTra; //オブジェクトRectTransform
        RectTransform[] targetNumTra; //収穫数RectTransform
        Image[][] targetNumIma;       //出力数字
        int maxDigit = 2;             //最大桁数

        VegetableType[] tartgetVeg_;  //目標野菜
        int targetVegetableNum_;      //目標野菜の個数
        int[] targetNum_;             //種類ごとの目標個数
        int stageNum_;                //ステージ番号

        string[] vegName;  //野菜の名前
        int vegTypeNum;    //使用する野菜の数
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

            scoreObjTra  = new RectTransform[vegTypeNum];
            scoreNumTra  = new RectTransform[vegTypeNum];
            scoreNumIma  = new Image[vegTypeNum][];
            targetObjTra = new RectTransform[vegTypeNum];
            targetNumTra = new RectTransform[vegTypeNum];
            targetNumIma = new Image[vegTypeNum][];
            for (int index = 0; index < vegTypeNum; index++)
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
            }
            harvestNum = new int[vegTypeNum];

            //ステージ設定
            tartgetVeg_         = tartgetVeg;            //目標野菜
            targetVegetableNum_ = targetVegetableNum;    //目標野菜の個数
            targetNum_          = targetNum;             //種類ごとの目標個数
            stageNum_           = stageNum;              //ステージ番号
            targetIndex = new int[targetVegetableNum_];  //目標野菜のインデクス番号
            clearJudge  = new bool[targetVegetableNum_]; //クリア判定
            int ten = 10;
            for (int index = 0; index < targetVegetableNum_; index++)
            {
                int tensPlace = (int)Mathf.Floor(targetNum_[index] / ten); //10の位
                int onesPlace = targetNum_[index] % ten;                   //1の位
                targetIndex[index] = (int)tartgetVeg_[index];
                targetNumIma[targetIndex[index]][0].sprite = targetNumSpr[onesPlace];
                targetNumIma[targetIndex[index]][1].sprite = (tensPlace == 0) ? targetNumSpr[ten] : targetNumSpr[tensPlace];
                clearJudge[index] = false;
            }
        }

        //野菜収穫
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
                if (targetIndexIndex >= 0 && targetNum_[targetIndexIndex] <= harvestNum[vegIndex])
                {
                    clearJudge[targetIndexIndex] = true;

                    //クリア判定
                    if (Array.IndexOf(clearJudge, false) < 0)
                        Debug.Log("クリアですぞよ");
                }
            }
        }
    }
}
