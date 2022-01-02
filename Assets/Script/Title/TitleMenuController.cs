using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoundFunction;
using System;
using ShootMode;
using static ShootMode.ShootModeDefine;

public class TitleMenuController : MonoBehaviour
{
    public enum TargetVegetable
    {
        Broccoli,
        Cabbage,
        Paprika,
        Carrot,
        Pumpkin,
        Corn
    }

    public enum Mode
    {
        puzzle = 0,
        shoot
    }
    static int modeNum = Enum.GetValues(typeof(Mode)).Length;
    const  int puzzleModeNum = (int)Mode.puzzle;
    const  int shootModeNum  = (int)Mode.shoot;

    [Header("モード")]
    [SerializeField]
    GameObject[] modeObj;

    GameObject[]    modeSelectObj = new GameObject[modeNum];    //モード選択ボタンオブジェクト
    GameObject[]    tutorialObj   = new GameObject[modeNum];    //チュートリアルボタンオブジェクト
    GameObject[]    handObj       = new GameObject[modeNum];    //手オブジェクト
    RectTransform[] handTra       = new RectTransform[modeNum]; //手RectTransform 
    Vector2[][]     handMovePos   = new Vector2[modeNum][];     //手動作座標
    RectTransform[] stageBoxTra   = new RectTransform[modeNum]; //ステージボックス
    GameObject[][]  stageObj      = new GameObject[modeNum][];  //ステージボタンオブジェクト
    int[]           stageNum      = new int[modeNum];           //ステージ数
    GameObject[]    selectBackObj = new GameObject[modeNum];    //選択画面に戻るボタンオブジェクト
    GameObject[]    rightArrow    = new GameObject[modeNum];    //右矢印ボタンオブジェクト
    RectTransform[] rightArrowTra = new RectTransform[modeNum]; //右矢印ボタンオブジェクト
    GameObject[]    leftArrow     = new GameObject[modeNum];    //左矢印ボタンオブジェクト
    RectTransform[] leftArrowTra  = new RectTransform[modeNum]; //左矢印ボタンオブジェクト

    [Header("SoundManager")]
    [SerializeField]
    SoundManager soundMan;

    [Header("ハムスターの親オブジェクト")]
    [SerializeField]
    Transform hamsterBox;

    [Header("風船ハムスタープレハブ")]
    [SerializeField]
    GameObject hamsterPre;

    [Header("ステージクリア表示プレハブ")]
    [SerializeField]
    GameObject stageClearPre;

    [Header("野菜Sprite")]
    [SerializeField]
    Sprite[] vegSprite;

    Camera cameraMain;                         //MainCamera
    RectTransform CanvasTra;                   //CanvasのRectTransform
    int[] displayStageNum = new int[modeNum];  //表示するステージ番号
    int   maxDisplay      = 10;                //最大表示数
    int   displayPageNum  = -1;                //表示しているページ番号
    int   nowDisplayMode  = -1;                //表示しているモード番号
    float Magnification;                       //タップ位置修正倍率
    float DifferenceX;                         //タップ位置修正数X
    float tapStartPosX;                        //タップ開始位置X
    bool  stageScroll = false;                 //ステージスクロール？
    bool  stageSelect = false;                 //ステージ選択？

    string[] objTag       = new string[] { "Button", "SettingScreen" }; //UIタップ判定用タグ
    float[]  stageBoxPosX = new float[]  { 0.0f, -1080.0f };            //ステージボックスX座標

    bool[] moveArrow = new bool[modeNum]; //矢印を動かす?
    float  moveSpeed = 0.01f;             //矢印動作速度
    float  maxScale  = 1.1f;              //拡大限界値
    float  minScale  = 0.9f;              //縮小限界値
    float  scale     = 1.0f;              //初期拡縮
    bool   expansion = true;              //拡張最大値?

    void Start()
    {
        //========================================================================
        //セーブデータ読み取り
        //========================================================================
        SaveDataManager saveMan = GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>();
        saveMan.PuzzleModeLoadData();
        displayStageNum = new int[] { saveMan.puzzelModeStageNum, saveMan.shootModeStageNum };
        //========================================================================


        //========================================================================
        //メンバ変数取得
        //========================================================================
        //共通
        //========================================================================
        CanvasTra     = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        cameraMain    = Camera.main;
        Magnification = CanvasTra.sizeDelta.x / Screen.width;
        DifferenceX   = CanvasTra.sizeDelta.x / 2;
        //========================================================================


        //========================================================================
        //メンバ変数取得・ボタン関数指定
        //========================================================================
        //モード別
        //========================================================================
        Button[][] stageBut  = new Button[modeNum][];   //ステージ選択ボタン
        Button[] tutorialBut = new Button[modeNum];     //チュートリアルボタン
        for (int modeNumber = 0; modeNumber < modeNum; modeNumber++)
        {
            //モードオブジェクトtransform取得
            Transform modeTra = modeObj[modeNumber].transform;

            //モード選択ボタンオブジェクト取得
            modeSelectObj[modeNumber] = modeTra.GetChild(0).gameObject;

            //チュートリアルボタンオブジェクト
            tutorialObj[modeNumber] = modeTra.GetChild(1).gameObject;
            tutorialBut[modeNumber] = tutorialObj[modeNumber].GetComponent<Button>();
            if (displayStageNum[modeNumber] == 0)
            {
                //手オブジェクト取得
                handObj[modeNumber]     = modeTra.GetChild(2).gameObject;
                handTra[modeNumber]     = handObj[modeNumber].GetComponent<RectTransform>();
                handMovePos[modeNumber] = new Vector2[] { new Vector2(25.0f, -400.0f), new Vector2(25.0f, -440.0f) };

                //チュートリアルボタン座標指定
                RectTransform TutorialTra    = tutorialObj[modeNumber].GetComponent<RectTransform>();
                TutorialTra.sizeDelta        = new Vector2(700.0f, 200.0f);
                TutorialTra.anchoredPosition = new Vector2(0.0f, -200.0f);
                TutorialTra.GetChild(0).gameObject.GetComponent<Text>().fontSize = 90;
            }

            //ステージオブジェクト取得
            stageBoxTra[modeNumber] = modeTra.GetChild(3).gameObject.GetComponent<RectTransform>();
            stageNum[modeNumber]    = stageBoxTra[modeNumber].childCount;
            stageObj[modeNumber]    = new GameObject[stageNum[modeNumber]];
            for (int stageInd = 0; stageInd < stageNum[modeNumber]; stageInd++)
            { stageObj[modeNumber][stageInd] = stageBoxTra[modeNumber].GetChild(stageInd).gameObject; }

            //ステージ選択ボタン取得
            stageBut[modeNumber] = new Button[stageNum[modeNumber]];
            for (int stageInd = 0; stageInd < stageNum[modeNumber]; stageInd++)
            { stageBut[modeNumber][stageInd] = stageObj[modeNumber][stageInd].GetComponent<Button>(); }

            //タイトルへ戻るボタン取得
            selectBackObj[modeNumber] = modeTra.GetChild(4).gameObject;

            //矢印オブジェクト取得
            rightArrow[modeNumber]    = modeTra.GetChild(5).gameObject;
            leftArrow[modeNumber]     = modeTra.GetChild(6).gameObject;
            rightArrowTra[modeNumber] = leftArrow[modeNumber].GetComponent<RectTransform>();
            leftArrowTra[modeNumber]  = leftArrow[modeNumber].GetComponent<RectTransform>();
            moveArrow[modeNumber]     = false;

            //モード選択・矢印・タイトルへ戻るボタン関数指定
            Button modeSelectBut = modeSelectObj[modeNumber].GetComponent<Button>();
            Button selectBackBut = selectBackObj[modeNumber].GetComponent<Button>();
            Button rightArrowbut = rightArrow[modeNumber].GetComponent<Button>();
            Button leftArrowbut  = leftArrow[modeNumber].GetComponent<Button>();
            switch (modeNumber)
            {
                //パズルモード
                case puzzleModeNum:
                    modeSelectBut.onClick.AddListener(() => OnClickPuzzleMode(true));
                    selectBackBut.onClick.AddListener(() => OnClickPuzzleMode(false));
                    rightArrowbut.onClick.AddListener(() => PuzzleStageDisplay(1));
                    leftArrowbut.onClick.AddListener (() => PuzzleStageDisplay(0));
                    break;

                //シュートモード
                case shootModeNum:
                    modeSelectBut.onClick.AddListener(() => OnClickShootMode(true));
                    selectBackBut.onClick.AddListener(() => OnClickShootMode(false));
                    rightArrowbut.onClick.AddListener(() => ShootStageDisplay(1));
                    leftArrowbut.onClick.AddListener (() => ShootStageDisplay(0));
                    break;
            }
        }
        //========================================================================


        //========================================================================
        //ボタンに関数を追加
        //========================================================================
        //パズルモード
        //========================================================================
        TargetVegetable[][] puzzleTargetVeg = new TargetVegetable[stageNum[puzzleModeNum] + 1][];
        puzzleTargetVeg[0]  = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot };
        puzzleTargetVeg[1]  = new TargetVegetable[] { TargetVegetable.Carrot };
        puzzleTargetVeg[2]  = new TargetVegetable[] { TargetVegetable.Cabbage };
        puzzleTargetVeg[3]  = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Broccoli };
        puzzleTargetVeg[4]  = new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Cabbage };
        puzzleTargetVeg[5]  = new TargetVegetable[] { TargetVegetable.Paprika };
        puzzleTargetVeg[6]  = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot, TargetVegetable.Paprika };
        puzzleTargetVeg[7]  = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli };
        puzzleTargetVeg[8]  = new TargetVegetable[] { TargetVegetable.Carrot };
        puzzleTargetVeg[9]  = new TargetVegetable[] { TargetVegetable.Pumpkin };
        puzzleTargetVeg[10] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Cabbage };
        puzzleTargetVeg[11] = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Carrot, TargetVegetable.Pumpkin };
        puzzleTargetVeg[12] = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Cabbage };
        puzzleTargetVeg[13] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Carrot, TargetVegetable.Cabbage, TargetVegetable.Broccoli };
        puzzleTargetVeg[14] = new TargetVegetable[] { TargetVegetable.Corn };
        puzzleTargetVeg[15] = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Corn };
        puzzleTargetVeg[16] = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Pumpkin };
        puzzleTargetVeg[17] = new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Corn, TargetVegetable.Paprika, TargetVegetable.Pumpkin };
        puzzleTargetVeg[18] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Paprika };
        puzzleTargetVeg[19] = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Corn, TargetVegetable.Broccoli, TargetVegetable.Pumpkin };
        puzzleTargetVeg[20] = new TargetVegetable[] { TargetVegetable.Carrot };

        int[][] puzzleTargetVegNum = new int[stageNum[puzzleModeNum] + 1][];
        puzzleTargetVegNum[0]  = new int[] { 6, 5 };
        puzzleTargetVegNum[1]  = new int[] { 6 };
        puzzleTargetVegNum[2]  = new int[] { 15 };
        puzzleTargetVegNum[3]  = new int[] { 10, 10 };
        puzzleTargetVegNum[4]  = new int[] { 15, 15 };
        puzzleTargetVegNum[5]  = new int[] { 40 };
        puzzleTargetVegNum[6]  = new int[] { 20, 25, 5 };
        puzzleTargetVegNum[7]  = new int[] { 20, 30 };
        puzzleTargetVegNum[8]  = new int[] { 60 };
        puzzleTargetVegNum[9]  = new int[] { 25 };
        puzzleTargetVegNum[10] = new int[] { 20, 20 };
        puzzleTargetVegNum[11] = new int[] { 10, 10, 10 };
        puzzleTargetVegNum[12] = new int[] { 40, 40 };
        puzzleTargetVegNum[13] = new int[] { 10, 15, 15, 20 };
        puzzleTargetVegNum[14] = new int[] { 20 };
        puzzleTargetVegNum[15] = new int[] { 30, 30 };
        puzzleTargetVegNum[16] = new int[] { 9, 9, 9 };
        puzzleTargetVegNum[17] = new int[] { 40, 50, 30, 40 };
        puzzleTargetVegNum[18] = new int[] { 30, 30 };
        puzzleTargetVegNum[19] = new int[] { 70, 20, 40, 35 };
        puzzleTargetVegNum[20] = new int[] { 50 };

        int[] puzzleTurn = new int[stageNum[puzzleModeNum] + 1];
        puzzleTurn[0]  = 50;
        puzzleTurn[1]  = 30;
        puzzleTurn[2]  = 30;
        puzzleTurn[3]  = 30;
        puzzleTurn[4]  = 30;
        puzzleTurn[5]  = 30;
        puzzleTurn[6]  = 20;
        puzzleTurn[7]  = 20;
        puzzleTurn[8]  = 20;
        puzzleTurn[9]  = 20;
        puzzleTurn[10] = 20;
        puzzleTurn[11] = 10;
        puzzleTurn[12] = 15;
        puzzleTurn[13] = 8;
        puzzleTurn[14] = 20;
        puzzleTurn[15] = 30;
        puzzleTurn[16] = 5;
        puzzleTurn[17] = 20;
        puzzleTurn[18] = 8;
        puzzleTurn[19] = 30;
        puzzleTurn[20] = 2;

        //チュートリアルボタンに関数指定
        tutorialBut[puzzleModeNum].onClick.AddListener(() => OnClickPuzzleStatge(puzzleTargetVeg[0], puzzleTargetVeg[0].Length, puzzleTargetVegNum[0], puzzleTurn[0], 0));

        //ボタンに関数指定
        for (int i = 0; i < stageNum[puzzleModeNum]; i++)
        {
            int index = i + 1;
            stageBut[puzzleModeNum][index - 1].onClick.AddListener(() => OnClickPuzzleStatge(puzzleTargetVeg[index], puzzleTargetVeg[index].Length, puzzleTargetVegNum[index], puzzleTurn[index], index));
        }

        //Clear表示
        for (int i = 1; i < displayStageNum[puzzleModeNum]; i++)
        {
            GameObject clearObj = Instantiate(stageClearPre);
            Transform clearTra = clearObj.transform;
            clearTra.SetParent(stageObj[puzzleModeNum][i].transform, false);
            int targetCount = puzzleTargetVeg[i].Length;
            for (int a = 0; a < clearTra.childCount; a++)
            {
                if (targetCount > a)
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[(int)puzzleTargetVeg[i][a]];
                else
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[6];
            }
        }
        //========================================================================


        //========================================================================
        //ボタンに関数を追加
        //========================================================================
        //シュートモード
        //========================================================================
        VegetableType[][] shootTargetVeg = new VegetableType[stageNum[shootModeNum] + 1][];
        shootTargetVeg[0]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Cabbage };
        shootTargetVeg[1]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Carrot };
        shootTargetVeg[2]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Paprika };
        shootTargetVeg[3]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Pumpkin };
        shootTargetVeg[4]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Corn };
        shootTargetVeg[5]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Cabbage };
        shootTargetVeg[6]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Carrot };
        shootTargetVeg[7]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Paprika };
        shootTargetVeg[8]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Pumpkin };
        shootTargetVeg[9]  = new VegetableType[] { VegetableType.Broccoli, VegetableType.Corn };
        shootTargetVeg[10] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Cabbage };
        shootTargetVeg[11] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Carrot };
        shootTargetVeg[12] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Paprika };
        shootTargetVeg[13] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Pumpkin };
        shootTargetVeg[14] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Corn };
        shootTargetVeg[15] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Cabbage };
        shootTargetVeg[16] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Carrot };
        shootTargetVeg[17] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Paprika };
        shootTargetVeg[18] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Pumpkin };
        shootTargetVeg[19] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Corn };
        shootTargetVeg[20] = new VegetableType[] { VegetableType.Broccoli, VegetableType.Corn };

        int[][] shootTargetVegNum = new int[stageNum[shootModeNum] + 1][];
        shootTargetVegNum[0]  = new int[] { 1, 3 };
        shootTargetVegNum[1]  = new int[] { 2, 3 };
        shootTargetVegNum[2]  = new int[] { 3, 3 };
        shootTargetVegNum[3]  = new int[] { 4, 3 };
        shootTargetVegNum[4]  = new int[] { 5, 3 };
        shootTargetVegNum[5]  = new int[] { 6, 3 };
        shootTargetVegNum[6]  = new int[] { 7, 3 };
        shootTargetVegNum[7]  = new int[] { 8, 3 };
        shootTargetVegNum[8]  = new int[] { 9, 3 };
        shootTargetVegNum[9]  = new int[] { 10, 3 };
        shootTargetVegNum[10] = new int[] { 11, 3 };
        shootTargetVegNum[11] = new int[] { 12, 3 };
        shootTargetVegNum[12] = new int[] { 13, 3 };
        shootTargetVegNum[13] = new int[] { 14, 3 };
        shootTargetVegNum[14] = new int[] { 15, 3 };
        shootTargetVegNum[15] = new int[] { 16, 3 };
        shootTargetVegNum[16] = new int[] { 17, 3 };
        shootTargetVegNum[17] = new int[] { 18, 3 };
        shootTargetVegNum[18] = new int[] { 19, 3 };
        shootTargetVegNum[19] = new int[] { 20, 3 };
        shootTargetVegNum[19] = new int[] { 21, 3 };

        float[] shootGenerateTime = new float[stageNum[shootModeNum] + 1];
        shootGenerateTime[0]  = 20.0f;
        shootGenerateTime[1]  = 20.0f;
        shootGenerateTime[2]  = 20.0f;
        shootGenerateTime[3]  = 20.0f;
        shootGenerateTime[4]  = 20.0f;
        shootGenerateTime[5]  = 15.0f;
        shootGenerateTime[6]  = 15.0f;
        shootGenerateTime[7]  = 15.0f;
        shootGenerateTime[8]  = 15.0f;
        shootGenerateTime[9]  = 15.0f;
        shootGenerateTime[10] = 10.0f;
        shootGenerateTime[11] = 10.0f;
        shootGenerateTime[12] = 10.0f;
        shootGenerateTime[13] = 10.0f;
        shootGenerateTime[14] = 10.0f;
        shootGenerateTime[15] = 5.0f;
        shootGenerateTime[16] = 5.0f;
        shootGenerateTime[17] = 5.0f;
        shootGenerateTime[18] = 5.0f;
        shootGenerateTime[19] = 5.0f;
        shootGenerateTime[20] = 5.0f;

        //チュートリアルボタンに関数指定
        tutorialBut[shootModeNum].onClick.AddListener(() => OnClickShootStatge(shootTargetVeg[0], shootTargetVeg[0].Length, shootTargetVegNum[0], shootGenerateTime[0], 0));

        //ボタンに関数指定
        for (int i = 0; i < stageNum[shootModeNum]; i++)
        {
            int index = i + 1;
            stageBut[shootModeNum][index - 1].onClick.AddListener(() => OnClickShootStatge(shootTargetVeg[index], shootTargetVeg[index].Length, shootTargetVegNum[index], shootGenerateTime[index], index));
        }

        //Clear表示
        for (int i = 1; i < displayStageNum[shootModeNum]; i++)
        {
            GameObject clearObj = Instantiate(stageClearPre);
            Transform clearTra = clearObj.transform;
            clearTra.SetParent(stageObj[shootModeNum][i].transform, false);
            int targetCount = shootTargetVeg[i].Length;
            for (int a = 0; a < clearTra.childCount; a++)
            {
                if (targetCount > a)
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[(int)shootTargetVeg[i][a]];
                else
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[6];
            }
        }
        //========================================================================

        StartCoroutine(soundMan.BGM_Start(0));  //BGM開始
        StartCoroutine(HamsterGenerate());      //ハムスター動作開始
    }

    //========================================================================
    //モード選択(パズルモード)
    //========================================================================
    //selectMode; モード選択？(elseの場合はモード選択画面に戻る)
    //========================================================================
    void OnClickPuzzleMode(bool selectMode)
    {
        //変数更新
        stageSelect = selectMode;

        //モード選択ボタン表示切替
        foreach (GameObject modeSelObj in modeSelectObj)
        { modeSelObj.SetActive(!selectMode); }

        //チュートリアルボタン表示切替
        tutorialObj[puzzleModeNum].SetActive(selectMode);

        //ステージクリア数0
        if (displayStageNum[puzzleModeNum] == 0)
        {
            //手オブジェクト表示切替
            handObj[puzzleModeNum].SetActive(selectMode);
            handTra[puzzleModeNum].anchoredPosition = handMovePos[puzzleModeNum][0];
            if (selectMode) StartCoroutine(PromptTutorial(puzzleModeNum));
        }
        else
        {
            //ステージ選択ボタン表示切替
            for (int stageInd = 0; stageInd < displayStageNum[puzzleModeNum] + 1; stageInd++)
            { stageObj[puzzleModeNum][stageInd].SetActive(selectMode); }
        }

        //戻るボタン表示切替
        selectBackObj[puzzleModeNum].SetActive(selectMode);

        if (selectMode)
        {
            soundMan.YesTapSE();
            StartCoroutine(PuzzleStageDisplay((displayStageNum[puzzleModeNum] <= maxDisplay) ? 0 : 1));
            nowDisplayMode = puzzleModeNum;
        }
        else
        {
            soundMan.NoTapSE();
            moveArrow[puzzleModeNum] = false;
            rightArrow[puzzleModeNum].SetActive(false);
            leftArrow[puzzleModeNum].SetActive(false);
            nowDisplayMode = -1;
        }
    }

    //========================================================================
    //ステージ選択ボタン表示(パズルモード)
    //========================================================================
    //displayPageNum; ステージ表示ページ番号
    //========================================================================
    IEnumerator PuzzleStageDisplay(int displayPageNum)
    {
        if (displayStageNum[puzzleModeNum] <= maxDisplay)
        {
            //矢印動作させない
            moveArrow[puzzleModeNum] = false;
        }
        else
        {
            //矢印動作開始
            moveArrow[puzzleModeNum] = true;
            switch (displayPageNum)
            {
                case 0:
                    leftArrow[puzzleModeNum].SetActive(false);
                    rightArrow[puzzleModeNum].SetActive(true);
                    break;
                case 1:
                    leftArrow[puzzleModeNum].SetActive(true);
                    rightArrow[puzzleModeNum].SetActive(false);
                    break;
            }
        }
        float oneFrameTime = 0.02f;
        float stagePosX = stageBoxTra[puzzleModeNum].anchoredPosition.x;
        while ((stagePosX <= stageBoxPosX[displayPageNum] - 1 || stagePosX >= stageBoxPosX[displayPageNum] + 1) && !stageScroll)
        {
            stageBoxTra[puzzleModeNum].anchoredPosition = Vector2.Lerp(stageBoxTra[puzzleModeNum].anchoredPosition, new Vector2(stageBoxPosX[displayPageNum], 0.0f), 0.3f);
            yield return new WaitForSeconds(oneFrameTime);
            stagePosX = stageBoxTra[puzzleModeNum].anchoredPosition.x;
        }
    }

    //========================================================================
    //パズルモードステージ選択(パズルモード開始)
    //========================================================================
    //targetVeg;    目標野菜
    //vegetableNum; 目標野菜の数
    //targetNum;    目標野菜の各収穫目標数
    //turnNum;      ターン数
    //stageNum;     ステージ番号
    //========================================================================
    void OnClickPuzzleStatge(TargetVegetable[] targetVeg, int vegetableNum, int[] targetNum, int turnNum, int stageNum)
    {
        soundMan.YesTapSE();
        SceneNavigator.Instance.Change("PuzzleMode", 0.5f);

        PuzzleMainController.tartgetVeg = new PuzzleMainController.TargetVegetable[vegetableNum];
        PuzzleMainController.targetNum  = new int[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            PuzzleMainController.tartgetVeg[i] = (PuzzleMainController.TargetVegetable)(int)targetVeg[i];
            PuzzleMainController.targetNum[i]  = targetNum[i];
        }
        PuzzleMainController.vegetableNum = vegetableNum;
        PuzzleMainController.turnNum      = turnNum;
        PuzzleMainController.stageNum     = stageNum;
        PuzzleMainController.tutorial     = (stageNum == 0) ? true : false;
    }

    //========================================================================
    //モード選択(シュートモード)
    //========================================================================
    //selectMode; モード選択？(elseの場合はモード選択画面に戻る)
    //========================================================================
    void OnClickShootMode(bool selectMode)
    {
        //変数更新
        stageSelect = selectMode;

        //モード選択ボタン表示切替
        foreach (GameObject modeSelObj in modeSelectObj)
        { modeSelObj.SetActive(!selectMode); }

        //チュートリアルボタン表示切替
        tutorialObj[shootModeNum].SetActive(selectMode);

        //ステージクリア数0
        if (displayStageNum[shootModeNum] == 0)
        {
            //手オブジェクト表示切替
            handObj[shootModeNum].SetActive(selectMode);
            handTra[shootModeNum].anchoredPosition = handMovePos[shootModeNum][0];
            if (selectMode) StartCoroutine(PromptTutorial(shootModeNum));
        }
        else
        {
            //ステージ選択ボタン表示切替
            for (int stageInd = 0; stageInd < displayStageNum[shootModeNum] + 1; stageInd++)
            { stageObj[shootModeNum][stageInd].SetActive(selectMode); }
        }

        //戻るボタン表示切替
        selectBackObj[shootModeNum].SetActive(selectMode);

        if (selectMode)
        {
            soundMan.YesTapSE();
            StartCoroutine(ShootStageDisplay((displayStageNum[shootModeNum] <= maxDisplay) ? 0 : 1));
            nowDisplayMode = shootModeNum;
        }
        else
        {
            soundMan.NoTapSE();
            moveArrow[shootModeNum] = false;
            rightArrow[shootModeNum].SetActive(false);
            leftArrow[shootModeNum].SetActive(false);
            nowDisplayMode = -1;
        }
    }

    //========================================================================
    //ステージ選択ボタン表示(シュートモード)
    //========================================================================
    //displayPageNum; ステージ表示ページ番号
    //========================================================================
    IEnumerator ShootStageDisplay(int displayPageNum)
    {
        if (displayStageNum[shootModeNum] <= maxDisplay)
        {
            //矢印動作させない
            moveArrow[shootModeNum] = false;
        }
        else
        {
            //矢印動作開始
            moveArrow[shootModeNum] = true;
            switch (displayPageNum)
            {
                case 0:
                    leftArrow[shootModeNum].SetActive(false);
                    rightArrow[shootModeNum].SetActive(true);
                    break;
                case 1:
                    leftArrow[shootModeNum].SetActive(true);
                    rightArrow[shootModeNum].SetActive(false);
                    break;
            }
        }
        float oneFrameTime = 0.02f;
        float stagePosX = stageBoxTra[shootModeNum].anchoredPosition.x;
        while ((stagePosX <= stageBoxPosX[displayPageNum] - 1 || stagePosX >= stageBoxPosX[displayPageNum] + 1) && !stageScroll)
        {
            stageBoxTra[shootModeNum].anchoredPosition = Vector2.Lerp(stageBoxTra[shootModeNum].anchoredPosition, new Vector2(stageBoxPosX[displayPageNum], 0.0f), 0.3f);
            yield return new WaitForSeconds(oneFrameTime);
            stagePosX = stageBoxTra[shootModeNum].anchoredPosition.x;
        }
    }

    //========================================================================
    //シュートモードステージ選択(シュートモード開始)
    //========================================================================
    //targetVeg;          目標野菜
    //targetVegetableNum; 目標野菜の数
    //targetNum;          目標野菜の各収穫目標数
    //blickGenerateTime;  生成時間
    //stageNum;           ステージ番号
    //========================================================================
    void OnClickShootStatge(VegetableType[] targetVeg, int targetVegetableNum, int[] targetNum, float blickGenerateTime, int stageNum)
    {
        soundMan.YesTapSE();
        SceneNavigator.Instance.Change("ShootMode", 0.5f);

        ShootModeManager.tartgetVeg = new VegetableType[targetVegetableNum];
        ShootModeManager.targetNum = new int[targetVegetableNum];
        for (int i = 0; i < targetVegetableNum; i++)
        {
            ShootModeManager.tartgetVeg[i] = (VegetableType)(int)targetVeg[i];
            ShootModeManager.targetNum[i] = targetNum[i];
        }
        ShootModeManager.targetVegetableNum = targetVegetableNum;
        ShootModeManager.blickGenerateTime = blickGenerateTime;
        ShootModeManager.stageNum = stageNum;
        ShootModeManager.tutorial = (stageNum == 0) ? true : false;
    }

    //========================================================================
    //ハムスター生成
    //========================================================================
    IEnumerator HamsterGenerate()
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(5.0f, 12.0f);
            yield return new WaitForSeconds(waitTime);
            GameObject hamsterObj = Instantiate(hamsterPre);
            hamsterObj.transform.SetParent(hamsterBox, false);
        }
    }

    //========================================================================
    //チュートリアル選択を促す
    //========================================================================
    //modeNumber;  モード番号
    //========================================================================
    IEnumerator PromptTutorial(int modeNumber)
    {
        float oneFrameTime = 0.02f;
        float moveSpeed    = 0.0f;
        bool firstMoveEnd  = false;
        int targetIndex    = 1;

        while (handObj[modeNumber].activeSelf)
        {
            moveSpeed += oneFrameTime;
            handTra[modeNumber].anchoredPosition = Vector2.Lerp(handTra[modeNumber].anchoredPosition, handMovePos[modeNumber][targetIndex], moveSpeed);

            if (!firstMoveEnd && handTra[modeNumber].anchoredPosition.y - 1 <= handMovePos[modeNumber][targetIndex].y)
            {
                firstMoveEnd = true;
                moveSpeed = 0.0f;
                targetIndex = 0;
            }
            else if (firstMoveEnd && handTra[modeNumber].anchoredPosition.y + 1 >= handMovePos[modeNumber][targetIndex].y)
            {
                firstMoveEnd = false;
                moveSpeed = 0.0f;
                targetIndex = 1;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }
    }

    void FixedUpdate()
    {
        if (stageSelect && (displayStageNum[puzzleModeNum] > maxDisplay || displayStageNum[shootModeNum] > maxDisplay))
        {
            //タップ位置のオブジェクト有無確認
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (!hit2d || 0 > Array.IndexOf(objTag, hit2d.transform.gameObject.tag))
                {
                    stageScroll = true;
                    tapStartPosX = Input.mousePosition.x;
                    tapStartPosX = tapStartPosX * Magnification - DifferenceX;
                }
            }

            //ステージ選択時のスクロール動作
            if (stageScroll)
            {
                float posX = Input.mousePosition.x * Magnification - DifferenceX - tapStartPosX + stageBoxPosX[displayPageNum];
                RectTransform referenceTra = stageBoxTra[nowDisplayMode];
                referenceTra.anchoredPosition = Vector2.Lerp(referenceTra.anchoredPosition, new Vector2(posX, 0.0f), 1);
                if (referenceTra.anchoredPosition.x > stageBoxPosX[0])
                    referenceTra.anchoredPosition = Vector2.Lerp(referenceTra.anchoredPosition, new Vector2(stageBoxPosX[0], 0.0f), 0.9f);
                else if (referenceTra.anchoredPosition.x < stageBoxPosX[1])
                    referenceTra.anchoredPosition = Vector2.Lerp(referenceTra.anchoredPosition, new Vector2(stageBoxPosX[1], 0.0f), 0.9f);
            }

            //スクロール終了
            if (stageScroll && Input.GetMouseButtonUp(0))
            {
                stageScroll = false;
                switch (nowDisplayMode)
                {
                    case puzzleModeNum: StartCoroutine(PuzzleStageDisplay((stageBoxTra[puzzleModeNum].anchoredPosition.x > stageBoxPosX[1] / 2.0f) ? 0 : 1)); break;
                    case shootModeNum:  StartCoroutine(ShootStageDisplay((stageBoxTra[shootModeNum].anchoredPosition.x > stageBoxPosX[1] / 2.0f) ? 0 : 1));   break;
                }

            }

            //矢印拡縮動作
            if (moveArrow[nowDisplayMode])
            {
                if (expansion)
                {
                    scale += moveSpeed;
                    if (scale > maxScale)
                        expansion = false;
                }
                else
                {
                    scale -= moveSpeed;
                    if (scale < minScale)
                        expansion = true;
                }
                if (rightArrow[nowDisplayMode].activeSelf) rightArrowTra[nowDisplayMode].localScale = new Vector2(scale, scale);
                if (leftArrow[nowDisplayMode].activeSelf)  leftArrowTra[nowDisplayMode].localScale  = new Vector2(scale, scale);
            }
        }
    }
}