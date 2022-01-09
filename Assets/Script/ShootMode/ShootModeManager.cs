using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        public static bool tutorial;        　      //チュートリアル？
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
            BLOCK_GENERATE_TIME = blickGenerateTime;
            RectTransform canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            CANVAS_WIDTH = canvasTra.sizeDelta.x;
            CANVAS_HIGH  = canvasTra.sizeDelta.y;

            //BGM開始
            int bgmIndex = 1;
            if (stageNum >= 8 && stageNum <= 14) bgmIndex = 2;
            else if (stageNum >= 15) bgmIndex = 3;
            soundManScr.BGM_Start(bgmIndex);
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

                //タイトル画面へ戻る
                Destroy(soundManObj);
                SceneNavigator.Instance.Change("TitleScene", 1.0f);
            }
        }
    }
}