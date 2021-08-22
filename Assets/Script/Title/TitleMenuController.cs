using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

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

    [Header("パズルモード開始ボタン")]
    public GameObject pouzzleModeObject;
    private Button pouzzleModeButton;
    public GameObject[] pouzzleStageObject;
    private Button[] pouzzleStageButton;
    [Header("選択画面に戻るボタン")]
    public GameObject selectBuckObject;
    private Button selectBackButton;
    [Header("SoundManager")]
    public SoundManager soundMan;
    private SaveDataManager saveMan;
    [Header("ハムスターの親オブジェクト")]
    public Transform hamsterBox;
    [Header("風船ハムスター")]
    public GameObject hamsterPre;
    [Header("矢印")]
    public GameObject[] arrow; //0:Left 1:Right
    private RectTransform[] arrowTra;
    private Button[] arrowBut;
    [Header("手")]
    public GameObject hand;
    private RectTransform handTra;
    private Vector2[] targetPos;
    [Header("ステージクリア表示プレハブ")]
    public GameObject stageClearPre;
    [Header("野菜Sprite")]
    public Sprite[] vegSprite;

    private int displayStageNum;      //表示するステージ番号
    private int maxDisplay = 10;      //最大表示数

    [Header("ステージボックス")]
    public RectTransform stageTra;
    private Camera cameraMain;        //MainCamera
    private RectTransform CanvasTra;  //CanvasのRectTransform
    private int displayPageNum;       //表示しているページ番号
    private float Magnification;      //タップ位置修正倍率
    private float DifferenceX;        //タップ位置修正数X
    private float tapStartPosX;       //タップ開始位置X
    private bool stageScroll = false; //ステージスクロール？
    private bool stageSelect = false; //ステージ選択？
    private string[] objTag = new string[] { "Button", "SettingScreen" };
    private float[] stageBoxPosX = new float[] { 0.0f, -1080.0f };

    private bool moveArrow = false;   //矢印を動かす?
    private float moveSpeed = 0.01f;  //矢印動作速度
    private float maxScale = 1.1f;    //拡大限界値
    private float minScale = 0.9f;    //縮小限界値
    private float scale = 1.0f;       //初期拡縮
    private bool expansion = true;    //拡張最大値?

    // Start is called before the first frame update
    void Start()
    {
        CanvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        cameraMain = Camera.main;
        Magnification = CanvasTra.sizeDelta.x / Screen.width;
        DifferenceX = CanvasTra.sizeDelta.x / 2;
        StartCoroutine(soundMan.BGM_Start(0));

        saveMan = GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>();
        saveMan.PuzzleModeLoadData();
        displayStageNum = saveMan.puzzelModeStageNum;
        pouzzleModeButton = pouzzleModeObject.GetComponent<Button>();
        selectBackButton = selectBuckObject.GetComponent<Button>();
        pouzzleStageButton = new Button[pouzzleStageObject.Length];
        for (int i = 0; i < pouzzleStageObject.Length; i++)
        {
            pouzzleStageButton[i] = pouzzleStageObject[i].GetComponent<Button>();
        }

        if (displayStageNum == 0)
        {
            handTra = hand.GetComponent<RectTransform>();
            targetPos = new Vector2[] { new Vector2(25.0f, -400.0f), new Vector2(25.0f, -440.0f) };
            RectTransform TutorialTra = pouzzleStageButton[0].GetComponent<RectTransform>();
            TutorialTra.sizeDelta = new Vector2(700.0f, 200.0f);
            TutorialTra.anchoredPosition = new Vector2(0.0f, -200.0f);
            TutorialTra.GetChild(0).gameObject.GetComponent<Text>().fontSize = 90;
        }

        int arrowNum = arrow.Length;
        arrowTra = new RectTransform[arrowNum];
        arrowBut = new Button[arrowNum];
        for (int i = 0; i < arrowNum; i++)
        {
            arrowTra[i] = arrow[i].GetComponent<RectTransform>();
            arrowBut[i] = arrow[i].GetComponent<Button>();
            float posX = stageBoxPosX[i];
            arrowBut[i].onClick.AddListener(() => StartCoroutine(StageDisplay(posX)));
        }

        //ボタンに関数を追加
        int stageNum = pouzzleStageButton.Length;
        TargetVegetable[][] tragetVeg = new TargetVegetable[stageNum][];
        tragetVeg[0]  = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot };
        tragetVeg[1]  = new TargetVegetable[] { TargetVegetable.Carrot };
        tragetVeg[2]  = new TargetVegetable[] { TargetVegetable.Cabbage };
        tragetVeg[3]  = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Broccoli };
        tragetVeg[4]  = new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Cabbage };
        tragetVeg[5]  = new TargetVegetable[] { TargetVegetable.Paprika };
        tragetVeg[6]  = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot, TargetVegetable.Paprika };
        tragetVeg[7]  = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli };
        tragetVeg[8]  = new TargetVegetable[] { TargetVegetable.Carrot };
        tragetVeg[9]  = new TargetVegetable[] { TargetVegetable.Pumpkin };
        tragetVeg[10] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Cabbage };
        tragetVeg[11] = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Carrot, TargetVegetable.Pumpkin };
        tragetVeg[12] = new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Cabbage };
        tragetVeg[13] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Carrot, TargetVegetable.Cabbage, TargetVegetable.Broccoli };
        tragetVeg[14] = new TargetVegetable[] { TargetVegetable.Corn };
        tragetVeg[15] = new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Corn };
        tragetVeg[16] = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Pumpkin };
        tragetVeg[17] = new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Corn, TargetVegetable.Paprika, TargetVegetable.Pumpkin };
        tragetVeg[18] = new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Paprika };
        tragetVeg[19] = new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Corn, TargetVegetable.Broccoli, TargetVegetable.Pumpkin };
        tragetVeg[20] = new TargetVegetable[] { TargetVegetable.Carrot };

        int[][] targetVegNum = new int[stageNum][];
        targetVegNum[0]  = new int[] { 6, 5 };
        targetVegNum[1]  = new int[] { 6 };
        targetVegNum[2]  = new int[] { 15 };
        targetVegNum[3]  = new int[] { 10, 10 };
        targetVegNum[4]  = new int[] { 15, 15 };
        targetVegNum[5]  = new int[] { 40 };
        targetVegNum[6]  = new int[] { 20, 25, 5 };
        targetVegNum[7]  = new int[] { 20, 30 };
        targetVegNum[8]  = new int[] { 60 };
        targetVegNum[9]  = new int[] { 25 };
        targetVegNum[10] = new int[] { 20, 20 };
        targetVegNum[11] = new int[] { 10, 10, 10 };
        targetVegNum[12] = new int[] { 40, 40 };
        targetVegNum[13] = new int[] { 10, 15, 15, 20 };
        targetVegNum[14] = new int[] { 20 };
        targetVegNum[15] = new int[] { 30, 30 };
        targetVegNum[16] = new int[] { 9, 9, 9 };
        targetVegNum[17] = new int[] { 40, 50, 30, 40 };
        targetVegNum[18] = new int[] { 30, 30 };
        targetVegNum[19] = new int[] { 70, 20, 40, 35 };
        targetVegNum[20] = new int[] { 50 };

        int[] turn = new int[stageNum];
        turn[0]  = 50;
        turn[1]  = 30;
        turn[2]  = 30;
        turn[3]  = 30;
        turn[4]  = 30;
        turn[5]  = 30;
        turn[6]  = 20;
        turn[7]  = 20;
        turn[8]  = 20;
        turn[9]  = 20;
        turn[10] = 20;
        turn[11] = 10;
        turn[12] = 15;
        turn[13] = 8;
        turn[14] = 20;
        turn[15] = 30;
        turn[16] = 5;
        turn[17] = 20;
        turn[18] = 8;
        turn[19] = 30;
        turn[20] = 2;

        for (int i = 0; i < stageNum; i++)
        {
           int index = i;
           pouzzleStageButton[index].onClick.AddListener(() => OnClickGameStart(tragetVeg[index], tragetVeg[index].Length, targetVegNum[index], turn[index], index));
        }

        for (int i = 1; i < displayStageNum; i++)
        {
            GameObject clearObj = Instantiate(stageClearPre);
            Transform clearTra = clearObj.transform;
            clearTra.SetParent(pouzzleStageObject[i].transform, false);
            int targetCount = tragetVeg[i].Length;
            for (int a = 0; a < clearTra.childCount; a++)
            {
                if(targetCount > a) 
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[(int)tragetVeg[i][a]];
                else
                    clearTra.GetChild(a).gameObject.GetComponent<Image>().sprite = vegSprite[6];
            }
        }

        pouzzleModeButton.onClick.AddListener(() => OnClickSelectMode(true));
        selectBackButton.onClick.AddListener(() => OnClickSelectMode(false));

        StartCoroutine(HamsterGenerate());
    }

    void OnClickGameStart(TargetVegetable[] targetVeg, int vegetableNum, int[] targetNum, int turnNum, int stageNum)
    {
        soundMan.YesTapSE();
        SceneNavigator.Instance.Change("PuzzleMode", 0.5f);

        PuzzleMainController.tartgetVeg = new PuzzleMainController.TargetVegetable[vegetableNum];
        PuzzleMainController.targetNum = new int[vegetableNum];
        for (int i = 0; i < vegetableNum; i++)
        {
            PuzzleMainController.tartgetVeg[i] = (PuzzleMainController.TargetVegetable)(int)targetVeg[i];
            PuzzleMainController.targetNum[i] = targetNum[i];
        }
        PuzzleMainController.vegetableNum = vegetableNum;
        PuzzleMainController.turnNum = turnNum;
        PuzzleMainController.stageNum = stageNum;
        PuzzleMainController.tutorial = (stageNum == 0) ? true : false;
    }
    void OnClickSelectMode(bool selectMode)
    {
        if (displayStageNum == 0)
        {
            hand.SetActive(selectMode);
            handTra.anchoredPosition = targetPos[0];
            if (selectMode) StartCoroutine(PromptTutorial());
        }
        stageSelect = selectMode;
        pouzzleModeObject.SetActive(!selectMode);
        int stageTotal = pouzzleStageObject.Length - 1;
        int loopTimes = (displayStageNum > stageTotal) ? stageTotal : displayStageNum;
        for (int i = 0; i < loopTimes + 1; i++)
        {
            pouzzleStageObject[i].SetActive(selectMode);
        }
        selectBuckObject.SetActive(selectMode);
        if (selectMode)
        {
            soundMan.YesTapSE();
            StartCoroutine(StageDisplay((displayStageNum <= maxDisplay) ? stageBoxPosX[0] : stageBoxPosX[1]));
        }
        else
        {
            soundMan.NoTapSE();
            moveArrow = false;
            foreach (GameObject arrowObj in arrow)
            {
                arrowObj.SetActive(false);
            }
        }
    }

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

    IEnumerator PromptTutorial()
    {
        float oneFrameTime = 0.02f;
        float moveSpeed = 0.0f;
        bool firstMoveEnd = false;
        int targetIndex = 1;

        while (hand.activeSelf)
        {
            moveSpeed += oneFrameTime;
            handTra.anchoredPosition = Vector2.Lerp(handTra.anchoredPosition, targetPos[targetIndex], moveSpeed);

            if (!firstMoveEnd && handTra.anchoredPosition.y - 1 <= targetPos[targetIndex].y)
            {
                firstMoveEnd = true;
                moveSpeed = 0.0f;
                targetIndex = 0;
            }
            else if (firstMoveEnd && handTra.anchoredPosition.y + 1 >= targetPos[targetIndex].y)
            {
                firstMoveEnd = false;
                moveSpeed = 0.0f;
                targetIndex = 1;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }
    }

    IEnumerator StageDisplay(float posX)
    {
        displayPageNum = (posX == stageBoxPosX[0]) ? 0 : 1;
        if (displayStageNum <= maxDisplay)
        {
            moveArrow = false;
        }
        else
        {
            moveArrow = true;
            switch (displayPageNum)
            {
                case 0:
                    arrow[0].SetActive(false);
                    arrow[1].SetActive(true);
                    break;
                case 1:
                    arrow[0].SetActive(true);
                    arrow[1].SetActive(false);
                    break;
            }
        }
        float oneFrameTime = 0.02f;
        float stagePosX = stageTra.anchoredPosition.x;
        while ((stagePosX <= posX - 1 || stagePosX >= posX + 1) && !stageScroll)
        {
            stageTra.anchoredPosition = Vector2.Lerp(stageTra.anchoredPosition, new Vector2(posX, 0.0f), 0.3f);
            yield return new WaitForSeconds(oneFrameTime);
            stagePosX = stageTra.anchoredPosition.x;
        }
    }

    void FixedUpdate()
    {
        if (stageSelect && displayStageNum > maxDisplay)
        {
            //ステージ選択時のスクロール動作
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
            if (stageScroll)
            {
                float posX = Input.mousePosition.x * Magnification - DifferenceX - tapStartPosX + stageBoxPosX[displayPageNum];
                stageTra.anchoredPosition = Vector2.Lerp(stageTra.anchoredPosition, new Vector2(posX, 0.0f), 1);
                if (stageTra.anchoredPosition.x > stageBoxPosX[0])
                    stageTra.anchoredPosition = Vector2.Lerp(stageTra.anchoredPosition, new Vector2(stageBoxPosX[0], 0.0f), 0.9f);
                else if (stageTra.anchoredPosition.x < stageBoxPosX[1])
                    stageTra.anchoredPosition = Vector2.Lerp(stageTra.anchoredPosition, new Vector2(stageBoxPosX[1], 0.0f), 0.9f);
            }
            if (stageScroll && Input.GetMouseButtonUp(0))
            {
                stageScroll = false;
                StartCoroutine(StageDisplay((stageTra.anchoredPosition.x > stageBoxPosX[1] / 2.0f) ? stageBoxPosX[0] : stageBoxPosX[1]));
            }

            if (moveArrow)
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
                if (arrow[0].activeSelf) arrowTra[0].localScale = new Vector2(scale, scale);
                if (arrow[1].activeSelf) arrowTra[1].localScale = new Vector2(scale, scale);
            }
        }
    }
}
