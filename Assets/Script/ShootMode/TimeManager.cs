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
        float elapsedTime  = 0.0f;
        float oneFrameTime = 0.02f;

        while (!GAME_OVER && !GAME_CLEAR)
        {
            yield return new WaitForFixedUpdate();

            //�ꕔ�̓��쒆�̓J�E���g�X�g�b�v(1�s���n���E�t�B�[�o�[�E�u���b�N�폜)
            if (!SPECIAL_HARVEST && !FEVER_START && !blockMan.blockDeleteNow)
                elapsedTime += oneFrameTime;

            //�ꕔ�̓��쒆�͏I������܂őҋ@(�����E�����u���b�N�؂�ւ�)
            if (!blockMan.throwNow && !blockMan.blockChangeNow)
            {
                if (BLOCK_GENERATE_TIME <= elapsedTime)
                {
                    //�Q�[���I���H
                    if (GAME_OVER || GAME_CLEAR) break;

                    //1�s����
                    StartCoroutine(blockMan.LineBlockGenerate(1));

                    //�o�ߎ��ԃ��Z�b�g
                    elapsedTime = 0.0f;
                }
            }
        }
    }
}