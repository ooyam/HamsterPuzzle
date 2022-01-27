using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using ShootMode_Tutorial;
using SoundFunction;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class FeverHamuster_Tutorial : MonoBehaviour
    {
        [Header("ハムスタースプライト")]
        [SerializeField]
        Sprite hamsterDefSpr;

        BoxCollider2D coll;         //フィーバーハムスターBoxCollider2D
        RectTransform tra;          //フィーバーハムスターRectTransform
        GameObject feverHamObj;     //フィーバーハムスターRectTransform
        RectTransform startObjTra;  //フィーバー開始オブジェクトRectTransform(フィーバー開始オブジェクト)

        float magnification;     //タップ操作時の修正値
        float differenceX;       //タップ操作時の修正値
        float differenceY;       //タップ操作時の修正値
        Vector2 defaultPos = new Vector2(0.0f, -530.0f);

        string objName;                   //自身のタグ
        string[] blockTag;                //ブロックタグリスト
        BlockManager_Tutorial blockMan;   //BlockManager
        ScoreManager_Tutorial scoreMan;   //ScoreManager
        SoundManager soundMan;            //SoundManager
        GameObject mainHamObj;            //メインハムスターBoxオブジェクト
        TutorialManager tutorialMan;      //チュートリアルマネージャー
        Camera mainCamera;                //メインカメラ
        bool hamsterMove;                 //移動開始フラグ

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
            blockMan    = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
            scoreMan    = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>();
            soundMan    = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            mainHamObj  = GameObject.FindWithTag("HamsterBox");
            tutorialMan = GameObject.Find("Tutorial").GetComponent<TutorialManager>();

            //SE
            soundMan.FeverStartSE_Shoot();

            //BGM
            StartCoroutine(soundMan.FeverBGM_Shoot());

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
            targetPos  = new Vector2(0.0f, 150.0f);
            mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
            StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime);

            //メインハムスター非表示
            mainHamObj.SetActive(false);
            feverHamObj.SetActive(true);

            //一旦下がる(加速)
            moveSpeed  = 1.5f;
            acceleRate = 1.05f;
            targetPos  = new Vector2(0.0f, 100.0f);
            mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
            StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime);

            //一旦下がる(減速)
            moveSpeed  = 3.0f;
            acceleRate = 0.98f;
            targetPos  = new Vector2(0.0f, 50.0f);
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

            //次の説明へ
            tutorialMan.NextDescriptionStart();

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
            //元の位置に戻る
            hamsterMove      = false;
            float moveSpeed  = 5.0f;
            float acceleRate = 1.0f;
            float mvoeTime   = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
            tra.GetChild(0).GetComponent<Image>().sprite = hamsterDefSpr;
            if (tra.anchoredPosition.x > defaultPos.x) tra.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
            yield return new WaitForSeconds(mvoeTime);

            //メインハムスター表示
            mainHamObj.SetActive(true);
            FEVER_START = false;

            //次の説明へ
            tutorialMan.NextDescriptionStart();
            Destroy(this.gameObject);
        }

        //========================================================================
        //ハムスター移動
        //========================================================================
        IEnumerator HamsterMove()
        {
            float maxX = PLAY_SCREEN_WIDTH / 2.0f;
            bool firstTap = true;
            while (hamsterMove)
            {
                yield return new WaitForFixedUpdate();
                if (firstTap && Input.GetMouseButtonDown(0))
                {
                    //手非表示
                    tutorialMan.HandHide();
                    firstTap = false;
                }
                if (Input.GetMouseButton(0))
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
                }
            }
        }

        //========================================================================
        //接触判定
        //========================================================================
        void OnTriggerEnter2D(Collider2D col)
        {
            //収穫成功
            if (FEVER_START)
            {
                GameObject connectObj = col.gameObject;
                string connectObjTag  = connectObj.tag;
                int tagIndex = Array.IndexOf(blockTag, connectObjTag);
                if (0 <= tagIndex)
                {
                    //SE
                    soundMan.FeverHarvestSE_Shoot();
                    blockMan.fallCompleteCount++;
                    scoreMan.HarvestVegetable(connectObjTag);
                    Destroy(connectObj);
                }
            }
        }
    }
}