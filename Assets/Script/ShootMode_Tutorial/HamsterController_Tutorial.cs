using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode_Tutorial;
using ShootMode;
using System;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class HamsterController_Tutorial : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        LineRenderer line;                 //LineRenderer
        Renderer ren;                      //Renderer
        Camera mainCamra;                  //MainCamera
        [System.NonSerialized]
        public Color nowBlockColor;        //投擲ブロックの色

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:通常(右向き) 1:反転(左向き) 2:投擲準備完了(右向き) 3:投擲準備完了(右向き)
        [System.NonSerialized]
        public int spriteNum = 0;         //使用中sprite番号

        [Header("BlockBox")]
        [SerializeField]
        RectTransform blockBoxTra;
        Rect blockBoxSize;   //backGroundの幅

        [Header("BlockManager")]
        [SerializeField]
        BlockManager_Tutorial blockMan;

        [Header("チュートリアルマネージャー")]
        [SerializeField]
        TutorialManager tutorialMan;

        float magnification;               //タップ位置修正倍率
        float differenceX;                 //タップ位置修正数X
        float differenceY;                 //タップ位置修正数Y
        float hamsterPosX;                 //ハムスターX座標
        float posY = -530.0f;              //ハムスターY座標
        float fastTapPosY;                 //最初に触った位置
        bool  displayLine = false;         //軌道線表示中？
        float throwStartTapPos = -50.0f;   //投げ始める位置
        float topLimit;                    //投擲ラインY座標上限
        float[] sideLimit;                 //投擲ラインX座標上限
        float lineStartPosX = 70.0f;       //投擲ラインのスタート位置X
        float lineStartPosY = -20.0f;       //投擲ラインのスタート位置Y
        Vector3 lineStartPos;              //投擲ラインのスタート位置
        string[] blockTag;                 //ブロックタグリスト

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            line      = GetComponent<LineRenderer>();
            ren       = GetComponent<Renderer>();
            mainCamra = Camera.main;

            differenceY   = CANVAS_HEIGHT / 2.0f;
            differenceX   = CANVAS_WIDTH  / 2.0f;
            magnification = CANVAS_WIDTH  / Screen.width;
            hamsterPosX   = tra.anchoredPosition.x;
            blockBoxSize  = blockBoxTra.rect;
            topLimit      = blockBoxSize.height / 2.0f + blockBoxTra.anchoredPosition.y - posY;
            sideLimit     = new float[] { blockBoxSize.width / 2.0f - hamsterPosX, -blockBoxSize.width / 2.0f - hamsterPosX };

            //ブロックタグ取得
            System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
            blockTag = new string[vegetableType.Length];
            foreach (VegetableType value in vegetableType)
            { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        }

        //========================================================================
        //ハムスタータップ
        //========================================================================
        public void HumsterButtonDown()
        {
            //ゲーム中？
            if (GAME_START && !GAME_OVER && !GAME_CLEAR)
            {
                //特定の動作中？
                if (!blockMan.throwNow && !blockMan.blockDeleteNow && !blockMan.blockChangeNow && !SPECIAL_HARVEST && !FEVER_START && !SETTING_DISPLAY)
                {
                    if (tutorialMan.throwWait)
                    {
                        //投擲指示手非表示
                        tutorialMan.HandHide();

                        //sprite変更
                        spriteNum  = (spriteNum == 0) ? 2 : 3;
                        ima.sprite = hamsterSprite[spriteNum];

                        //ブロックの位置変更
                        tra.SetSiblingIndex(0);
                        blockMan.ThrowBlockPosChange(spriteNum);

                        //線の色変更
                        ren.material.color = nowBlockColor;

                        //投擲準備
                        StartCoroutine(PreparingThrowBlock());
                    }
                }
            }
        }

        //========================================================================
        //ブロック投擲準備
        //========================================================================
        IEnumerator PreparingThrowBlock()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                //タップ座標取得
                Vector3 mousePos = Input.mousePosition;
                mousePos = new Vector3(mousePos.x * magnification - differenceX, (mousePos.y * magnification - differenceY) - posY, 0.0f);
                if (mousePos.y < throwStartTapPos)
                {
                    //軌道線を引く
                    displayLine = true;
                    Vector3[] linePos = LineCalculation(mousePos);
                    DrawLine(linePos);

                    //ブロックを投げる
                    if (Input.GetMouseButtonUp(0))
                    {
                        line.positionCount = 0;
                        StartCoroutine(blockMan.BlockThrow(linePos));
                        break;
                    }
                }
                else
                {
                    //軌道線を消す
                    if (displayLine)
                    {
                        line.positionCount = 0;
                        displayLine = false;
                    }

                    //投擲をやめる
                    if (Input.GetMouseButtonUp(0))
                    {
                        //子オブジェクト番号変更(ブロックの前にでる)
                        tra.SetSiblingIndex(1);
                        blockMan.ThrowBlockPosChange((spriteNum == 2) ? 0 : 1);
                        break;
                    }
                }

                //ゲーム終了時
                if (!GAME_START || GAME_OVER || GAME_CLEAR)
                {
                    //軌道線を消す
                    if (displayLine)
                    {
                        line.positionCount = 0;
                        displayLine = false;
                    }

                    tra.SetSiblingIndex(1);
                    blockMan.ThrowBlockPosChange((spriteNum == 2) ? 0 : 1);
                    break;
                }
            }

            //spriteを戻す
            spriteNum = (spriteNum == 2) ? 0 : 1;
            ima.sprite = hamsterSprite[spriteNum];
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
            bool  rightThrow = mousePos.x < hamsterPosX;                         //右に投げ始める?
            lineStartPos     = new Vector3(lineStartPosX, lineStartPosY, 0.0f);  //ライン投擲開始座標
            float[] maxX     = new float[2];                                     //X最大値
            maxX[0]          = sideLimit[0];
            maxX[1]          = sideLimit[1];
            if (!rightThrow)
            {
                lineStartPos = new Vector3(-lineStartPosX, lineStartPosY, 0.0f);
                maxX[0] = sideLimit[1];
                maxX[1] = sideLimit[0];
            }
            linePosY = (maxX[0] / -(mousePos.x - hamsterPosX)) * -mousePos.y;   //Y座標算出

            //---------------------------------------------
            //ハムスターの向き指定
            //---------------------------------------------
            if (rightThrow && spriteNum == 3)
            {
                spriteNum = 2;
                ima.sprite = hamsterSprite[spriteNum];
                blockMan.ThrowBlockPosChange(spriteNum);
            }
            else if(!rightThrow && (spriteNum == 2))
            {
                spriteNum = 3;
                ima.sprite = hamsterSprite[spriteNum];
                blockMan.ThrowBlockPosChange(spriteNum);
            }

            if (linePosY < topLimit)
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
                    float multiplier   = blockBoxSize.width / quotient;
                    float nextLinePosY = Mathf.Abs(multiplier) + linePos[frontLineIndex].y;
                    int   maxXIndex    = ((rightThrow && linePos[frontLineIndex].x < 0) || (!rightThrow && linePos[frontLineIndex].x > 0)) ? 0 : 1;
                    float nextLinePosX = maxX[maxXIndex];
                    if (nextLinePosY < topLimit)
                    {
                        //Rayを飛ばす
                        Vector3 nextLinePos = FlyRay(linePos[linePos.Count - 1], new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                        if (nextLinePos.x != nextLinePosX || nextLinePos.y != nextLinePosY) lineStop = true;
                        linePos.Add(nextLinePos);
                    }
                    else
                    {
                        float multiplierY = (topLimit - linePos[frontLineIndex].y) * (((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y));
                        nextLinePosY = topLimit;
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
                float multiplierY = topLimit / -mousePos.y;
                linePosY = topLimit;
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

            frontPos = new Vector3((frontPos.x + differenceX) / magnification, (frontPos.y + posY + differenceY) / magnification, 0.0f);
            frontPos = mainCamra.ScreenToWorldPoint(frontPos);
            nextPos  = new Vector3((nextLinePosX + differenceX) / magnification, (nextLinePosY + posY + differenceY) / magnification, 0.0f);
            nextPos  = mainCamra.ScreenToWorldPoint(nextPos);

            //Rayの作成
            Ray2D ray = new Ray2D(frontPos, new Vector3(nextPos.x - frontPos.x, nextPos.y - frontPos.y, 0.0f));
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
            //線の長さ算出√(x' - x)**2 + (y' - y)**2
            int posCpunt = linePos.Length;
            float lineLength = 0.0f;
            for (int i = 0; i < posCpunt - 1; i++)
            { lineLength += Mathf.Sqrt(Mathf.Pow((linePos[i + 1].x - linePos[i].x), 2.0f) + Mathf.Pow((linePos[i + 1].y - linePos[i].y), 2.0f)); }
            ren.material.mainTextureScale = new Vector2(lineLength * 0.01f, 1.0f);

            //線出力
            line.positionCount = posCpunt;
            line.startWidth    = 15.0f;
            line.endWidth      = 15.0f;
            line.SetPositions(linePos);
        }
    }
}