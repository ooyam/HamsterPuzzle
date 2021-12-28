using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ShootModeManager : MonoBehaviour
    {
        [Header("背景オブジェクトTra")]
        [SerializeField]
        RectTransform backGroundTra;

        [Header("ゲームオーバープレハブ")]
        [SerializeField]
        GameObject gameOverObjPre;
        [System.NonSerialized]
        public bool gameOverObjDis = false;  //表示中フラグ

        [Header("ゲームクリアプレハブ")]
        [SerializeField]
        GameObject gameClearObjPre;

        [Header("リザルト画面プレハブ")]
        [SerializeField]
        GameObject resultScreenPre;

        public static VegetableType[] tartgetVeg;   //目標野菜
        public static int targetVegetableNum;       //目標野菜の個数
        public static int[] targetNum;              //種類ごとの目標個数
        public static float blickGenerateTime;      //生成時間
        public static int stageNum;                 //ステージ番号
        public static bool tutorial;        　      //チュートリアル？
        string[] vegName;                           //野菜の名前enum

        void Awake()
        {
            FlagReset();

            //enum取得
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegName = new string[vegetableType.Length];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            //ステージ設定(仮)
            tartgetVeg = new VegetableType[] { VegetableType.Broccoli };
            targetVegetableNum = 1;
            targetNum = new int[] { 1 };
            blickGenerateTime = 15.0f;
            stageNum = 0;
            tutorial = false;
            BLOCK_GENERATE_TIME = blickGenerateTime;
        }

        public IEnumerator GameOver()
        {
            if (!gameOverObjDis)
            {
                GAME_OVER = true;

                //ゲームオーバーオブジェクト生成
                gameOverObjDis = true;
                GameObject gameOverObj = Instantiate(gameOverObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameOverObj>().DirectGameOver(this));
                yield return new WaitWhile(() => gameOverObjDis == true);

                //リザルト画面表示
                GameObject resultScreenObj = Instantiate(resultScreenPre);
                resultScreenObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
            }
        }
    }
}