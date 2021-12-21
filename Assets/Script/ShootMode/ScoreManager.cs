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
        [Header("スコア表示オブジェクト")]   //0：ブロッコリー　1:キャベツ　2：パプリカ　3:ニンジン　4：カボチャ　5：トウモロコシ
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //オブジェクトRectTransform
        RectTransform[] numTra;      //収穫数RectTransform
        Image[][] numIma;            //出力数字
        int maxDigit = 2;            //最大桁数

        int vegTypeNum = Enum.GetValues(typeof(VegetableType)).Length; //使用する野菜の数
        int[] harvestNum; //収穫個数

        void Start()
        {
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
    }
}
