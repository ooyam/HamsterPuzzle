using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterController : MonoBehaviour
{
    RectTransform tra;                 //RectTransform
    RectTransform canvasTra;           //CanvasのRectTransform
    LineRenderer line;                 //LineRenderer

    [Header("BlockManager")]
    public BlockManager blockMan;
    bool tapStart = false;             //タップ開始
    float magnification;               //タップ位置修正倍率
    float differenceX;                 //タップ位置修正数X
    float differenceY;                 //タップ位置修正数Y
    float canvasHigh;                  //Canvasの高さ(1920.0f)
    float canvasWidth;                 //Canvasの幅(1080.0f)
    float maxX;                        //ハムスター追従限界値
    float posY = -530.0f;              //ハムスターY座標
    float posZ = -1.0f;                //ハムスターZ座標
    float fastTapPosY;                 //最初に触った位置
    bool throwOperation = false;       //投げる動作
    float throwTriggerTapPos = -100.0f;//投げ始める位置
    float throwStopTapPos = -50.0f;    //投げるのをやめる位置
    Vector3 lineStartPos = new Vector3(0.0f, 30.0f, 0.0f);//投擲ラインのスタート位置

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        line = GetComponent<LineRenderer>();
        canvasHigh = canvasTra.sizeDelta.y;
        canvasWidth = canvasTra.sizeDelta.x;
        differenceX = canvasWidth / 2;
        differenceY = canvasHigh / 2;
        magnification = canvasWidth / Screen.width;
        maxX = differenceX - 50.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fastTapPosY = Input.mousePosition.y * magnification - differenceY;
            tapStart = true;
        }
        if (tapStart)
        {
            Vector3 mousePos = Input.mousePosition;
            //座標の修正
            Vector3 targetPos = new Vector3(mousePos.x * magnification - differenceX, (mousePos.y * magnification - differenceY) - posY, posZ);
            if (targetPos.y < throwTriggerTapPos || (throwOperation && targetPos.y < throwStopTapPos))
            {
                Vector3[] linePos = LineCalculation(targetPos);
                DrawLine(linePos);
                if (!throwOperation) throwOperation = true;
            }
            else
            {
                //移動する
                targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
                tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, new Vector3(targetPos.x, posY), 1.0f);

                if (throwOperation)
                {
                    //投げるのをやめる
                    line.positionCount = 0;
                    throwOperation = false;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            tapStart = false;
            throwOperation = false;
            line.positionCount = 0;
        }
    }

    //投擲ライン座標計算
    Vector3[] LineCalculation(Vector3 mousePos)
    {
        float linePosX = 0.0f;
        float linePosY = 0.0f;
        float hamPosX = tra.anchoredPosition.x;
        float maxY = canvasHigh - (blockMan.blockPosY * (blockMan.nowLineNum - 1)) + posY;     //Y最大値
        float maxX = (mousePos.x < hamPosX) ? differenceX - hamPosX : -differenceX - hamPosX;  //X最大値
        float multiplierX = maxX / -(mousePos.x - hamPosX);                                    //乗数         
        linePosY = multiplierX * -mousePos.y;                                                  //Y座標算出

        //反射有
        if (linePosY <= maxY)
        {
            linePosX = maxX;
            List<Vector3> linePos = new List<Vector3>();
            linePos.Add(lineStartPos);
            linePos.Add(new Vector3(linePosX, linePosY, 0.0f));

            //反射計算
            while (true)
            {
                float frontLinePosX = linePos[linePos.Count - 1].x;
                float frontLinePosY = linePos[linePos.Count - 1].y;
                float quotient = frontLinePosX / frontLinePosY;
                float multiplier = canvasWidth / quotient;
                float nextLinePosY = Mathf.Abs(multiplier) + frontLinePosY;
                float nextLinePosX = (frontLinePosX >= 0) ? -(canvasWidth - Mathf.Abs(maxX)) : canvasWidth - Mathf.Abs(maxX);
                if (nextLinePosY < maxY)
                {
                    linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                }
                else
                {
                    break;
                }
            }

            //投擲ライン出力(配列に変換)
            return linePos.ToArray();
        }
        //反射無
        else
        {
            float multiplierY = maxY / -mousePos.y;
            linePosY = maxY;
            linePosX = multiplierY * -(mousePos.x - hamPosX);

            //投擲ライン出力
            return new Vector3[] { lineStartPos, new Vector3(linePosX, linePosY, 0.0f) };
        }
    }

    //線を引く
    void DrawLine(Vector3[] linePos)
    {
        line.positionCount = linePos.Length;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.SetPositions(linePos);
    }
}
