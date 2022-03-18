using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SoundFunction;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ShootModeManager : MonoBehaviour
    {
        [Header("背景オブジェクトTra")]
        [SerializeField]
        RectTransform backGroundTra;

        [Header("背景Sprite")]
        [SerializeField]
        Sprite[] backGroundSpr;

        [Header("全消しフィーバーハムスタープレハブ")]
        [SerializeField]
        GameObject feverPre;

        [Header("ゲームオーバープレハブ")]
        [SerializeField]
        GameObject gameOverObjPre;
        [System.NonSerialized]
        public bool gameOverObjDis = false;  //表示中フラグ

        [Header("ゲームクリアプレハブ")]
        [SerializeField]
        GameObject gameClearObjPre;
        [System.NonSerialized]
        public bool gameClearObjDis = false;  //表示中フラグ

        [Header("リザルト画面プレハブ")]
        [SerializeField]
        GameObject resultScreenPre;

        [Header("インタースティシャル広告")]
        [SerializeField]
        GameObject interstitialObj;

        GameObject soundManObj;
        SoundManager soundManScr;  //SoundManger

        public static VegetableType[] tartgetVeg;   //目標野菜
        public static int targetVegNum;             //目標野菜の個数
        public static int useVegNum;                //使用する野菜の個数
        public static int[] targetNum;              //種類ごとの目標個数
        public static float blickGenerateTime;      //生成時間
        public static int stageNum;                 //ステージ番号
        string[] vegName;                           //野菜の名前enum

        void Awake()
        {
            //各フラグリセット
            FlagReset();

            //SoundoManager取得
            soundManObj = GameObject.FindWithTag("SoundManager");
            soundManScr = soundManObj.GetComponent<SoundManager>();

            //enum取得
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegName = new string[vegetableType.Length];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            //定数設定
            BLOCK_GENERATE_TIME         = blickGenerateTime;
            RectTransform canvasTra     = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            RectTransform playScreenTra = canvasTra.GetChild(1).GetComponent<RectTransform>();
            CANVAS_WIDTH       = canvasTra.sizeDelta.x;
            CANVAS_HEIGHT      = canvasTra.sizeDelta.y;
            PLAY_SCREEN_WIDTH  = playScreenTra.sizeDelta.x;
            PLAY_SCREEN_HEIGHT = playScreenTra.sizeDelta.y;

            //BGM開始
            int bgmIndex = 1;
            if (stageNum >= 8 && stageNum <= 14) bgmIndex = 2;
            else if (stageNum >= 15) bgmIndex = 3;
            soundManScr.BGM_Start(bgmIndex);

            Image backGroundIma = backGroundTra.GetComponent<Image>();
            if (stageNum < 6)
                backGroundIma.sprite = backGroundSpr[0];
            else if (stageNum < 11)
                backGroundIma.sprite = backGroundSpr[1];
            else if (stageNum < 16)
                backGroundIma.sprite = backGroundSpr[2];
            else
                backGroundIma.sprite = backGroundSpr[3];
        }

        //========================================================================
        //フィーバー
        //========================================================================
        public void Fever()
        {
            if (!FEVER_START)
            {
                FEVER_START = true;

                //フィーバーオブジェクト生成
                GameObject feverObj = Instantiate(feverPre);
                feverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(feverObj.GetComponent<FeverHamuster>().FeverStart());
            }
        }


        //========================================================================
        //ゲームオーバー
        //========================================================================
        public IEnumerator GameOver()
        {
            if (!gameOverObjDis)
            {
                GAME_OVER = true;

                //ゲームオーバーオブジェクト生成
                gameOverObjDis = true;
                GameObject gameOverObj = Instantiate(gameOverObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameOverObj>().DirectGameOver(this, soundManScr));
                yield return new WaitWhile(() => gameOverObjDis == true);

                //リザルト画面表示
                GameObject resultScreenObj = Instantiate(resultScreenPre);
                resultScreenObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
            }
        }

        //========================================================================
        //ゲームクリア
        //========================================================================
        public IEnumerator GameClear()
        {
            if (!gameClearObjDis)
            {
                //ゲームクリアオブジェクト生成
                gameClearObjDis = true;
                GameObject gameOverObj = Instantiate(gameClearObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameClearObj>().DirectGameClear(this, soundManScr));
                yield return new WaitWhile(() => gameClearObjDis == true);

                //クリアステージ番号書き込み
                GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>().WriteShootModeSaveData(stageNum);

                //インタースティシャル広告表示
                Instantiate(interstitialObj);
            }
        }
    }
}