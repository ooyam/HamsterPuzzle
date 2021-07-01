using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PanelManager : MonoBehaviour
{
    [Header("PanelBoxの取得")]
    public RectTransform PabnelBoxTra;
    [Header("HamsterBoxの取得")]
    public RectTransform HamsterBoxTra;
    [Header("野菜パネルの取得")]
    public GameObject[] VegetablePanel;
    private int VegetableTypeNum;
    [Header("ハムスターパネルの取得")]
    public GameObject HamsterPanelPre;
    [Header("チュートリアル")]
    public GameObject tutorialObj;
    private TutorialController tutorialCon;

    private CalorieGauge calGauge;   //カロリーゲージ
    private TurnController TurnCon;  //TurnController取得
    private ScoreManager ScoreMan;   //ScoreManager取得

    private Camera CameraMain;             //メインカメラの取得
    private float ScreenWidth = 1080.0f;   //画面幅
    private float ScreenHight = 1920.0f;   //画面高さ

    private int PanelColumns = 6;          //パネルの列数
    private int PanelLines = 5;            //パネルの行数
    [System.NonSerialized]
    public int PanelNum;                   //パネルの総数
    [System.NonSerialized]
    public bool tutorial = false;          //チュートリアル?

    private GameObject HamsterPanel;                  //生成ハムスターパネル
    private HamsterPanelController HamsterPanelScr;   //生成ハムスターパネルスクリプト
    private RectTransform HamsterPanelTra;            //生成野菜パネルのRectTransformリスト
    private GameObject[] PanelList;                   //生成野菜パネルのリスト
    private string[] PanelListTag;                    //生成野菜パネルのタグリスト
    private RectTransform[] PanelListTra;             //生成野菜パネルのRectTransformリスト
    private VegetablePanelController[] PanelListScr;  //生成野菜パネルのスクリプトリスト
    [System.NonSerialized]
    public Vector2[] PanelPosList;           //生成パネルの生成位置
    private int[] PanelPosNum;               //生成パネルの生成位置番号
    [System.NonSerialized]
    public bool HamsterMoving;               //ハムスター移動中？
    [System.NonSerialized]
    public bool HamsterPosChange;            //ハムスター動いた？
    private int NowHamsterPosIndex;          //現在のハムスターパネル位置番号
    private bool SecondGanerateContinue;     //二回目以降のパネル生成やり直し判定

    private int HarvestNum = 3;                       //収穫に必要な本数
    private List<int> HarvestIndex = new List<int>(); //収穫リスト
    private int HarvestCompNum = 0;                   //収穫完了個数
    [System.NonSerialized]
    public bool gameOver = false;                     //ゲームオーバー？
    private bool calorieZero = false;                 //カロリー無くなった？
    [System.NonSerialized]
    public bool gameClear = false;                    //ゲームクリア？

    private string Broccoli = "Broccoli"; //ブロッコリー
    private string Cabbage = "Cabbage";   //キャベツ
    private string Paprika = "Paprika";   //パプリカ
    private string Carrot = "Carrot";     //ニンジン

    // Start is called before the first frame update
    void Start()
    {
        CameraMain = Camera.main;
        TurnCon = GameObject.FindWithTag("Turn").GetComponent<TurnController>();
        ScoreMan = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        calGauge = GameObject.FindWithTag("CalorieGauge").GetComponent<CalorieGauge>();
        PanelNum = PanelLines * PanelColumns;
        PanelList = new GameObject[PanelNum];
        PanelListTag = new string[PanelNum];
        PanelListTra = new RectTransform[PanelNum];
        PanelPosList = new Vector2[PanelNum];
        PanelPosNum = new int[PanelNum];
        PanelListScr = new VegetablePanelController[PanelNum];
        VegetableTypeNum = VegetablePanel.Length;
        tutorial = PuzzleMainController.tutorial;
        if (tutorial)
        {
            tutorialObj.SetActive(true);
            tutorialCon = tutorialObj.GetComponent<TutorialController>();
        }

        StartCoroutine(PanelGeneration(true));
    }

    void Update()
    {
        if (HamsterMoving)
        {
            Ray ray = new Ray(HamsterPanelTra.position, HamsterPanelTra.forward);
            RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
            if (hit2d)
            {
                Transform hit2dTra = hit2d.transform;
                if (hit2dTra.gameObject.tag == Broccoli ||
                    hit2dTra.gameObject.tag == Cabbage ||
                    hit2dTra.gameObject.tag == Paprika ||
                    hit2dTra.gameObject.tag == Carrot)
                {
                    PanelPosChange(Array.IndexOf(PanelList, hit2dTra.gameObject));
                }
            }
        }
    }

    //パネル生成
    private IEnumerator PanelGeneration(bool FirstGaneration)
    {
        if (FirstGaneration)
        {
            float PosX = ScreenWidth / PanelColumns;
            float PosFixX = PosX * 2.5f;
            float PosY = (ScreenHight / PanelLines) * 0.45f;
            float PosFixY = 70.0f;

            int hamsterNum = (tutorial) ? 8 : UnityEngine.Random.Range(0, PanelNum);

            int[] tutorialBroccoliPosIndex = new int[] { 9, 10, 14 };
            int[] tutorialCarrotPosIndex = new int[] { 1, 7, 12, 19, 25 };

            for (int i = 0; i < PanelNum; i++)
            {
                if (i < PanelColumns) PanelPosList[i] = new Vector2((PosX * i) - PosFixX, PosY * 2.0f - PosFixY);
                else if (i < PanelColumns * 2) PanelPosList[i] = new Vector2((PosX * (i - PanelColumns)) - PosFixX, PosY - PosFixY);
                else if (i < PanelColumns * 3) PanelPosList[i] = new Vector2((PosX * (i - PanelColumns * 2)) - PosFixX, -PosFixY);
                else if (i < PanelColumns * 4) PanelPosList[i] = new Vector2((PosX * (i - PanelColumns * 3)) - PosFixX, -PosY - PosFixY);
                else PanelPosList[i] = new Vector2((PosX * (i - PanelColumns * 4)) - PosFixX, -PosY * 2.0f - PosFixY);
                PanelPosNum[i] = i;

                if (i == hamsterNum)
                {
                    PanelArrangement(hamsterNum, true);
                }
                else
                {
                    while (true)
                    {
                        int UseVegetableType = -1;
                        if (tutorial && 0 <= Array.IndexOf(tutorialBroccoliPosIndex, i))
                        {
                            UseVegetableType = 0;
                        }
                        else if (tutorial && 0 <= Array.IndexOf(tutorialCarrotPosIndex, i))
                        {
                            UseVegetableType = 3;
                            if (i != tutorialCarrotPosIndex[2])
                            {
                                int frontPanelIndex = i - 1;
                                Destroy(PanelList[frontPanelIndex]);
                                PanelList[frontPanelIndex] = Instantiate(VegetablePanel[UnityEngine.Random.Range(0, VegetableTypeNum - 1)]);
                                PanelListTag[frontPanelIndex] = PanelList[frontPanelIndex].tag;
                                PanelArrangement(frontPanelIndex, false);
                            }
                        }
                        else
                        {
                            UseVegetableType = UnityEngine.Random.Range(0, VegetableTypeNum);
                        }

                        PanelList[i] = Instantiate(VegetablePanel[UseVegetableType]);
                        PanelListTag[i] = PanelList[i].tag;

                        if (OrderDecision(i, i - 1, i - 2) && OrderDecision(i, i - PanelColumns, i - 2 * PanelColumns)) break;
                        else
                        {
                            Destroy(PanelList[i]);
                            continue;
                        }
                    }
                    PanelArrangement(i, false);
                }
            }
        }
        else
        {
            yield return null;
            NowHamsterPosIndex = HamsterPanelScr.HamPosNum;
            for (int i = 0; i < PanelNum; i++)
            {
                if(PanelList[i] == null && i != NowHamsterPosIndex)
                {
                    while (true)
                    {
                        SecondGanerateContinue = false;
                        int UseVegetableType = UnityEngine.Random.Range(0, VegetableTypeNum);
                        PanelList[i] = Instantiate(VegetablePanel[UseVegetableType]);
                        PanelListTag[i] = PanelList[i].tag;
                        HarvestDecision(i, true);
                        if (!SecondGanerateContinue) break;
                        else
                        {
                            Destroy(PanelList[i]);
                            continue;
                        }
                    }
                    PanelArrangement(i, false);
                }
            }
            HamsterPanelScr.Harvest = false;
            HarvestCompNum = 0;
        }
    }
    //上下左右3列並んでいないか判断
    private bool OrderDecision(int Dependent_1, int Dependent_2, int Dependent_3)
    {
        if (Dependent_2 >= 0 && Dependent_3 >= 0 && Dependent_2 < PanelNum && Dependent_3 < PanelNum)
        {
            if (PanelListTag[Dependent_1] == PanelListTag[Dependent_2] && PanelListTag[Dependent_1] == PanelListTag[Dependent_3])
                return false;
            else return true;
        }
        else return true;
    }
    //パネル位置指定
    private void PanelArrangement(int posIndex, bool hamster)
    {
        if (hamster)
        {
            HamsterPanel = Instantiate(HamsterPanelPre);
            HamsterPanelTra = HamsterPanel.GetComponent<RectTransform>();
            HamsterPanelTra.position = PanelPosList[posIndex];
            HamsterPanelTra.SetParent(HamsterBoxTra, false);
            HamsterPanelScr = HamsterPanel.GetComponent<HamsterPanelController>();
            HamsterPanelScr.PanelPosChange(posIndex);
            TurnCon.hamsterPanelImage = HamsterPanel.GetComponent<Image>();
        }
        else
        {
            PanelListTra[posIndex] = PanelList[posIndex].GetComponent<RectTransform>();
            PanelListTra[posIndex].position = PanelPosList[posIndex];
            PanelListTra[posIndex].SetParent(PabnelBoxTra, false);
            PanelListScr[posIndex] = PanelList[posIndex].GetComponent<VegetablePanelController>();
            PanelListScr[posIndex].PanelPosChange(posIndex);
        }
    }


    //ハムスターが離された時
    public void HamsterRelease()
    {
        HamsterMoving = false;
        HarvestIndex.Clear();

        for (int i = 0; i < PanelNum; i++)
        { HarvestDecision(i, false); }

        int harvestIndexCount = HarvestIndex.Count;
        bool reduceTurn = true;
        //体力回復判定(縦一列)
        if (harvestIndexCount >= PanelLines)
        {
            for (int a = 0; a < PanelColumns; a++)
            {
                string panelTag = PanelListTag[a];
                for (int i = 0; i < PanelLines; i++)
                {
                    if (!(HarvestIndex.Contains(i * PanelColumns + a) && panelTag == PanelListTag[i * PanelColumns + a]))
                        break;
                    else if (i == PanelLines - 1)
                        calGauge.VegetableHarvest(true);
                }
            }

            //ターン回復判定(横一列)
            if (harvestIndexCount >= PanelColumns)
            {
                for (int b = 0; b < PanelLines; b++)
                {
                    int posNum = PanelColumns * b;
                    string panelTag = PanelListTag[posNum];
                    for (int i = 0; i < PanelColumns; i++)
                    {
                        if (!(HarvestIndex.Contains(posNum + i) && panelTag == PanelListTag[posNum + i]))
                            break;
                        else if (i == PanelColumns - 1)
                        {
                            reduceTurn = false;
                            TurnCon.TurnRecovery();
                        }
                    }
                }
            }
        }

        if (HamsterPosChange)
        {
            TurnCon.TurnCalculation(reduceTurn);
            HamsterPosChange = false;
        }

        if (harvestIndexCount > 0)
        {
            if (calorieZero) calorieZero = false;
            HamsterPanelScr.Harvest = true;
            TurnCon.HamsterSpriteChange(3);
        }
        else if (gameOver || calorieZero)
        {
            HamsterPanelScr.gameOver = true;
            TurnCon.effectStart = true;
            if (calorieZero)
            {
                TurnCon.HamsterSpriteChange(2);
                StartCoroutine(TurnCon.GameOver());
            }
        }
        foreach (int i in HarvestIndex)
        {
            PanelListTra[i].SetSiblingIndex(PanelNum - 1);
            PanelListScr[i].Harvest();
        }
    }

    //収穫判定
    private void HarvestDecision(int ListIndex, bool SecondGaneration)
    {
        bool[] Right = new bool[] { false, false };
        bool[] Left = new bool[] { false, false };
        bool[] Top = new bool[] { false, false };
        bool[] Bottom = new bool[] { false, false };

        for (int i = 0; i < HarvestNum - 1; i++)
        {
            if (((ListIndex + i) + 1) % PanelColumns == 0)
                Right[i] = true;
            if ((ListIndex - i) % PanelColumns == 0)
                Left[i] = true;
            if (PanelColumns > ListIndex - i * PanelColumns)
                Top[i] = true;
            if (PanelNum - PanelColumns <= ListIndex + i * PanelColumns)
                Bottom[i] = true;
        }

        string[] ReferenceTag = new string[9];
        ReferenceTag[0] = PanelListTag[ListIndex];
        if (!(ListIndex + 1 < 0 || ListIndex + 1 >= PanelNum))
            ReferenceTag[1] = PanelListTag[ListIndex + 1];
        if (!(ListIndex + 2 < 0 || ListIndex + 2 >= PanelNum))
            ReferenceTag[2] = PanelListTag[ListIndex + 2];
        if (!(ListIndex - 1 < 0 || ListIndex - 1 >= PanelNum))
            ReferenceTag[3] = PanelListTag[ListIndex - 1];
        if (!(ListIndex - 2 < 0 || ListIndex - 2 >= PanelNum))
            ReferenceTag[4] = PanelListTag[ListIndex - 2];
        if (!(ListIndex - PanelColumns < 0 || ListIndex - PanelColumns >= PanelNum))
            ReferenceTag[5] = PanelListTag[ListIndex - PanelColumns];
        if (!(ListIndex - 2 * PanelColumns < 0 || ListIndex - 2 * PanelColumns >= PanelNum))
            ReferenceTag[6] = PanelListTag[ListIndex - 2 * PanelColumns];
        if (!(ListIndex + PanelColumns < 0 || ListIndex + PanelColumns >= PanelNum))
            ReferenceTag[7] = PanelListTag[ListIndex + PanelColumns];
        if (!(ListIndex + 2 * PanelColumns < 0 || ListIndex + 2 * PanelColumns >= PanelNum))
            ReferenceTag[8] = PanelListTag[ListIndex + 2 * PanelColumns];

        while (true)
        {
            if (!Right[0] && !Right[1])
            {
                if (ReferenceTag[0] == ReferenceTag[1] &&
                    ReferenceTag[0] == ReferenceTag[2])
                {
                    if (!SecondGaneration)
                    {
                        for (int i = 0; i < HarvestNum; i++)
                        {
                            if (!HarvestIndex.Contains(ListIndex + i))
                                HarvestIndex.Add(ListIndex + i);
                        }
                    }
                    else
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
            }

            if (!Left[0] && !Left[1])
            {
                if (ReferenceTag[0] == ReferenceTag[3] &&
                    ReferenceTag[0] == ReferenceTag[4])
                {
                    if (!SecondGaneration)
                    {
                        for (int i = 0; i < HarvestNum; i++)
                        {
                            if (!HarvestIndex.Contains(ListIndex - i))
                                HarvestIndex.Add(ListIndex - i);
                        }
                    }
                    else
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
            }

            if (!Top[0] && !Top[1])
            {
                if (ReferenceTag[0] == ReferenceTag[5] &&
                    ReferenceTag[0] == ReferenceTag[6])
                {
                    if (!SecondGaneration)
                    {
                        for (int i = 0; i < HarvestNum; i++)
                        {
                            if (!HarvestIndex.Contains(ListIndex - i * PanelColumns))
                                HarvestIndex.Add(ListIndex - i * PanelColumns);
                        }
                    }
                    else
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
            }

            if (!Bottom[0] && !Bottom[1])
            {
                if (ReferenceTag[0] == ReferenceTag[7] &&
                    ReferenceTag[0] == ReferenceTag[8])
                {
                    if (!SecondGaneration)
                    {
                        for (int i = 0; i < HarvestNum; i++)
                        {
                            if (!HarvestIndex.Contains(ListIndex + i * PanelColumns))
                                HarvestIndex.Add(ListIndex + i * PanelColumns);
                        }
                    }
                    else
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
            }

            if (SecondGaneration)
            {
                if (!Right[0] && !Left[0])
                {
                    if (ReferenceTag[0] == ReferenceTag[1] &&
                        ReferenceTag[0] == ReferenceTag[3])
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
                if (!Top[0] && !Bottom[0])
                {
                    if (ReferenceTag[0] == ReferenceTag[5] &&
                        ReferenceTag[0] == ReferenceTag[7])
                    {
                        SecondGanerateContinue = true;
                        break;
                    }
                }
            }
            break;
        }
    }
    //収穫完了判定
    public void HarvestComplete(string VegetableTag)
    {
        HarvestCompNum++;
        calGauge.VegetableHarvest(false);
        ScoreMan.HarvestVegetable(VegetableTag);
        if (HarvestCompNum == HarvestIndex.Count)
        {
            if(gameClear)
            {
                HamsterPanelScr.gameClear = true;
                TurnCon.HamsterSpriteChange(4);
            }
            else
            {
                if (gameOver)
                {
                    HamsterPanelScr.gameOver = true;
                    TurnCon.effectStart = true;
                }
                else StartCoroutine(PanelGeneration(false));
                TurnCon.HamsterSpriteChange(-1);
            }
            if (tutorial)
            {
                StartCoroutine(tutorialCon.HarvestComplete());
            }
        }
    }

    //パネル入れ替わり
    private void PanelPosChange(int ReferencePosNumber)
    {
        HamsterPosChange = true;
        NowHamsterPosIndex = HamsterPanelScr.HamPosNum;

        PanelListScr[ReferencePosNumber].PanelPosChange(NowHamsterPosIndex);
        PanelListTra[ReferencePosNumber].anchoredPosition = PanelPosList[NowHamsterPosIndex];
        PanelList[NowHamsterPosIndex] = PanelList[ReferencePosNumber];
        PanelListTag[NowHamsterPosIndex] = PanelListTag[ReferencePosNumber];
        PanelListTra[NowHamsterPosIndex] = PanelListTra[ReferencePosNumber];
        PanelListScr[NowHamsterPosIndex] = PanelListScr[ReferencePosNumber];
        PanelList[ReferencePosNumber] = null;
        PanelListTag[ReferencePosNumber] = null;
        PanelListTra[ReferencePosNumber] = null;
        PanelListScr[ReferencePosNumber] = null;

        HamsterPanelScr.PanelPosChange(ReferencePosNumber);
        HamsterPanelTra.anchoredPosition = PanelPosList[ReferencePosNumber];

        if (calGauge.HamsterMoved())
        {
            calorieZero = true;
            HamsterPanelScr.ReleasePanel();
        }
    }
}
