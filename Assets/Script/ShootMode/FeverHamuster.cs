using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;


public class FeverHamuster : MonoBehaviour
{
    BoxCollider2D coll;         //フィーバーハムスターBoxCollider2D
    RectTransform tra;          //フィーバーハムスターRectTransform
    GameObject feverHamObj;     //フィーバーハムスターRectTransform
    RectTransform startObjTra;  //フィーバー開始オブジェクトRectTransform(フィーバー開始オブジェクト)

    float magnification;     //タップ操作時の修正値
    float differenceX;       //タップ操作時の修正値
    float differenceY;       //タップ操作時の修正値
    Vector2 defaultPos = new Vector2(0.0f, -530.0f);

    string objName;          //自身のタグ
    string[] blockTag;       //ブロックタグリスト
    BlockManager blockMan;   //BlockManager
    GameObject mainHamObj;   //メインハムスターオブジェクト
    Camera mainCamera;       //メインカメラ
    bool hamsterMove;        //移動開始フラグ

    //========================================================================
    //フィーバー開始(全消し)
    //========================================================================
    public IEnumerator FeverStart()
    {
        mainCamera  = Camera.main;
        objName     = this.gameObject.name;
        tra         = GetComponent<RectTransform>();
        coll        = GetComponent<BoxCollider2D>();
        feverHamObj = tra.GetChild(0).gameObject;
        startObjTra = tra.GetChild(1).GetComponent<RectTransform>();
        tra.anchoredPosition = defaultPos;
        feverHamObj.SetActive(false);

        magnification = CANVAS_WIDTH / Screen.width;
        differenceX   = CANVAS_WIDTH / 2.0f;
        differenceY   = CANVAS_HEIGHT / 2.0f;

        //ブロックタグ取得
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        blockMan   = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();
        mainHamObj = GameObject.FindWithTag("Hamster");

        //フィーバー開始オブジェクト画面中央まで移動
        float moveSpeed   = 15.0f;
        float acceleRate  = 1.0f;
        Vector2 targetPos = new Vector2(0.0f, 70.0f);
        float mvoeTime    = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //減速
        moveSpeed  = 15.0f;
        acceleRate = 0.9f;
        targetPos  = new Vector2(0.0f, 175.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //メインハムスター非表示
        mainHamObj.SetActive(false);
        feverHamObj.SetActive(true);

        //一旦下がる(加速)
        moveSpeed  = 1.5f;
        acceleRate = 1.05f;
        targetPos  = new Vector2(0.0f, 125.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //一旦下がる(減速)
        moveSpeed  = 3.0f;
        acceleRate = 0.98f;
        targetPos  = new Vector2(0.0f, 75.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //上昇開始(加速)
        moveSpeed  = 1.0f;
        acceleRate = 1.3f;
        targetPos  = new Vector2(0.0f, 100.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //画面外まで上昇
        moveSpeed  = 15.0f;
        acceleRate = 1.01f;
        targetPos  = new Vector2(0.0f, CANVAS_HEIGHT);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);
        Destroy(startObjTra.gameObject);

        //ハムスター移動可能
        hamsterMove = true;
        StartCoroutine(HamsterMove());

        //ブロックランダム生成開始
        StartCoroutine(blockMan.FeverStrat(this));
    }

    //========================================================================
    //ハムスター元の位置に戻る
    //========================================================================
    public IEnumerator ReturnFirstPosition()
    {
        hamsterMove      = false;
        float moveSpeed  = 12.0f;
        float acceleRate = 1.0f;
        float mvoeTime   = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
        StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
        yield return new WaitForSeconds(mvoeTime);

        //メインハムスター表示
        mainHamObj.SetActive(true);
        Destroy(this.gameObject);
    }

    //========================================================================
    //ハムスター移動
    //========================================================================
    IEnumerator HamsterMove()
    {
        bool push  = false;
        float maxX = CANVAS_WIDTH / 2.0f;
        while (hamsterMove)
        {
            yield return new WaitForFixedUpdate();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (hit2d && hit2d.transform.gameObject.name == objName)
                {
                    push = true;
                }
            }
            if (push)
            {
                Vector3 mousePos = Input.mousePosition;
                //座標の修正
                mousePos.x = mousePos.x * magnification - differenceX;
                mousePos.y = mousePos.y * magnification - differenceY;
                mousePos.z = 0.0f;
                // X, Y座標の範囲を制限する
                mousePos.x = Mathf.Clamp(mousePos.x, -maxX, maxX);
                mousePos.y = Mathf.Clamp(mousePos.y, defaultPos.y, defaultPos.y);
                tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, mousePos, 1.0f);
                if (Input.GetMouseButtonUp(0))
                {
                    push = false;
                }
            }
        }
    }
}
