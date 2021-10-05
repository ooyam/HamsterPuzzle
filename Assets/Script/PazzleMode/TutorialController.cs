using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GFramework; //SimpleRoundedImageを使用する
using static MoveFunction.ObjectMove;

public class TutorialController : MonoBehaviour
{
    [Header("フィルター取得")]
    public GameObject[] filter;
    [Header("フィルター前格納ボックス")]
    public GameObject frontBoxObj;
    [System.NonSerialized]
    public Transform frontBoxTra;
    [Header("テキストボックス")]
    public GameObject[] textBox;
    private GameObject textObj;
    private Transform textTra;
    private Text textText;
    private Image textIma;
    private SimpleRoundedImage hintIma;
    [Header("テキスト格納ボックス")]
    public Transform textBoxTra;
    [Header("ステータスボード")]
    public Transform statusBoardTra;
    [Header("手取得")]
    public GameObject hand;
    private RectTransform handTra;
    private Image handIma;
    [Header("カロリーゲージ")]
    public Transform calGaugeTra;
    [Header("ターン表示")]
    public Transform turnTra;
    [Header("ターゲット表示")]
    public Transform targetTra;
    [Header("収穫リスト")]
    public Transform harvestTra;

    private HamsterPanelController HamsterCon;   //ハムスターパネルスクリプト
    private PanelManager PanelMan;               //PanelManger
    private SoundManager SoundMan;               //SoundManager
    [System.NonSerialized]
    public int tupNum = 0;               //タップ回数
    private bool waiting = false;        //待機中？
    private bool description = false;    //説明中？
    private float displayTime = 2.0f;    //説明の最低表示時間
    [System.NonSerialized]
    public bool ColDescription = false;  //体力説明完了？
    private bool imageFade = false;      //イメージをフェードする？
    private bool textIndexTen = false;   //テキスト番号10？
    private float textTenColor = 180.0f / 255.0f;  //テキスト番号10の色のG値

    private bool textDestroy = false;    //テキスト消去中？
    private bool textDisplay = false;    //テキスト表示途中？
    private float[] textColAlpha = new float[] { 1.0f, 0.0f };   //テキストのアルファ値

    //手の位置 0･1:オラフ移動 2:体力ゲージ 3:ターン 4:目標
    private Vector2[] handStartPos =
        new Vector2[] {
            new Vector2(0.0f, -30.0f),
            new Vector2(0.0f, -200.0f),
            new Vector2(100.0f, -600.0f),
            new Vector2(475.0f, 580.0f),
            new Vector2(400.0f, 770.0f)
        };
    private Vector2[] handEndPos =
        new Vector2[] {
            new Vector2(0.0f, -200.0f),
            new Vector2(-360.0f, -200.0f),
            new Vector2(100.0f, -650.0f),
            new Vector2(475.0f, 530.0f),
            new Vector2(350.0f, 770.0f)
        };
    private Color[] handColor = new Color[] { new Color(1, 1, 1, 1), new Color(1, 1, 1, 0) };

    void Start()
    {
        Time.timeScale = 1.0f;
        PanelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        SoundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        frontBoxTra = frontBoxObj.transform;
        handTra = hand.GetComponent<RectTransform>();
        handIma = hand.GetComponent<Image>();
        TextDisplay(0);
        frontBoxObj.SetActive(true);
        StartCoroutine(DescriptionStart());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!description && !waiting && !textDestroy)
            {
                tupNum++;
                StartCoroutine(NextDescription(tupNum));
            }
        }
    }

    //========================================================================
    //次の説明
    //========================================================================
    //descriptionNum;   説明番号
    //========================================================================
    IEnumerator NextDescription(int descriptionNum)
    {
        changeEnd = false;
        switch (descriptionNum)
        {
            case 1:
                SoundMan.YesTapSE();
                frontBoxObj.SetActive(false);
                FilterDisplay(0);
                StartCoroutine(TextDestroy(true));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                hand.SetActive(true);
                StartCoroutine(HandMove(0));
                HamsterCon.description = true;
                TextDisplay(1);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 2:
            case 6:
                SoundMan.YesTapSE();
                HamsterCon.description = false;
                hand.SetActive(false);
                description = true;
                break;
            case 3:
                SoundMan.YesTapSE();
                TimeScaleChange(1.0f);
                FilterDestroy(1);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                description = true;
                break;
            case 4:
                SoundMan.YesTapSE();
                calGaugeTra.SetParent(statusBoardTra, true);
                turnTra.SetParent(frontBoxTra, true);
                StartCoroutine(HandMove(3));
                handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                StartCoroutine(DescriptionStart());
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                FilterDestroy(1);
                TextDisplay(4);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 5:
                SoundMan.YesTapSE();
                turnTra.SetParent(statusBoardTra, true);
                hand.SetActive(true);
                StartCoroutine(HandMove(1));
                HamsterCon.MovingLimit(false);
                HamsterCon.description = true;
                frontBoxObj.SetActive(false);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                FilterDisplay(2);
                TextDisplay(5);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 7:
                SoundMan.YesTapSE();
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                TextDisplay(7);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 8:
                SoundMan.YesTapSE();
                TimeScaleChange(1.0f);
                FilterDestroy(3);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                description = true;
                break;
            case 9:
                SoundMan.YesTapSE();
                targetTra.SetParent(statusBoardTra, true);
                harvestTra.SetParent(statusBoardTra, true);
                hand.SetActive(false);
                StartCoroutine(DescriptionStart());
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                TextDisplay(9);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 10:
                SoundMan.YesTapSE();
                StartCoroutine(DescriptionStart());
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                TextDisplay(10);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 11:
                SoundMan.YesTapSE();
                StartCoroutine(DescriptionStart());
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                TextDisplay(11);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 12:
                SoundMan.YesTapSE();
                HamsterCon.tutorial = false;
                HamsterCon.MovingLimit(false);
                PanelMan.tutorial = false;
                this.gameObject.SetActive(false);
                break;
        }

    }

    //========================================================================
    //手フェード
    //========================================================================
    //posIndex;   指定座標番号
    //========================================================================
    IEnumerator HandMove(int posIndex)
    {
        int nowTapNum = tupNum;
        float oneFrameTime = 0.02f;
        bool firstMoveEnd = false;
        float moveSpeed = 0.0f;
        float handColAlpha = 1.0f;
        handIma.color = new Color(1, 1, 1, handColAlpha);
        bool fade = (posIndex <= 1) ? true : false;
        float moveSpeedFix = (fade) ? 1.5f : 1.0f;
        float handPos = 0.0f;
        float comparisonPos = 0.0f;
        Vector2 targetPos = handEndPos[posIndex];
        handTra.anchoredPosition = handStartPos[posIndex];

        while (nowTapNum == tupNum)
        {
            if (!firstMoveEnd || !fade)
            {
                moveSpeed += oneFrameTime / moveSpeedFix;
                handTra.anchoredPosition = Vector2.Lerp(handTra.anchoredPosition, targetPos, moveSpeed);
                switch (posIndex)
                {
                    case 1:
                    case 4:
                        handPos = handTra.anchoredPosition.x;
                        comparisonPos = targetPos.x;
                        break;
                    default:
                        handPos = handTra.anchoredPosition.y;
                        comparisonPos = targetPos.y;
                        break;
                }

                if (!firstMoveEnd && handPos - 1 <= comparisonPos)
                {
                    firstMoveEnd = true;
                    moveSpeed = 0.0f;
                    if (!fade) targetPos = handStartPos[posIndex];
                }
                else if (firstMoveEnd && handPos + 1 >= comparisonPos)
                {
                    firstMoveEnd = false;
                    moveSpeed = 0.0f;
                    targetPos = handEndPos[posIndex];
                }
            }
            else
            {
                handColAlpha -= oneFrameTime * 2.5f;
                handIma.color = new Color(1, 1, 1, handColAlpha);
                if (handColAlpha <= 0.0f)
                {
                    firstMoveEnd = false;
                    handColAlpha = 1.0f;
                    handIma.color = new Color(1, 1, 1, handColAlpha);
                    handTra.anchoredPosition = handStartPos[posIndex];
                }
            }
            yield return new WaitForSecondsRealtime(oneFrameTime);
        }
        handIma.color = new Color(1, 1, 1, 1);
    }

    //========================================================================
    //テキスト表示
    //========================================================================
    //textIndex;   テキスト番号
    //========================================================================
    public void TextDisplay(int textIndex)
    {
        textColAlpha[1] = 0.0f;
        textObj = Instantiate(textBox[textIndex]);
        textTra = textObj.transform;
        textText = textTra.GetChild(0).gameObject.GetComponent<Text>();
        textText.color = new Color(0, 0, 0, 0);
        if(textIndex == 8 || textIndex == 10 || textIndex == 11)
        {
            imageFade = true;
            if (textIndex == 10)
            {
                textIndexTen = true;
                hintIma = textTra.GetChild(2).gameObject.GetComponent<SimpleRoundedImage>();
                hintIma.color = new Color(1, textTenColor, 0, 0);
            }
            textIma = textTra.GetChild(1).gameObject.GetComponent<Image>();
            textIma.color = new Color(1, 1, 1, 0);
        }
        textTra.SetParent(textBoxTra, false);
        textDisplay = true;
        StartCoroutine(TextFade());
    }

    //========================================================================
    //テキスト消去
    //========================================================================
    //firstDes;   初関数起動？
    //========================================================================
    public IEnumerator TextDestroy(bool firstDes)
    {
        if (firstDes) HamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterPanelController>();
        textColAlpha[0] = 1.0f;
        textDestroy = true;
        StartCoroutine(TextFade());
        yield return new WaitWhile(() => textDestroy == true);
        if (textObj != null) Destroy(textObj);
    }

    //========================================================================
    //テキストフェード
    //========================================================================
    IEnumerator TextFade()
    {
        float oneFrameTime = 0.02f;
        while (true)
        {
            if (textDestroy)
            {
                textColAlpha[0] -= oneFrameTime * 2.5f;
                if (textObj != null)
                {
                    textText.color = new Color(0, 0, 0, textColAlpha[0]);
                    if (imageFade) textIma.color = new Color(1, 1, 1, textColAlpha[0]);
                    if (textIndexTen) hintIma.color = new Color(1, textTenColor, 0, textColAlpha[0]);
                }
                if (textColAlpha[0] <= 0.0f)
                {
                    textDestroy = false;
                    break;
                }
            }
            if (textDisplay)
            {
                textColAlpha[1] += oneFrameTime * 2.5f;
                if (textObj != null)
                {
                    textText.color = new Color(0, 0, 0, textColAlpha[1]);
                    if (imageFade) textIma.color = new Color(1, 1, 1, textColAlpha[1]);
                    if (textIndexTen) hintIma.color = new Color(1, textTenColor, 0, textColAlpha[1]);
                }
                if (textColAlpha[1] >= 1.0f)
                {
                    textDestroy = false;
                    if (imageFade) imageFade = false;
                    if (textIndexTen) textIndexTen = false;
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(oneFrameTime);
        }
    }

    //========================================================================
    //フィルター表示
    //========================================================================
    //filterIndex;   フィルター番号
    //========================================================================
    public void FilterDisplay(int filterIndex)
    {
        filter[filterIndex].SetActive(true);
    }

    //========================================================================
    //フィルター消去
    //========================================================================
    //filterIndex;   フィルター番号
    //========================================================================
    public void FilterDestroy(int filterIndex)
    {
        filter[filterIndex].SetActive(false);
    }

    //========================================================================
    //説明開始
    //========================================================================
    public IEnumerator DescriptionStart()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(displayTime);
        waiting = false;
    }

    //========================================================================
    //時間変更
    //========================================================================
    //timeScale;   時間の指定
    //========================================================================
    void TimeScaleChange(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    //========================================================================
    //収穫完了    
    //========================================================================
    public IEnumerator HarvestComplete()
    {
        if (!ColDescription)
        {
            description = false;
            frontBoxObj.SetActive(true);
            hand.SetActive(true);
            handTra.rotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);
            StartCoroutine(HandMove(2));
            StartCoroutine(DescriptionStart());
            calGaugeTra.SetParent(frontBoxTra, true);
            ColDescription = true;
            TextDisplay(3);
            yield return new WaitWhile(() => textDisplay == true);
        }
        else
        {
            description = false;
            frontBoxObj.SetActive(true);
            targetTra.SetParent(frontBoxTra, true);
            harvestTra.SetParent(frontBoxTra, true);
            hand.SetActive(true);
            handTra.rotation = Quaternion.Euler(180.0f, 0.0f, 90.0f);
            StartCoroutine(HandMove(4));
            StartCoroutine(DescriptionStart());
            TextDisplay(8);
            yield return new WaitWhile(() => textDisplay == true);
        }
    }

    //========================================================================
    //ハムスター移動完了
    //========================================================================
    //textIndex;   テキスト番号
    //filterIndex; フィルター番号
    //========================================================================
    public IEnumerator HamsterMovingComplete(int textIndex, int filterIndex)
    {
        description = false;
        StartCoroutine(TextDestroy(false));
        yield return new WaitWhile(() => textDestroy == true);
        StartCoroutine(DescriptionStart());
        TimeScaleChange(0.0f);
        FilterDestroy(filterIndex - 1);
        FilterDisplay(filterIndex);
        TextDisplay(textIndex);
        yield return new WaitWhile(() => textDisplay == true);
    }
}
