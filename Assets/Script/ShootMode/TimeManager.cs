using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using static ShootMode.ShootModeDefine;

public class TimeManager : MonoBehaviour
{
    [Header("BlockManager")]
    public BlockManager blockMan;

    //========================================================================
    //ゲーム開始遅延
    //========================================================================
    IEnumerator Start()
    {
        float countTime = 1.0f;
        int stratTime = 3;
        for (int count = 0; count < stratTime; count++)
        {
            yield return new WaitForSeconds(countTime);
            if (count == stratTime) break;
        }
        GAME_START = true;
        StartCoroutine(LineBlockGenerateInterval());
    }

    //========================================================================
    //時間経過で1行生成指示
    //========================================================================
    IEnumerator LineBlockGenerateInterval()
    {
        float elapsedTime  = 0.0f;
        float oneFrameTime = 0.02f;

        while (!GAME_OVER && !GAME_CLEAR)
        {
            yield return new WaitForFixedUpdate();

            //一部の動作中はカウントストップ(1行収穫中・フィーバー・ブロック削除)
            if (!SPECIAL_HARVEST && !FEVER_START && !blockMan.blockDeleteNow)
                elapsedTime += oneFrameTime;

            //一部の動作中は終了するまで待機(投擲・投擲ブロック切り替え)
            if (!blockMan.throwNow && !blockMan.blockChangeNow)
            {
                if (BLOCK_GENERATE_TIME <= elapsedTime)
                {
                    //ゲーム終了？
                    if (GAME_OVER || GAME_CLEAR) break;

                    //1行生成
                    StartCoroutine(blockMan.LineBlockGenerate(1));

                    //経過時間リセット
                    elapsedTime = 0.0f;
                }
            }
        }
    }
}