using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Header("Filter取得")]
    public GameObject[] filter;
    [Header("FilterFrontBox")]
    public GameObject frontBoxObj;
    [System.NonSerialized]
    public Transform frontBoxTra;
    [Header("TextBox")]
    public GameObject[] textBox;
    private GameObject textObj;
    [Header("テキスト格納ボックス")]
    public Transform textBoxTra;
    [Header("手取得")]
    public GameObject hand;
    private RectTransform handTra;

    private HamsterPanelController HamsterCon;   //ハムスターパネルスクリプト
    [System.NonSerialized]
    public int tupNum = 0;             //タップ回数
    [System.NonSerialized]
    public bool waiting = false;       //待機中？
    [System.NonSerialized]
    public bool description = false;   //説明中？
    private float displayTime = 2.0f;  //説明の最低表示時間

    //手の位置
    private Vector2[] handStartPos = new Vector2[] { new Vector2(0.0f, 0.0f), new Vector2(0.0f, -170.0f) };
    private Vector2[] handEndPos = new Vector2[] { new Vector2(0.0f, -170.0f), new Vector2(-360.0f, -170.0f) };
    private Color[] handColor = new Color[] { new Color(1, 1, 1, 1), new Color(1, 1, 1, 0) };

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
    void FixedUpdate()
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
                    switch (tupNum)
                    {
                        case 1:
                            Debug.Log("フィルター１表示");
                            frontBoxObj.SetActive(false);
                            TextDestroy(true);
                            TextDisplay(1);
                            FilterDisplay(0);
                            hand.SetActive(true);
                            break;
                        case 2:
                        case 6:
                            Debug.Log("手消し");
                            hand.SetActive(false);
                            description = true;
                            HamsterCon.description = true;
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
                            TimeScaleChange(1.0f);
                            TextDestroy(false);
                            TextDisplay(4);
                            FilterDestroy(1);
                            StartCoroutine(DescriptionStart());
                            break;
                        case 5:
                            Debug.Log("フィルター表示");
                            frontBoxObj.SetActive(false);
                            TextDestroy(false);
                            TextDisplay(5);
                            FilterDisplay(0);
                            hand.SetActive(true);
                            break;
                        case 7:
                            Debug.Log("フィルター表示");
                            frontBoxObj.SetActive(true);
                            TextDestroy(false);
                            TextDisplay(8);
                            hand.SetActive(true);
                            break;
                        case 8:
                            Debug.Log("フィルター表示");
                            TextDestroy(false);
                            TextDisplay(9);
                            hand.SetActive(true);
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
}
