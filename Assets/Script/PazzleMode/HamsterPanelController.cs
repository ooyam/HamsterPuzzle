using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterPanelController : MonoBehaviour
{
    private RectTransform CanvasTra;       //CanvasのRectTransform
    private PanelManager PanelMangerScr;   //PanelMangerのスクリプト
    private TutorialController tutorialCon;//tutorialのスクリプト
    private float Magnification;           //タップ位置修正倍率
    private float DifferenceX;             //タップ位置修正数X
    private float DifferenceY;             //タップ位置修正数Y
    private float MinX;                    //ハムスター追従限界値
    private float MaxX;                    //ハムスター追従限界値
    private float MinY;                    //ハムスター追従限界値
    private float MaxY;                    //ハムスター追従限界値
    [System.NonSerialized]
    public bool tutorial = false;          //チュートリアル?
    [System.NonSerialized]
    public bool description = false;       //説明中？

    private RectTransform Tra;  //RectTransform取得
    [System.NonSerialized]
    public bool Push;           //パネルが押されている
    [System.NonSerialized]
    public int HamPosNum;       //自身の位置番号
    [System.NonSerialized]
    public bool Harvest;        //収穫中？
    [System.NonSerialized]
    public bool gameOver;       //ゲームオーバー？
    [System.NonSerialized]
    public bool gameClear;      //ゲームクリア？

    // Start is called before the first frame update
    void Start()
    {
        tutorial = PuzzleMainController.tutorial;
        if(tutorial) tutorialCon = GameObject.FindWithTag("Tutorial").GetComponent<TutorialController>();
        CanvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        PanelMangerScr = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
        Magnification = CanvasTra.sizeDelta.x / Screen.width;
        DifferenceX = CanvasTra.sizeDelta.x / 2;
        DifferenceY = CanvasTra.sizeDelta.y / 2;
        Tra = GetComponent<RectTransform>();
        MovingLimit(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Push)
        {
            Vector3 MousePos = Input.mousePosition;
            //座標の修正
            MousePos.x = MousePos.x * Magnification - DifferenceX;
            MousePos.y = MousePos.y * Magnification - DifferenceY;
            MousePos.z = 0.0f;
            // X, Y座標の範囲を制限する
            MousePos.x = Mathf.Clamp(MousePos.x, MinX, MaxX);
            MousePos.y = Mathf.Clamp(MousePos.y, MinY, MaxY);
            Tra.anchoredPosition = Vector3.Lerp(Tra.anchoredPosition, MousePos, 1);
            if (Input.GetMouseButtonUp(0))
            {
                ReleasePanel();
            }
        }
    }

    //タップした時
    public void PushPanel()
    {
        if (!Harvest && !gameOver && !gameClear && !description)
        {
            Push = true;
            PanelMangerScr.HamsterMoving = true;
        }
    }
    //離した時
    public void ReleasePanel()
    {
        if (!Harvest && !gameOver && !gameClear && !description)
        {
            Push = false;
            Tra.anchoredPosition = PanelMangerScr.PanelPosList[HamPosNum];
            PanelMangerScr.HamsterRelease();
        }
    }

    //パネル位置変更
    public void PanelPosChange(int panelPosNum)
    {
        HamPosNum = panelPosNum;
        if (tutorial)
        {
            if (tutorialCon.tupNum == 2 || panelPosNum == 12)
            {
                int textIndex = 2;
                int filterIndex = 1;
                if (panelPosNum == 12)
                {
                    textIndex *= 3;
                    filterIndex *= 3;
                    Debug.Log("にんじん並んだテキスト表示");
                }
                else
                {
                    Debug.Log("ブロッコリー並んだテキスト表示");
                }
                tutorialCon.HamsterMovingComplete(textIndex, filterIndex);
                ReleasePanel();
            }
        }
    }

    //移動制限指定
    public void MovingLimit(bool first)
    {
        if (!tutorial)
        {
            MinX = PanelMangerScr.PanelPosList[0].x;
            MaxX = -MinX;
            MinY = PanelMangerScr.PanelPosList[PanelMangerScr.PanelNum - 1].y;
            MaxY = PanelMangerScr.PanelPosList[0].y;
        }
        else
        {
            if (first)
            {
                MinX = Tra.anchoredPosition.x;
                MaxX = MinX;
                MinY = PanelMangerScr.PanelPosList[14].y;
                MaxY = Tra.anchoredPosition.y;
            }
            else
            {
                MinX = PanelMangerScr.PanelPosList[12].x;
                MaxX = Tra.anchoredPosition.x;
                MinY = Tra.anchoredPosition.y;
                MaxY = MinY;
            }
        }
    }
}
