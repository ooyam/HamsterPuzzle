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
    int stageNum = 0;   //�X�e�[�W�ԍ�

    //========================================================================
    //�Q�[���J�n�x��
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
    //���Ԍo�߂�1�s�����w��
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
        while (!GAME_OVER && !GAME_CLEAR)
        {
            //�ҋ@
            yield return new WaitForSeconds(generateTime);

            //�����E�u���b�N�폜�E�����u���b�N�������I������܂őҋ@
            yield return new WaitWhile(() => blockMan.throwNow == true);
            yield return new WaitWhile(() => blockMan.blockDeleteNow == true);
            yield return new WaitWhile(() => blockMan.blockChangeNow == true);

            //�Q�[���I���H
            if (GAME_OVER || GAME_CLEAR) break;

            //1�s����
            StartCoroutine(blockMan.LineBlockGenerate(1));
        }
    }
}