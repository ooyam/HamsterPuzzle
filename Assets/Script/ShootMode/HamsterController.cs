using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShootMode
{
    public class HamsterController : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        RectTransform canvasTra;           //CanvasのRectTransform
        LineRenderer line;                 //LineRenderer

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:通常(右向き) 1:反転(左向き)
        [System.NonSerialized]
        public bool spriteDefault = true; //0番使用中？

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
        [System.NonSerialized]
        public bool gameStart = false;     //ゲーム開始？

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            line      = GetComponent<LineRenderer>();

            canvasHigh    = canvasTra.sizeDelta.y;
            canvasWidth   = canvasTra.sizeDelta.x;
            differenceX   = canvasWidth / 2;
            differenceY   = canvasHigh / 2;
            magnification = canvasWidth / Screen.width;
            maxX          = differenceX - 50.0f;
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
                            /*/移動する
                            targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
                            tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, new Vector3(targetPos.x, posY), 1.0f);
                            */
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
            float linePosX  = 0.0f;
            float linePosY  = 0.0f;
            float hamPosX   = tra.anchoredPosition.x;
            bool rightThrow = mousePos.x < hamPosX;                                //右に投げ始める?
            float maxY      = canvasHigh / 2 - posY - blockMan.blockDiameter / 2;  //Y最大値
            float[] maxX    = new float[2];                                        //X最大値
            maxX[0]         = (rightThrow) ? differenceX - hamPosX : -differenceX - hamPosX;
            maxX[1]         = (rightThrow) ? -differenceX - hamPosX : differenceX - hamPosX;
            linePosY        = (maxX[0] / -(mousePos.x - hamPosX)) * -mousePos.y;   //Y座標算出

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

            if (linePosY < maxY)
            {
                //---------------------------------------------
                //反射有
                //---------------------------------------------
                linePosX = maxX[0];
                List<Vector3> linePos = new List<Vector3>();
                linePos.Add(lineStartPos);
                linePos.Add(new Vector3(linePosX, linePosY, 0.0f));

                //---------------------------------------------
                //反射計算
                //---------------------------------------------
                bool frastReflection = true;
                while (true)
                {
                    int frontLineIndex = linePos.Count - 1;
                    float quotient = ((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y);
                    float multiplier = canvasWidth / quotient;
                    float nextLinePosY = Mathf.Abs(multiplier) + linePos[frontLineIndex].y;
                    int maxXIndex = ((rightThrow && linePos[frontLineIndex].x < 0) || (!rightThrow && linePos[frontLineIndex].x > 0)) ? 0 : 1;
                    float nextLinePosX = maxX[maxXIndex];
                    if (nextLinePosY < maxY)
                    {
                        linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                    }
                    else
                    {
                        float multiplierY = (maxY - linePos[frontLineIndex].y) * (((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y));
                        nextLinePosY = maxY;
                        nextLinePosX = linePos[frontLineIndex].x - multiplierY;
                        linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
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
                float multiplierY = maxY / -mousePos.y;
                linePosY = maxY;
                linePosX = multiplierY * -(mousePos.x - hamPosX);

                //---------------------------------------------
                //投擲ライン出力
                //---------------------------------------------
                return new Vector3[] { lineStartPos, new Vector3(linePosX, linePosY, 0.0f) };
            }
        }

        //========================================================================
        //線を引く
        //========================================================================
        //linePos; 起動ラインの頂点座標
        //========================================================================
        void DrawLine(Vector3[] linePos)
        {
            line.positionCount = linePos.Length;
            line.startWidth = 20.0f;
            line.endWidth = 20.0f;
            line.SetPositions(linePos);
        }
    }
}