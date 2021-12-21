using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;
using static ShootMode.ShootModeDefine;

public class StartObject : MonoBehaviour
{
    [Header("BackGround")]
    public RectTransform backGroundTra;
    RectTransform tra;

    void Start()
    {
        tra = GetComponent<RectTransform>();
        float posY = tra.anchoredPosition.y;

        float movespeed = 7.0f;                                                       //移動速度
        float stopTime = Mathf.Abs(tra.anchoredPosition.x) / movespeed * 0.02f;       //移動時間計算
        StartCoroutine(ShakeMovement(tra, 1.0f, 10.0f, -1, 0.0f, -1, stopTime));      //揺れ
        StartCoroutine(MoveMovement(tra, movespeed, 1.0f, new Vector2(0.0f, posY)));  //移動
        StartCoroutine(ObjectDelete(stopTime));
    }

    //========================================================================
    //自身の削除動作
    //========================================================================
    //waitTime; 削除開始までの待機時間
    //========================================================================
    IEnumerator ObjectDelete(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RectTransform hamsterTra = tra.GetChild(2).gameObject.GetComponent<RectTransform>();
        hamsterTra.SetParent(backGroundTra, true);
        hamsterTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GAME_START = true; //ゲーム開始フラグ

        float rotX = 0.0f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            rotX += 3.0f;
            tra.rotation = Quaternion.Euler(rotX, 0.0f, 0.0f);
            if (rotX >= 90.0f) break;
        }
        Destroy(this.gameObject);
    }
}
