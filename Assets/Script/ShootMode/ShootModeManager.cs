using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ShootModeManager : MonoBehaviour
    {
        [Header("背景オブジェクトTra")]
        [SerializeField]
        RectTransform backGroundTra;

        [Header("ゲームオーバープレハブ")]
        [SerializeField]
        GameObject gameOverObjPre;
        [System.NonSerialized]
        public bool gameOverObjDis = false;  //表示中フラグ

        [Header("ゲームクリアプレハブ")]
        [SerializeField]
        GameObject gameClearObjPre;

        [Header("リザルト画面プレハブ")]
        [SerializeField]
        GameObject resultScreenPre;

        void Awake()
        {
            FlagReset();
        }

        public IEnumerator GameOver()
        {
            if (!gameOverObjDis)
            {
                GAME_OVER = true;

                //ゲームオーバーオブジェクト生成
                gameOverObjDis = true;
                GameObject gameOverObj = Instantiate(gameOverObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameOverObj>().DirectGameOver(this));
                yield return new WaitWhile(() => gameOverObjDis == true);

                //リザルト画面表示
                GameObject resultScreenObj = Instantiate(resultScreenPre);
                resultScreenObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
            }
        }
    }
}