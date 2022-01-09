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
        while (!GAME_OVER && !GAME_CLEAR)
        {
            //�ҋ@
            yield return new WaitForSeconds(BLOCK_GENERATE_TIME);

            //�ꕔ�̓��쒆�͏I������܂őҋ@
            yield return new WaitWhile(() => SPECIAL_HARVEST == true);           //1�s���n��
            yield return new WaitWhile(() => blockMan.throwNow == true);         //����
            yield return new WaitWhile(() => blockMan.blockDeleteNow == true);   //�u���b�N�폜
            yield return new WaitWhile(() => blockMan.blockChangeNow == true);   //�����u���b�N�؂�ւ�

            //�Q�[���I���H
            if (GAME_OVER || GAME_CLEAR) break;

            //1�s����
            StartCoroutine(blockMan.LineBlockGenerate(1));
        }
    }
}