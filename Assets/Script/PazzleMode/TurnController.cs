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

    [System.NonSerialized]
    public Image hamsterPanelImage;   //ハムスターパネルのImage
    private PanelManager panelMan;    //PanelManager
    private Image[] numberImage;      //桁ごとのImage
    private Image hamsterImage;       //ハムスターの表情
    private int imageNum = 3;         //Imegeの個数
    private int nowTurn;              //残りターン数
    [System.NonSerialized]
    public bool effectStart = false;  //ゲームオーバーエフェクト開始

    // Start is called before the first frame update
    void Start()
    {
        panelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        nowTurn = PuzzleMainController.turnNum;
        int imageNum = 3;
        Transform tra = transform;
        numberImage = new Image[imageNum];
        for (int i = 0; i < imageNum; i++)
        {
            numberImage[i] = tra.GetChild(i).gameObject.GetComponent<Image>();
        }
        hamsterImage = GameObject.FindWithTag("HamsterFace").GetComponent<Image>();
        TurnCalculation(false);
        HamsterSpriteChange(-1);
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
    }

    //ターン回復
    public void TurnRecovery()
    {
        nowTurn++;
        TurnCalculation(false);
    }

    //ハムスタースプライトチェンジ
    public void HamsterSpriteChange(int spriteIndex)
    {
        bool faceBack = (spriteIndex < 0) ? true : false;
        if (faceBack)
        {
            if (nowTurn > 10)
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
        yield return new WaitUntil(() => effectStart == true);
        gameOverObj.GetComponent<GameOverGraphic>().turnOver = turnOver;
        gameOverObj.SetActive(true);
    }
}
