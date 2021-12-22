using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("数字スプライト")]
        [SerializeField]
        Sprite[] numberSpr;

        [Header("スコア表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：ニンジン　3:パプリカ　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] scoreObj;

        RectTransform[] scoreObjTra; //オブジェクトRectTransform
        RectTransform[] numTra;      //収穫数RectTransform
        Image[][] numIma;            //出力数字
        int maxDigit = 2;            //最大桁数

        string[] vegName; //野菜の名前
        int vegTypeNum;   //使用する野菜の数
        int[] harvestNum; //収穫個数

        void Start()
        {
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegTypeNum = vegetableType.Length;
            vegName = new string[vegTypeNum];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            scoreObjTra = new RectTransform[vegTypeNum];
            numTra      = new RectTransform[vegTypeNum];
            numIma      = new Image[vegTypeNum][];
            for (int index = 0; index < vegTypeNum; index++)
            {
                scoreObjTra[index] = scoreObj[index].GetComponent<RectTransform>();
                numTra[index]      = scoreObjTra[index].GetChild(0).GetComponent<RectTransform>();
                numIma[index]      = new Image[maxDigit];
                numIma[index][0]   = numTra[index].GetChild(0).GetComponent<Image>();
                numIma[index][1]   = numTra[index].GetChild(1).GetComponent<Image>();
            }
            harvestNum = new int[vegTypeNum];
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
                numIma[vegIndex][0].sprite = numberSpr[onesPlace];
                numIma[vegIndex][1].sprite = (tensPlace == 0) ? numberSpr[ten] : numberSpr[tensPlace];
            }
        }
    }
}
