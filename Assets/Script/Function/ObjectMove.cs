using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoveFunction
{
    public class ObjectMove : MonoBehaviour
    {
        //========================================================================
        //揺れる動作
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //moveSpeed;  動作速度
        //maxRot;     揺れ角度
        //moveCount;  1サイクル動作回数(カウントしない場合は-1指定)
        //stopTime;   停止時間
        //breakCount; 終了サイクル数(無限ループの場合は-1指定)
        //endTime;    揺れ終了時間(時間で止めない場合は-1指定)
        //========================================================================
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove      = true;   //左に回転？
            bool rotReturn     = false;  //角度戻し中？
            bool shakeStop     = false;  //停止？
            int loopTimes      = 0;      //動作回数
            int cycleTimes     = 0;      //サイクル回数
            float playTime     = 0.0f;   //揺れ動作再生時間
            float oneFrameTime = 0.02f;  //1フレームの時間
            while (true)
            {
                yield return new WaitForFixedUpdate();
                float rotZ = tra.localRotation.eulerAngles.z;
                rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
                if (!rotReturn)
                {
                    if (leftMove)
                    {
                        //---------------------------------------------
                        //左に回転
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, moveSpeed);
                        if (rotZ > maxRot) leftMove = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //右に回転
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, -moveSpeed);
                        if (rotZ < -maxRot)
                        {
                            loopTimes++;                            
                            if (0 < moveCount && moveCount <= loopTimes) rotReturn = true;  //終了サイクル数を超えた時に揺れを止める
                            else leftMove = true;
                        }
                    }
                }
                else
                {
                    //---------------------------------------------
                    //角度を0に戻す
                    //---------------------------------------------
                    tra.Rotate(0.0f, 0.0f, moveSpeed);
                    if (-0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        shakeStop = true;
                    }

                    //---------------------------------------------
                    //揺れを止める
                    //---------------------------------------------
                    if (shakeStop)
                    {
                        cycleTimes++;
                        if (0 < breakCount && breakCount <= cycleTimes) break;  //揺れ終了
                        else yield return new WaitForSeconds(stopTime);         //一時停止
                        loopTimes = 0;
                        rotReturn = false;
                        shakeStop = false;
                    }
                }

                //---------------------------------------------
                //時間で停止する場合の処理
                //---------------------------------------------
                if (0 < endTime)
                {
                    playTime += oneFrameTime;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //揺れ終了
                    }
                }
            }
        }


        //========================================================================
        //移動動作
        //========================================================================
        //tra;        動作オブジェクトのRectTransform
        //moveSpeed;  動作速度
        //targetPos;  目標座標
        //sideways;   X方向に動作？
        //========================================================================
        public static IEnumerator MoveMovement(RectTransform tra, float moveSpeed, Vector2 targetPos, bool sideways)
        {
            float offset = 0.5f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, targetPos, moveSpeed);
                Vector2 nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //移動終了
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    tra.anchoredPosition = targetPos;
                    break;
                }
            }
        }


        //========================================================================
        //拡大縮小動作
        //========================================================================
        //tra;          動作オブジェクトのRectTransform
        //scalingSpeed; 拡縮速度
        //changeScale;  変更時の拡大率
        //endScale;     終了時の拡大率
        //scalingTimes; 拡縮回数
        //========================================================================
        public static IEnumerator ScaleChange(RectTransform tra, float scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            Vector3 nowScale = tra.localScale;    //現在の拡大率
            bool scaleUp     = scalingSpeed > 0;  //拡大？
            bool scaleChange = true;              //変更動作中？
            bool end         = false;             //変更動作終了？

            for (int loopTimes = 0; loopTimes < scalingTimes; loopTimes++)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    if (scaleChange)
                    {
                        //---------------------------------------------
                        //変更動作
                        //---------------------------------------------
                        nowScale = Vector3.one * (nowScale.x + scalingSpeed);
                        if ((scaleUp && nowScale.x >= changeScale) || (!scaleUp && nowScale.x <= changeScale))
                            scaleChange = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //終了動作
                        //---------------------------------------------
                        nowScale = Vector3.one * (nowScale.x - scalingSpeed);
                        if ((scaleUp && nowScale.x <= endScale) || (!scaleUp && nowScale.x >= endScale))
                            end = true;
                    }
                    tra.localScale = nowScale;
                    if (end) break;
                }

                //変数リセット
                tra.localScale = Vector3.one * endScale;
                scaleChange = true;
                end = false;
            }
        }


        //========================================================================
        //色変更(2色点滅)動作  ※Image or Text の使用ない方は null を指定する
        //========================================================================
        //ima;          変更対象Image
        //tex;          変更対象Text
        //changeSpeed;  変更速度
        //colArray;     変更色の配列
        //compArray;    比較番号指定配列(0:R 1:G 2:B 3:A)
        //chengeCount;  ループ回数(配列1周で1カウント、-1指定で無限再生)
        //========================================================================
        //changeEnd;    点滅停止
        //========================================================================
        public static bool changeEnd = false;
        public static IEnumerator PaletteChange(Image ima, Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;                //1フレーム時間
            int loopTimes = 0;                         //繰り返し回数
            int colCount = colArray.Length;            //変更色の数

            int nowIndex = 0;    //現在の色
            int nextIndex = 1;   //次の色
            float nextCompCol = colArray[nextIndex][compArray[nowIndex]]; //比較色指定
            float judgeRange = 5.0f / 255.0f;                             //判定範囲

            if (ima != null)
            {
                //-------------------------
                //Image
                //-------------------------
                while (!changeEnd)
                {
                    ima.color = Color.Lerp(tex.color, colArray[nextIndex], changeSpeed);
                    float nowCompCol = ima.color[compArray[nowIndex]];
                    if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                    {
                        nowIndex = nextIndex;
                        nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                        nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                        loopTimes++;
                    }
                    if (chengeCount >= 0 && loopTimes >= chengeCount) break;
                    yield return new WaitForSecondsRealtime(oneFrameTime);
                }
            }
            else
            {
                //-------------------------
                //Text
                //-------------------------
                while (!changeEnd)
                {
                    tex.color = Color.Lerp(tex.color, colArray[nextIndex], changeSpeed);
                    float nowCompCol = tex.color[compArray[nowIndex]];
                    if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                    {
                        nowIndex = nextIndex;
                        nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                        nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                        loopTimes++;
                    }
                    if (chengeCount >= 0 && loopTimes >= chengeCount) break;
                    yield return new WaitForSecondsRealtime(oneFrameTime);
                }
            }
            changeEnd = false;
        }
    }
}