using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootMode
{
    //野菜の種類
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
            StartCoroutine(LineBlocGenerateInterval());
        }

        //========================================================================
        //時間経過で1行生成指示
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
