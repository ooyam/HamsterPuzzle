using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("テキスト格納ボックス")]
    public Transform textBoxTra;
    [Header("ステータスボード")]
    public Transform statusBoardTra;
    [Header("手取得")]
    public GameObject hand;
    private RectTransform handTra;
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
    [System.NonSerialized]
    public int tupNum = 0;               //タップ回数
    private bool waiting = false;        //待機中？
    private bool description = false;    //説明中？
    private float displayTime = 2.0f;    //説明の最低表示時間
    [System.NonSerialized]
    public bool ColDescription = false;  //体力説明完了？

    private bool textDestroy = false;    //テキスト消去中？
    private bool textDisplay = false;    //テキスト表示途中？
    private float[] textColAlpha = new float[] { 1.0f, 0.0f };   //テキストのアルファ値

    //手の位置
    private Vector2[] handStartPos = new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.0f, -170.0f) };
    private Vector2[] handEndPos = new Vector2[] { new Vector2(0.0f, -170.0f), new Vector2(-360.0f, -170.0f) };
    private Color[] handColor = new Color[] { new Color(1, 1, 1, 1), new Color(1, 1, 1, 0) };
    //0:体力ゲージ　1:ターン 　2:目標
    private Vector2[] handDesPos = 
        new Vector2[] { new Vector2(120.0f, 355.0f), new Vector2(475.0f, 500.0f), new Vector2(120.0f, 460.0f) };

    // Start is called before the first frame update
    void Start()
    {
        PanelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        frontBoxTra = frontBoxObj.transform;
        handTra = hand.GetComponent<RectTransform>();
        TextDisplay(0);
        frontBoxObj.SetActive(true);
        DescriptionStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!description)
            {
                if (!waiting)
                {
                    tupNum++;
                    StartCoroutine(NextDescription(tupNum));
                }
            }
        }
    }

    //次の説明
    IEnumerator NextDescription(int descriptionNum)
    {
        switch (descriptionNum)
        {
            case 1:
                frontBoxObj.SetActive(false);
                FilterDisplay(0);
                StartCoroutine(TextDestroy(true));
                yield return new WaitWhile(() => textDestroy == true);
                hand.SetActive(true);
                HamsterCon.description = true;
                TextDisplay(1);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 2:
            case 6:
                HamsterCon.description = false;
                hand.SetActive(false);
                description = true;
                break;
            case 3:
                TimeScaleChange(1.0f);
                FilterDestroy(1);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                description = true;
                break;
            case 4:
                calGaugeTra.SetParent(statusBoardTra, true);
                turnTra.SetParent(frontBoxTra, true);
                handTra.anchoredPosition = handDesPos[1];
                TimeScaleChange(1.0f);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                FilterDestroy(1);
                StartCoroutine(DescriptionStart()); ;
                TextDisplay(4);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 5:
                turnTra.SetParent(statusBoardTra, true);
                hand.SetActive(true);
                handTra.anchoredPosition = handStartPos[1];
                HamsterCon.MovingLimit(false);
                frontBoxObj.SetActive(false);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                FilterDisplay(2);
                TextDisplay(5);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 7:
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                TextDisplay(7);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 8:
                TimeScaleChange(1.0f);
                FilterDestroy(3);
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                description = true;
                break;
            case 9:
                StartCoroutine(TextDestroy(false));
                yield return new WaitWhile(() => textDestroy == true);
                StartCoroutine(DescriptionStart());
                TextDisplay(9);
                yield return new WaitWhile(() => textDisplay == true);
                break;
            case 10:
                targetTra.SetParent(statusBoardTra, true);
                harvestTra.SetParent(statusBoardTra, true);
                HamsterCon.tutorial = false;
                HamsterCon.MovingLimit(false);
                PanelMan.tutorial = false;
                this.gameObject.SetActive(false);
                break;
        }

    }

    //テキスト表示
    public void TextDisplay(int textIndex)
    {
        textColAlpha[1] = 0.0f;
        textObj = Instantiate(textBox[textIndex]);
        textTra = textObj.transform;
        textText = textTra.GetChild(0).gameObject.GetComponent<Text>();
        textText.color = new Color(0, 0, 0, 0);
        textTra.SetParent(textBoxTra, false);
        textDisplay = true;
        StartCoroutine(TextFade());
    }
    //テキスト消去
    public IEnumerator TextDestroy(bool firstDes)
    {
        if (firstDes) HamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterPanelController>();
        textColAlpha[0] = 1.0f;
        textDestroy = true;
        StartCoroutine(TextFade());
        waiting = true;
        yield return new WaitWhile(() => textDestroy == true);
        waiting = false;
        if (textObj != null) Destroy(textObj);
    }

    //テキストフェード
    IEnumerator TextFade()
    {
        float oneFlameTime = 0.02f;
        while (true)
        {
            if (textDestroy)
            {
                textColAlpha[0] -= oneFlameTime * 2;
                if (textObj != null) textText.color = new Color(0, 0, 0, textColAlpha[0]);
                if (textColAlpha[0] <= 0.0f)
                {
                    textDestroy = false;
                    break;
                }
            }
            if (textDisplay)
            {
                textColAlpha[1] += oneFlameTime * 2;
                if (textObj != null) textText.color = new Color(0, 0, 0, textColAlpha[1]);
                if (textColAlpha[1] >= 1.0f)
                {
                    textDestroy = false;
                    break;
                }
            }
            yield return new WaitForSecondsRealtime(oneFlameTime);
        }
    }

    //フィルター表示
    public void FilterDisplay(int filterIndex)
    {
        filter[filterIndex].SetActive(true);
        StartCoroutine(DescriptionStart());
    }
    //フィルター消去
    public void FilterDestroy(int filterIndex)
    {
        filter[filterIndex].SetActive(false);
    }
    //説明開始
    public IEnumerator DescriptionStart()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(displayTime);
        waiting = false;
    }
    //時間変更
    void TimeScaleChange(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    //収穫完了
    public IEnumerator HarvestComplete()
    {
        if (!ColDescription)
        {
            description = false;
            frontBoxObj.SetActive(true);
            hand.SetActive(true);
            handTra.anchoredPosition = handDesPos[0];
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
            handTra.anchoredPosition = handDesPos[2];
            StartCoroutine(DescriptionStart());
            TextDisplay(8);
            yield return new WaitWhile(() => textDisplay == true);
        }
    }
    //ハムスター移動完了
    public IEnumerator HamsterMovingComplete(int textIndex, int filterIndex)
    {
        description = false;
        StartCoroutine(TextDestroy(false));
        yield return new WaitWhile(() => textDestroy == true);
        TimeScaleChange(0.0f);
        FilterDestroy(filterIndex - 1);
        FilterDisplay(filterIndex);
        TextDisplay(textIndex);
        yield return new WaitWhile(() => textDisplay == true);
    }
}
