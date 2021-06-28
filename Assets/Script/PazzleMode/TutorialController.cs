using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [System.NonSerialized]
    public int tupNum = 0;               //タップ回数
    private bool waiting = false;        //待機中？
    private bool description = false;    //説明中？
    private float displayTime = 2.0f;    //説明の最低表示時間
    [System.NonSerialized]
    public bool ColDescription = false;  //体力説明完了？

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
                if (waiting)
                {
                    Debug.Log("待機中");
                }
                else
                {
                    tupNum++;
                    Debug.Log(tupNum);
                    switch (tupNum)
                    {
                        case 1:
                            Debug.Log("フィルター１表示");
                            frontBoxObj.SetActive(false);
                            TextDestroy(true);
                            TextDisplay(1);
                            FilterDisplay(0);
                            hand.SetActive(true);
                            HamsterCon.description = true;
                            break;
                        case 2:
                        case 6:
                            Debug.Log("手消し");
                            HamsterCon.description = false;
                            hand.SetActive(false);
                            description = true;
                            break;
                        case 3:
                            Debug.Log("時間戻し収穫待ち");
                            TimeScaleChange(1.0f);
                            TextDestroy(false);
                            FilterDestroy(1);
                            description = true;
                            break;
                        case 4:
                            Debug.Log("ターン説明");
                            calGaugeTra.SetParent(statusBoardTra, true);
                            turnTra.SetParent(frontBoxTra, true);
                            handTra.anchoredPosition = handDesPos[1];
                            TimeScaleChange(1.0f);
                            TextDestroy(false);
                            TextDisplay(4);
                            FilterDestroy(1);
                            StartCoroutine(DescriptionStart());
                            break;
                        case 5:
                            Debug.Log("フィルター表示");
                            turnTra.SetParent(statusBoardTra, true);
                            hand.SetActive(true);
                            handTra.anchoredPosition = handStartPos[1];
                            HamsterCon.MovingLimit(false);
                            frontBoxObj.SetActive(false);
                            TextDestroy(false);
                            TextDisplay(5);
                            FilterDisplay(2);
                            break;
                        case 7:
                            Debug.Log("列説明");
                            TextDestroy(false);
                            TextDisplay(7);
                            StartCoroutine(DescriptionStart());
                            break;
                        case 8:
                            Debug.Log("時間戻し収穫待ち");
                            TimeScaleChange(1.0f);
                            TextDestroy(false);
                            FilterDestroy(3);
                            description = true;
                            break;
                        case 9:
                            Debug.Log("最後の声援");
                            TextDestroy(false);
                            TextDisplay(9);
                            StartCoroutine(DescriptionStart());
                            break;
                        case 10:
                            Debug.Log("終了");
                            targetTra.SetParent(statusBoardTra, true);
                            harvestTra.SetParent(statusBoardTra, true);
                            HamsterCon.tutorial = false;
                            HamsterCon.MovingLimit(false);
                            this.gameObject.SetActive(false);
                            break;
                    }
                }
            }
        }
    }

    //テキスト表示
    public void TextDisplay(int textIndex)
    {
        textObj = Instantiate(textBox[textIndex]);
        textObj.transform.SetParent(textBoxTra, false);
    }
    //テキスト消去
    public void TextDestroy(bool firstDes)
    {
        if (firstDes) HamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterPanelController>();
        Destroy(textObj);
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
    public void TimeScaleChange(float timeScale)
    {
        Time.timeScale = timeScale;
    }
    //収穫完了
    public void HarvestComplete()
    {
        if (!ColDescription)
        {
            description = false;
            frontBoxObj.SetActive(true);
            hand.SetActive(true);
            handTra.anchoredPosition = handDesPos[0];
            TextDisplay(3);
            StartCoroutine(DescriptionStart());
            calGaugeTra.SetParent(frontBoxTra, true);
            ColDescription = true;
            Debug.Log("体力説明");
        }
        else
        {
            description = false;
            frontBoxObj.SetActive(true);
            targetTra.SetParent(frontBoxTra, true);
            harvestTra.SetParent(frontBoxTra, true);
            hand.SetActive(true);
            handTra.anchoredPosition = handDesPos[2];
            TextDisplay(8);
            StartCoroutine(DescriptionStart());
            Debug.Log("目標説明");
        }
    }
    //ハムスター移動完了
    public void HamsterMovingComplete(int textIndex, int filterIndex)
    {
        description = false;
        TimeScaleChange(0.0f);
        TextDestroy(false);
        TextDisplay(textIndex);
        FilterDestroy(filterIndex - 1);
        FilterDisplay(filterIndex);
    }
}
