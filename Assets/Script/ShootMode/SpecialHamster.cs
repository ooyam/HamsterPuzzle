using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;

public class SpecialHamster : MonoBehaviour
{
    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform blockBoxTra;

    [Header("ハムスタースプライト")]
    [SerializeField]
    Sprite[] hamsterSprite;   //0:通常 1:上を向く 2:横向き

    Image ima;               //スペシャルハムスターImage
    Image filterIma;         //スペシャルハムスターフィルターImage
    BoxCollider2D coll;      //スペシャルハムスターBoxCollider2D
    RectTransform tra;       //スペシャルハムスターRectTransform
    RectTransform parentTra; //スペシャルハムスターの親オブジェクトのRectTransform

    [System.NonSerialized]
    public float lowestLinePosY;  //現在の最低ブロック行のY座標
    [System.NonSerialized]
    bool specialAvailable;        //スペシャル使用可能状態？
    [System.NonSerialized]
    public bool specialHavestNow; //収穫中？

    string[] blockTag;        //ブロックタグリスト
    BlockManager blockMan;    //BlockManager

    void Start()
    {
        ima  = GetComponent<Image>();
        tra  = GetComponent<RectTransform>();
        coll = GetComponent<BoxCollider2D>();
        filterIma = tra.GetChild(0).GetComponent<Image>();
        parentTra = tra.parent.GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OneLineHarvest()));

        //ブロックタグ取得
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();

        StartCoroutine(EraseTenBlocks());//テスト
    }

    //========================================================================
    //スペシャル使用可能状態
    //========================================================================
    public IEnumerator EraseTenBlocks()
    {
        if (!specialAvailable)
        {
            //拡縮設定
            float scalingSpeed = 0.005f;  //拡大率変更速度
            float changeScale  = 1.05f;   //変更時の拡大率
            float endScale     = 1.0f;    //終了時の拡大率

            //点滅設定
            float colouringSpeed = 0.1f;    //点滅速度
            Color filterOn       = new Color(1.0f, 1.0f, 1.0f, 192.0f / 255.0f);   //フィルター白
            Color filterOff      = new Color(1.0f, 1.0f, 1.0f, 0.0f);              //フィルター透明

            //動作開始
            specialAvailable = true;
            float nowScale   = tra.localScale.x;
            bool change      = true;
            while (specialAvailable)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    if (change)
                    {
                        //拡大
                        if (nowScale >= changeScale) change = false;
                        tra.localScale = Vector3.MoveTowards(tra.localScale, Vector3.one * changeScale, scalingSpeed);

                        //フィルター白
                        filterIma.color = Color.Lerp(filterIma.color, filterOn, colouringSpeed);
                    }
                    else
                    {
                        //縮小
                        if (nowScale <= endScale) break;
                        tra.localScale = Vector3.MoveTowards(tra.localScale, Vector3.one * endScale, scalingSpeed);

                        //フィルター透明
                        filterIma.color = Color.Lerp(filterIma.color, filterOff, colouringSpeed);
                    }

                    //現在の拡大率更新
                    nowScale = tra.localScale.x;
                }

                //リセット
                tra.localScale  = Vector3.one * endScale;
                filterIma.color = filterOff;
                change = true;
            }

            //収穫開始
            specialHavestNow = true;
        }
    }

    //========================================================================
    //1行収穫
    //========================================================================
    IEnumerator OneLineHarvest()
    {
        if (specialAvailable)
        {
            //一部の動作中は終了するまで待機
            yield return new WaitWhile(() => blockMan.throwNow == true);         //投擲
            yield return new WaitWhile(() => blockMan.blockGenerateNow == true); //ブロック生成
            yield return new WaitWhile(() => blockMan.blockDeleteNow == true);   //ブロック削除
            yield return new WaitWhile(() => blockMan.blockChangeNow == true);   //投擲ブロック切り替え

            //収穫開始
            SPECIAL_HARVEST  = true;
            specialAvailable = false;
            yield return new WaitUntil(() => specialHavestNow == true);

            //現座標・Collider・scale取得
            Vector2 defaultPos      = tra.anchoredPosition;
            Vector2 defaultCollSize = coll.size;
            Vector2 defaultCollOff  = coll.offset;
            Vector3 defaultScale    = tra.localScale;

            //各動作遅延時間
            float delayTime = 0.5f;

            //ハムスターSprite変更
            tra.rotation   = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            ima.sprite     = hamsterSprite[1];
            tra.localScale = new Vector3(0.9f, 1.0f, 1.0f);
            yield return new WaitForSeconds(delayTime);
            ima.sprite     = hamsterSprite[2];
            tra.localScale = new Vector3(1.0f, 0.8f, 1.0f);

            //右端(画面外)に移動設定
            float moveSpeed   = 12.0f;
            float acceleRate  = 1.0f;
            float rightPosX   = CANVAS_WIDTH / 2.0f + 100.0f;
            Vector2 targetPos = new Vector2(rightPosX, defaultPos.y);
            float mvoeTime    = GetMoveTime(tra, moveSpeed, acceleRate, targetPos);

            //揺れ設定
            float rotSpeed = 2.0f;      //揺れ速度
            float maxRot   = 10.0f;     //揺れ角度
            int moveCount  = -1;        //1サイクル動作回数(カウントしない場合は - 1指定)
            float stopTime = 0.0f;      //停止時間
            int breakCount = -1;        //終了サイクル数(無限ループの場合は - 1指定)
            float endTime  = mvoeTime;  //揺れ終了時間(時間で止めない場合は - 1指定)

            //移動開始
            StartCoroutine(ShakeMovement(tra, rotSpeed, maxRot, moveCount, stopTime, breakCount, endTime));
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime + delayTime);

            //収穫ライン右端に移動し反転
            tra.SetParent(blockBoxTra, false);
            tra.anchoredPosition = new Vector2(rightPosX, lowestLinePosY);
            tra.rotation         = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            coll.size            = new Vector2(50.0f, 30.0f);
            coll.offset          = new Vector2(-30.0f, 0.0f);

            //左端に移動
            float leftPosX = -rightPosX;
            targetPos      = new Vector2(leftPosX, lowestLinePosY);
            mvoeTime       = GetMoveTime(tra, moveSpeed, acceleRate, targetPos);
            endTime        = mvoeTime;

            //移動開始
            StartCoroutine(ShakeMovement(tra, rotSpeed, maxRot, moveCount, stopTime, breakCount, endTime));
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime + delayTime);

            //元の場所の右端に移動
            tra.SetParent(parentTra, false);
            tra.anchoredPosition = new Vector2(rightPosX, defaultPos.y);

            //元の場所に移動
            mvoeTime = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
            endTime  = mvoeTime;

            //移動開始
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
            yield return new WaitForSeconds(mvoeTime);

            //ハムスターSprite変更
            ima.sprite = hamsterSprite[0];
            tra.anchoredPosition = defaultPos;
            coll.size            = defaultCollSize;
            coll.offset          = defaultCollOff;
            tra.localScale       = defaultScale;

            //終了判定
            SPECIAL_HARVEST  = false;
            specialHavestNow = false;
        }
        else
        {
            //収穫NG動作(左右揺れ)
            float shakeSpeed   = 20.0f;    //移動速度
            float shakeOffsetX = 20.0f;    //移動座標X
            float shakeOffsetY = 0.0f;     //移動座標Y
            int   shakeTimes   = 4;        //揺れ回数
            float delayTime    = 0.0f;     //移動間の遅延時間
            StartCoroutine(SlideShakeMovement(tra, shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime));
        }
    }

    //========================================================================
    //接触判定
    //========================================================================
    void OnTriggerEnter2D(Collider2D col)
    {
        if (SPECIAL_HARVEST)
        {
            GameObject connectObj = col.gameObject;
            string connectObjTag = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex) blockMan.BlockHarvest(connectObj);
        }
    }
}