using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode;
using System;

namespace ShootMode
{
    public class HamsterController : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        RectTransform canvasTra;           //CanvasのRectTransform
        LineRenderer line;                 //LineRenderer
        Camera mainCamra;                  //MainCamera

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:通常(右向き) 1:反転(左向き)
        [System.NonSerialized]
        public bool spriteDefault = true; //0番使用中？

        [Header("BlockBox")]
        [SerializeField]
        RectTransform blockBoxTra;

        [Header("BlockManager")]
        [SerializeField]
        BlockManager blockMan;

        bool tapStart = false;             //タップ開始
        float magnification;               //タップ位置修正倍率
        float differenceX;                 //タップ位置修正数X
        float differenceY;                 //タップ位置修正数Y
        float canvasHigh;                  //Canvasの高さ(1920.0f)
        float canvasWidth;                 //Canvasの幅(1080.0f)
        float hamsterPosX;                 //ハムスターX座標
        float posY = -530.0f;              //ハムスターY座標
        float fastTapPosY;                 //最初に触った位置
        bool throwOperation = false;       //投げる動作
        float throwTriggerTapPos = -100.0f;//投げ始める位置
        float throwStopTapPos = -50.0f;    //投げるのをやめる位置
        float throwMaxY;                   //投擲Y座標最大値
        float lineStartPosX = 70.0f;       //投擲ラインのスタート位置X
        float lineStartPosY = 30.0f;       //投擲ラインのスタート位置Y
        Vector3 lineStartPos;              //投擲ラインのスタート位置
        [System.NonSerialized]
        public bool gameStart = false;     //ゲーム開始？
        string[] blockTag;                 //ブロックタグリスト
        string NextBlockBoardTag = "NextBlockBoard"; //次投擲表示ボードタグ

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            line      = GetComponent<LineRenderer>();
            mainCamra = Camera.main;

            canvasHigh    = canvasTra.sizeDelta.y;
            canvasWidth   = canvasTra.sizeDelta.x;
            differenceX   = canvasWidth / 2;
            differenceY   = canvasHigh  / 2;
            magnification = canvasWidth / Screen.width;
            hamsterPosX   = tra.anchoredPosition.x;
            throwMaxY     = differenceY + blockBoxTra.anchoredPosition.y - posY - 100.0f;

            //ブロックタグ取得
            System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
            blockTag = new string[vegetableType.Length];
            foreach (VegetableType value in vegetableType)
            { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        }

        //========================================================================
        //タップ操作
        //========================================================================
        void FixedUpdate()
        {
            if (gameStart)
            {
                if (!blockMan.throwNow && !blockMan.blockDeleteNow)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //次投擲ボードタップ時は計算しない
                        Ray ray = mainCamra.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                        if (!(hit2d && hit2d.transform.gameObject.tag == NextBlockBoardTag))
                        {
                            tapStart = true;
                        }
                    }
                    if (tapStart)
                    {
                        //タップ座標取得
                        Vector3 mousePos = Input.mousePosition;
                        mousePos = new Vector3(mousePos.x * magnification - differenceX, (mousePos.y * magnification - differenceY) - posY, 0.0f);
                        if (mousePos.y < throwTriggerTapPos || (throwOperation && mousePos.y < throwStopTapPos))
                        {
                            Vector3[] linePos = LineCalculation(mousePos);
                            DrawLine(linePos);
                            if (throwOperation)
                            {
                                if (Input.GetMouseButtonUp(0))
                                {
                                    throwOperation = false;
                                    tapStart = false;
                                    line.positionCount = 0;
                                    //ブロックを投げる
                                    StartCoroutine(blockMan.BlockThrow(linePos));
                                }
                            }
                            else throwOperation = true;
                        }
                        else
                        {
                            if (throwOperation)
                            {
                                //投げるのをやめる
                                line.positionCount = 0;
                                throwOperation = false;
                            }
                        }
                    }
                }
            }
        }

        //========================================================================
        //投擲ライン座標計算
        //========================================================================
        //mousePos; マウスの位置
        //========================================================================
        Vector3[] LineCalculation(Vector3 mousePos)
        {
            float linePosX   = 0.0f;
            float linePosY   = 0.0f;
            bool  rightThrow = mousePos.x < hamsterPosX;                                //右に投げ始める?
            float lineStartX = (rightThrow) ? lineStartPosX : -lineStartPosX;           //投擲ブロックのX座標
            lineStartPos     = new Vector3(lineStartX, lineStartPosY, 0.0f);            //ライン投擲開始座標
            float[] maxX     = new float[2];                                            //X最大値
            maxX[0]          = (rightThrow) ? differenceX - hamsterPosX : -differenceX - hamsterPosX;
            maxX[1]          = (rightThrow) ? -differenceX - hamsterPosX : differenceX - hamsterPosX;
            linePosY         = (maxX[0] / -(mousePos.x - hamsterPosX)) * -mousePos.y;   //Y座標算出

            //---------------------------------------------
            //ハムスターの向き指定
            //---------------------------------------------
            if (rightThrow && !spriteDefault)
            {
                ima.sprite = hamsterSprite[0];
                blockMan.ThrowBlockPosChange(0);
                spriteDefault = true;
            }
            else if(!rightThrow && spriteDefault)
            {
                ima.sprite = hamsterSprite[1];
                blockMan.ThrowBlockPosChange(1);
                spriteDefault = false;
            }

            if (linePosY < throwMaxY)
            {
                //---------------------------------------------
                //反射有
                //---------------------------------------------
                linePosX = maxX[0];
                List<Vector3> linePos = new List<Vector3>();
                linePos.Add(lineStartPos);
                bool lineStop = false;

                //Rayを飛ばす
                Vector3 lineEmdPos = FlyRay(lineStartPos, new Vector3(linePosX, linePosY, 0.0f));
                if (lineEmdPos.x != linePosX || lineEmdPos.y != linePosY) lineStop = true;
                linePos.Add(lineEmdPos);

                //---------------------------------------------
                //反射計算
                //---------------------------------------------
                bool frastReflection = true;
                while (!lineStop)
                {
                    int frontLineIndex = linePos.Count - 1;
                    float quotient     = ((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y);
                    float multiplier   = canvasWidth / quotient;
                    float nextLinePosY = Mathf.Abs(multiplier) + linePos[frontLineIndex].y;
                    int   maxXIndex    = ((rightThrow && linePos[frontLineIndex].x < 0) || (!rightThrow && linePos[frontLineIndex].x > 0)) ? 0 : 1;
                    float nextLinePosX = maxX[maxXIndex];
                    if (nextLinePosY < throwMaxY)
                    {
                        //Rayを飛ばす
                        Vector3 nextLinePos = FlyRay(linePos[linePos.Count - 1], new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                        if (nextLinePos.x != nextLinePosX || nextLinePos.y != nextLinePosY) lineStop = true;
                        linePos.Add(nextLinePos);
                    }
                    else
                    {
                        float multiplierY = (throwMaxY - linePos[frontLineIndex].y) * (((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y));
                        nextLinePosY = throwMaxY;
                        nextLinePosX = linePos[frontLineIndex].x - multiplierY;

                        //Rayを飛ばす
                        Vector3 nextLinePos = FlyRay(linePos[linePos.Count - 1], new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                        if (nextLinePos.x != nextLinePosX || nextLinePos.y != nextLinePosY) lineStop = true;
                        linePos.Add(nextLinePos);
                        break;
                    }
                    if (frastReflection) frastReflection = false;
                }

                return linePos.ToArray();  //投擲ライン出力(配列に変換)
            }
            else
            {
                //---------------------------------------------
                //反射無
                //---------------------------------------------
                float multiplierY = throwMaxY / -mousePos.y;
                linePosY = throwMaxY;
                linePosX = multiplierY * -(mousePos.x - hamsterPosX);

                //Rayを飛ばす
                Vector3 lineEmdPos = FlyRay(lineStartPos, new Vector3(linePosX, linePosY, 0.0f));

                //---------------------------------------------
                //投擲ライン出力
                //---------------------------------------------
                return new Vector3[] { lineStartPos, lineEmdPos };
            }
        }

        //========================================================================
        //Rayを飛ばす
        //========================================================================
        //frontPos;  開始地点の座標
        //nextPos;   終着地点の座標
        //return;    ライン終着座標
        //========================================================================
        Vector3 FlyRay(Vector3 frontPos, Vector3 nextPos)
        {
            //座標設定
            float nextLinePosX = nextPos.x;
            float nextLinePosY = nextPos.y;

            frontPos = new Vector3((frontPos.x + differenceX) / magnification, (frontPos.y + posY) / magnification + differenceY, 0.0f);
            frontPos = mainCamra.ScreenToWorldPoint(frontPos);
            nextPos  = new Vector3((nextLinePosX + differenceX) / magnification, (nextLinePosY + posY) / magnification + differenceY, 0.0f);
            nextPos  = mainCamra.ScreenToWorldPoint(nextPos);

            //Rayの作成
            Ray2D ray = new Ray2D(frontPos, new Vector2(nextPos.x - frontPos.x, nextPos.y - frontPos.y));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //接触判定
            if (hit.collider)
            {
                string connectObjTag = hit.collider.gameObject.tag;
                if (0 <= Array.IndexOf(blockTag, connectObjTag))
                {
                    Vector3 connectPos = RectTransformUtility.WorldToScreenPoint(mainCamra, hit.point);
                    nextLinePosX = connectPos.x * magnification - differenceX;
                    nextLinePosY = (connectPos.y * magnification - differenceY) - posY;
                }
            }

            //終点座標を返す
            return new Vector3(nextLinePosX, nextLinePosY, 0.0f);
        }

        //========================================================================
        //線を引く
        //========================================================================
        //linePos; 起動ラインの頂点座標
        //========================================================================
        void DrawLine(Vector3[] linePos)
        {
            line.positionCount = linePos.Length;
            line.startWidth    = 20.0f;
            line.endWidth      = 20.0f;
            line.SetPositions(linePos);
        }
    }
}