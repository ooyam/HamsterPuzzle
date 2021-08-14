using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    [Header("数字スプライト")]
    public Sprite[] numbers;
    [Header("ハムスターのスプライト")]
    public Sprite[] hamsterFace;
    [Header("ゲームオーバーオブジェクト")]
    public GameObject gameOverObj;
    [Header("ゲームクリアオブジェクト")]
    public GameObject gameClearObj;
    [Header("CalorieGauge")]
    public CalorieGauge CalGage;

    [System.NonSerialized]
    public Image hamsterPanelImage;   //ハムスターパネルのImage
    private PanelManager panelMan;    //PanelManager
    private Image[] numberImage;      //数字Image
    private RectTransform numberTra;  //数字RectTransform
    private Image hamsterImage;       //ハムスターの表情
    private int imageNum = 3;         //Imegeの個数
    private int nowTurn;              //残りターン数
    [System.NonSerialized]
    public bool effectStart = false;  //ゲームオーバーエフェクト開始

    private float moveSpeed = 3.5f;
    private float maxRot = 10.0f;
    private int moveTimes = 0;
    private bool leftMove = true;
    [System.NonSerialized]
    public bool moveStart = false;
    private bool moveStop = false;

    // Start is called before the first frame update
    void Start()
    {
        panelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        nowTurn = PuzzleMainController.turnNum;
        Transform tra = transform;
        numberImage = new Image[imageNum];
        for (int i = 0; i < imageNum; i++)
        {
            GameObject numObj = tra.GetChild(i).gameObject;
            numberImage[i] = numObj.GetComponent<Image>();
            if (i == 0) numberTra = numObj.GetComponent<RectTransform>();
        }
        GameObject hamObj = GameObject.FindWithTag("HamsterFace");
        hamsterImage = hamObj.GetComponent<Image>();
        TurnCalculation(false);
        HamsterSpriteChange(-1);
    }

    void FixedUpdate()
    {
        if (moveStart && !moveStop)
        {
            float rotZ = numberTra.localRotation.eulerAngles.z;
            rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
            if (leftMove)
            {
                numberTra.Rotate(0.0f, 0.0f, moveSpeed);
                if (rotZ > maxRot)
                {
                    leftMove = false;
                }
            }
            else
            {
                numberTra.Rotate(0.0f, 0.0f, -moveSpeed);
                if (rotZ < -maxRot)
                {
                    leftMove = true;
                    moveTimes++;
                }
            }
            if (moveTimes >= 3 && rotZ <= 0.5f && rotZ >= -0.5f)
            {
                StartCoroutine(MoveStop());
            }
        }
    }

    IEnumerator MoveStop()
    {
        float waitTime = 0.8f;
        moveStop = true;
        yield return new WaitForSeconds(waitTime);
        moveStop = false;
        moveTimes = 0;
    }

    //ターン計算
    public void TurnCalculation(bool reduceTurn)
    {
        int ten = 10;
        if (reduceTurn) nowTurn--;
        if (nowTurn < ten)
        {
            numberImage[0].sprite = numbers[nowTurn];
            numberImage[1].sprite = numbers[ten];
            numberImage[2].sprite = numbers[ten];
            if (nowTurn == 0) StartCoroutine(GameOver(true));
        }
        else
        {
            int SpriteIndex = 0;
            int referenceNum = nowTurn;
            for (int i = 1; i < imageNum; i++)
            {
                SpriteIndex = referenceNum % ten;
                numberImage[i].sprite = numbers[SpriteIndex];
                referenceNum /= ten;
            }
            numberImage[0].sprite = numbers[ten];
        }
        HamsterSpriteChange(-1);
        if (!moveStart && nowTurn <= 5) moveStart = true;
    }

    //ターン回復
    public void TurnRecovery()
    {
        nowTurn++;
        TurnCalculation(false);
        if (moveStart && nowTurn > 5)
        {
            moveStart = false;
            numberTra.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
    }

    //ハムスタースプライトチェンジ
    public void HamsterSpriteChange(int spriteIndex)
    {
        bool faceBack = (spriteIndex < 0) ? true : false;
        if (faceBack)
        {
            if (nowTurn > 5)
                spriteIndex = 0;
            else if (nowTurn > 0)
                spriteIndex = 1;
            else
                spriteIndex = 2;
        }
        if (spriteIndex == 4) gameClearObj.SetActive(true);
        hamsterImage.sprite = hamsterFace[spriteIndex];
        if(hamsterPanelImage != null) hamsterPanelImage.sprite = (faceBack) ? hamsterFace[5] : hamsterFace[spriteIndex];
    }

    //ゲームオーバー
    public IEnumerator GameOver(bool turnOver)
    {
        panelMan.gameOver = true;
        moveStart = false;
        CalGage.moveStart = false;
        yield return new WaitUntil(() => effectStart == true);
        gameOverObj.GetComponent<GameOverGraphic>().turnOver = turnOver;
        gameOverObj.SetActive(true);
    }
}
