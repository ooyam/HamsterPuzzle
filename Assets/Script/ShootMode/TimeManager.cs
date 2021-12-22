using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShootMode;
using static ShootMode.ShootModeDefine;

public class TimeManager : MonoBehaviour
{
    [Header("BlockManager")]
    public BlockManager blockMan;
    [Header("HamsterController")]
    public HamsterController hamCon;
    int stageNum = 0;   //ステージ番号

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
        hamCon.gameStart = true;
        StartCoroutine(LineBlockGenerateInterval());
    }

    //========================================================================
    //時間経過で1行生成指示
    //========================================================================
    IEnumerator LineBlockGenerateInterval()
    {
        float generateTime = BLOCK_GENERATE_TIME;
        switch (stageNum)
        {
            case 10:
                generateTime = BLOCK_GENERATE_TIME;
                break;
            default:
                break;
        }
        while (true)
        {
            yield return new WaitForSeconds(generateTime);
            StartCoroutine(blockMan.LineBlockGenerate(1));
            if (GAME_OVER) break;
        }
    }
}