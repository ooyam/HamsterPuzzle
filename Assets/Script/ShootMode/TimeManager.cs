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
        while (!GAME_OVER && !GAME_CLEAR)
        {
            //待機
            yield return new WaitForSeconds(BLOCK_GENERATE_TIME);

            //一部の動作中は終了するまで待機
            yield return new WaitWhile(() => SPECIAL_HARVEST == true);           //1行収穫中
            yield return new WaitWhile(() => blockMan.throwNow == true);         //投擲
            yield return new WaitWhile(() => blockMan.blockDeleteNow == true);   //ブロック削除
            yield return new WaitWhile(() => blockMan.blockChangeNow == true);   //投擲ブロック切り替え

            //ゲーム終了？
            if (GAME_OVER || GAME_CLEAR) break;

            //1行生成
            StartCoroutine(blockMan.LineBlockGenerate(1));
        }
    }
}