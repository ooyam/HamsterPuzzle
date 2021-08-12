using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
            TutorialTra.GetChild(0).gameObject.GetComponent<Text>().fontSize = 80;
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
        pouzzleStageButton[0].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 6, 5 }, 50, 0));
        pouzzleStageButton[1].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot }, 1, new int[] { 6 }, 20, 1));
        pouzzleStageButton[2].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Paprika }, 2, new int[] { 8, 8 }, 20, 2));
        pouzzleStageButton[3].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage }, 1, new int[] { 15 }, 10, 3));
        pouzzleStageButton[4].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Paprika, TargetVegetable.Cabbage }, 3, new int[] { 10, 10, 20 }, 15, 4));
        pouzzleStageButton[5].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Paprika, TargetVegetable.Broccoli }, 3, new int[] { 20, 30, 35 }, 25, 5));
        pouzzleStageButton[6].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 30, 20 }, 13, 6));
        pouzzleStageButton[7].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Carrot }, 3, new int[] { 25, 25, 25 }, 15, 7));
        pouzzleStageButton[8].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Paprika }, 1, new int[] { 30 }, 8, 8));
        pouzzleStageButton[9].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Paprika, TargetVegetable.Cabbage, TargetVegetable.Broccoli, TargetVegetable.Pumpkin }, 4, new int[] { 25, 25, 25, 25 }, 20, 9));
        pouzzleStageButton[10].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot, TargetVegetable.Cabbage, TargetVegetable.Pumpkin }, 3, new int[] { 10, 10, 10 }, 5, 10));
        pouzzleStageButton[11].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Broccoli, TargetVegetable.Carrot }, 2, new int[] { 50, 50 }, 20, 11));
        pouzzleStageButton[12].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin }, 1, new int[] { 80 }, 30, 12));
        pouzzleStageButton[13].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Corn, TargetVegetable.Cabbage, TargetVegetable.Paprika }, 4, new int[] { 40, 40, 40, 40 }, 20, 13));
        pouzzleStageButton[14].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Corn, TargetVegetable.Pumpkin }, 2, new int[] { 20, 20 }, 5, 14));
        pouzzleStageButton[15].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Pumpkin, TargetVegetable.Broccoli, TargetVegetable.Cabbage, TargetVegetable.Corn }, 4, new int[] { 35, 50, 35, 50 }, 10, 15));
        pouzzleStageButton[16].onClick.AddListener(() =>
        OnClickGameStart(new TargetVegetable[] { TargetVegetable.Carrot }, 1, new int[] { 50 }, 2, 16));

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
        int stageTotal = pouzzleStageObject.Length;
        int loopTimes = (displayStageNum >= stageTotal) ? stageTotal : displayStageNum;
        for (int i = 0; i < loopTimes + 1; i++)
        {
            pouzzleStageObject[i].SetActive(selectMode);
        }
        selectBuckObject.SetActive(selectMode);
        if (selectMode)
        {
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
            float waitTime = Random.Range(5.0f, 12.0f);
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
        soundMan.YesTapSE();
        displayPageNum = (posX == stageBoxPosX[0]) ? 0 : 1;
        if (displayStageNum <= maxDisplay)
        {
            moveArrow = false;
        }
        else if (displayStageNum <= maxDisplay * 2)
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
                if (!hit2d || hit2d.transform.gameObject.tag != "Button")
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
