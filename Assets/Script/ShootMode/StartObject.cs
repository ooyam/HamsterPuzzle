using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;
using static MoveFunction.ObjectMove;
using static ShootMode.ShootModeDefine;

public class StartObject : MonoBehaviour
{
    [Header("BackGround")]
    public RectTransform backGroundTra;
    RectTransform tra;
    SoundManager soundMan;
    bool walkSe;

    IEnumerator Start()
    {
        soundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

        //歩きSE
        soundMan.StartWalkSE_Shoot();
        walkSe = true;

        //移動開始
        tra = GetComponent<RectTransform>();
        float posY = tra.anchoredPosition.y;
        float movespeed = 7.0f;                                                       //移動速度
        float stopTime = GetMoveTime(tra, movespeed, 1.0f, new Vector2(0.0f, posY));  //移動時間計算
        StartCoroutine(ShakeMovement(tra, 1.0f, 10.0f, -1, 0.0f, -1, stopTime));      //揺れ
        yield return StartCoroutine(MoveMovement(tra, movespeed, 1.0f, new Vector2(0.0f, posY)));  //移動

        //ゲーム開始
        RectTransform hamsterTra = tra.GetChild(2).gameObject.GetComponent<RectTransform>();
        hamsterTra.SetParent(backGroundTra, true);
        hamsterTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GAME_START = true; //ゲーム開始フラグ

        //歩きSE停止
        soundMan.SE_Stop();
        walkSe = false;

        //パネル倒し
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

    void Update()
    {
        if (!GAME_START)
        {
            if (walkSe && SETTING_DISPLAY)
            {
                //歩きSE停止
                soundMan.SE_Stop();
                walkSe = false;
                soundMan.YesTapSE();
            }

            if (!walkSe && !SETTING_DISPLAY)
            {
                //歩きSE再生
                soundMan.StartWalkSE_Shoot();
                walkSe = true;
            }
        }
    }
}
