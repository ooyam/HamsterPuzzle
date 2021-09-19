using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            bool leftMove = true;        //左に回転？
            bool rotReturn = false;      //角度戻し中？
            bool shakeStop = false;      //停止？
            int loopTimes = 0;           //動作回数
            int cycleTimes = 0;          //サイクル回数
            float playTime = 0.0f;       //揺れ動作再生時間
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
    }
}