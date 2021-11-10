using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootMode
{
    //��؂̎��
    public enum VegetableType
    {
        Broccoli = 0,
        Cabbage,
        Carrot,
        Paprika,
        Pumpkin,
        Corn
    }

    public class TimeManager : MonoBehaviour
    {
        [Header("BlocManager")]
        public BlocManager blocMan;
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
            hamCon.gameStart = true;
            StartCoroutine(LineBlocGenerateInterval());
        }

        //========================================================================
        //���Ԍo�߂�1�s�����w��
        //========================================================================
        IEnumerator LineBlocGenerateInterval()
        {
            float generateTime = 30.0f;
            switch (stageNum)
            {
                case 10:
                    generateTime = 20.0f;
                    break;
                default:
                    break;
            }
            while (true)
            {
                yield return new WaitForSeconds(generateTime);
                StartCoroutine(blocMan.LineBlocGenerate(1));
            }
        }
    }
}
