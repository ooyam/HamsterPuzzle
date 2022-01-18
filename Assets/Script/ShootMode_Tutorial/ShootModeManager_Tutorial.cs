using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SoundFunction;
using ShootMode;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class ShootModeManager_Tutorial : MonoBehaviour
    {
        [Header("背景オブジェクトTra")]
        [SerializeField]
        RectTransform backGroundTra;

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

            //ゲーム開始
            GAME_START = true;

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
                StartCoroutine(feverObj.GetComponent<FeverHamuster_Tutorial>().FeverStart());
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
                StartCoroutine(gameOverObj.GetComponent<GameOverObj_Tutorial>().DirectGameOver(this, soundManScr));
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
                StartCoroutine(gameOverObj.GetComponent<GameClearObj_Tutorial>().DirectGameClear(this, soundManScr));
                yield return new WaitWhile(() => gameClearObjDis == true);

                //クリアステージ番号書き込み
                GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>().WriteShootModeSaveData(stageNum);

                //タイトル画面へ戻る
                Destroy(soundManObj);
                SceneNavigator.Instance.Change("TitleScene", 1.0f);
            }
        }
    }
}