using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ShootMode;
using GFramework;
using static ShootMode.ShootModeDefine;
using static ShootMode.ShootModeManager;
using static MoveFunction.ObjectMove;

public class BlockManager : MonoBehaviour
{
    [Header("ブロックの取得")]
    [SerializeField]
    GameObject[] blockPre;
    List<GameObject> blockObj    = new List<GameObject>();       //生成ブロックobject
    List<RectTransform> blockTra = new List<RectTransform>();    //生成ブロックRectTransform
    List<Rigidbody2D> blockRig   = new List<Rigidbody2D>();      //生成ブロックRigidbody2D
    List<int[]> blockPosIndex    = new List<int[]>();            //生成ブロック座標番号
    List<int> nowDeleteIndex     = new List<int>();              //削除中ブロックのオブジェクト番号リスト
    [System.NonSerialized]
    public int fallCompleteCount;                                //落下完了ブロック数
    int throwBlockIndex;                                         //投擲ブロックのリスト番号
    int nextThrowBlockIndex;                                     //次の投擲ブロックのリスト番号
    CircleCollider2D[] blockCollider = new CircleCollider2D[2];  //Collider 0:次投擲ブロック 1:投擲ブロック
    Color[] blockColor;                                          //ブロックの色
    string[] blockTag;                                           //ブロックのタグリスト

    [Header("シュートモードマネージャー")]
    [SerializeField]
    ShootModeManager ShootModeMan;

    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform blockBoxTra;
    float blockBoxHight;  //ブロックボックス高さ
    float blockPosFixY;   //ブロックボックス高さ / 2 - ブロック半径

    [Header("ハムスターボックス")]
    [SerializeField]
    Transform hamsterBoxTra;
    HamsterController hamsterScr;  //HamsterController

    [Header("次投擲ブロック表示ボード")]
    [SerializeField]
    RectTransform nextBlockBoardTra;

    [Header("スペシャルハムスタースクリプト")]
    [SerializeField]
    SpecialHamster speHamScr;

    [Header("エフェクトプレハブ")]
    public GameObject effectPre;

    int[] columnNum = new int[] { 9, 10 };     //1行の列数
    Vector2[][][] blockPos;                    //ブロック配置座標 0:パターン番号 1:行番号 2:列番号
    int generatePattern;                       //ライン生成するパターン番号
    int usingVegNum;                           //使用する野菜の数
    [System.NonSerialized]
    public int nowLineNum;                     //現在の行数
    [System.NonSerialized]
    public float blockPosY  = 103.8f;          //ブロック生成位置Y
    float[][] blockPosX     = new float[2][];  //ブロック生成位置X
    Vector2[] throwBlockPos = new Vector2[4];  //投擲ブロック生成座標
    Vector2 nextThrowBlockPos;                 //次の投擲ブロック生成座標
    [System.NonSerialized]
    public bool blockGenerateNow;              //生成中？
    [System.NonSerialized]
    public bool throwNow;                      //投擲中？
    [System.NonSerialized]
    public bool blockDeleteNow;                //ブロック削除中？
    [System.NonSerialized]
    public bool blockChangeNow;                //ブロック交換中？
    [System.NonSerialized]
    public float blockDiameter = 120.0f;       //ブロック直径
    float[] blockRotDirecting  = new float[] { 90.0f, 270.0f };  //演出用ブロック角度

    IEnumerator Start()
    {
        hamsterScr  = hamsterBoxTra.GetChild(0).gameObject.GetComponent<HamsterController>();
        usingVegNum = useVegNum;
        throwBlockPos[0]  = new Vector2(70.0f, -10.0f);
        throwBlockPos[1]  = new Vector2(-throwBlockPos[0].x, throwBlockPos[0].y);
        throwBlockPos[2]  = new Vector2(70.0f, -50.0f);
        throwBlockPos[3]  = new Vector2(-throwBlockPos[2].x, throwBlockPos[2].y);
        nextThrowBlockPos = new Vector2(0.0f, -30.0f);

        //ブロックの色取得
        int blockPreCount = blockPre.Length;
        blockColor = new Color[blockPreCount];
        blockTag   = new string[blockPreCount];
        for (int blockPreInd = 0; blockPreInd < blockPreCount; blockPreInd++)
        {
            blockColor[blockPreInd] = blockPre[blockPreInd].transform.GetChild(0).GetComponent<GFramework.SimpleRoundedImage>().color;
            blockTag[blockPreInd]   = blockPre[blockPreInd].tag;
        }

        //ブロック配置座標指定
        float blockRadius = blockDiameter / 2.0f;　//ブロック半径
        blockBoxHight     = blockBoxTra.rect.height;
        blockPosFixY      = blockBoxHight / 2.0f + blockRadius;
        float[] posXFix   = new float[] { (columnNum[0] - 1) * blockRadius, (columnNum[1] - 1) * blockRadius };
        int patternNum    = columnNum.Length;
        blockPos = new Vector2[patternNum][][];
        for (int ind_1 = 0; ind_1 < patternNum; ind_1++)
        {
            blockPos[ind_1] = new Vector2[BLOCK_MAX_LINE_NUM][];
            for (int ind_2 = 0; ind_2 < BLOCK_MAX_LINE_NUM; ind_2++)
            {
                blockPos[ind_1][ind_2] = new Vector2[columnNum[ind_1]];
                for (int ind_3 = 0; ind_3 < columnNum[ind_1]; ind_3++)
                {
                    float posX = blockDiameter * ind_3 - posXFix[ind_1];             //X座標計算
                    float posY = -blockPosY * ind_2 - blockRadius + blockPosFixY;    //Y座標計算
                    blockPos[ind_1][ind_2][ind_3] = new Vector2(posX, posY);
                }
            }
        }

        //ブロックを3列生成
        int firstGenerateLinesNum = 3;
        StartCoroutine(LineBlockGenerate(firstGenerateLinesNum));

        //投擲用ブロック生成
        NextThrowBlockGenerate();

        //ゲーム開始待ち
        yield return new WaitUntil(() => GAME_START == true);
        StartCoroutine(ThrowBlockGenerate());
    }

    //========================================================================
    //ブロック生成
    //========================================================================
    //blockPreIndex; ブロックのプレハブ番号
    //throwBloc;     投擲ブロック？
    //========================================================================
    int BlockGenerate(int blockPreIndex, bool throwBlock)
    {
        GameObject blockObject     = Instantiate(blockPre[blockPreIndex]);
        RectTransform blockRectTra = blockObject.GetComponent<RectTransform>();
        if (!throwBlock)
        {
            blockRectTra.SetParent(blockBoxTra, false);
            blockRectTra.SetSiblingIndex(0);
        }
        blockObj.Add(blockObject);
        blockTra.Add(blockRectTra);
        blockRig.Add(blockObject.GetComponent<Rigidbody2D>());
        blockPosIndex.Add(new int[0]);
        return blockTra.Count - 1;
    }

    //========================================================================
    //投擲ブロック生成
    //========================================================================
    IEnumerator ThrowBlockGenerate()
    {
        //次投擲ブロック回転
        blockChangeNow = true;
        float waitTime = ThrowBlockChangeDirecting(new int[] { nextThrowBlockIndex }, new Vector3(0.0f, blockRotDirecting[0], 0.0f));
        yield return new WaitForSeconds(waitTime);

        //投擲ブロックを持つ
        HaveThrowBlock();

        //次の投擲ブロック生成
        NextThrowBlockGenerate();

        //各ブロック回転表示
        blockTra[throwBlockIndex].localRotation     = Quaternion.Euler(0.0f, blockRotDirecting[1], 0.0f);
        blockTra[nextThrowBlockIndex].localRotation = Quaternion.Euler(0.0f, blockRotDirecting[1], 0.0f);
        waitTime = ThrowBlockChangeDirecting(new int[] { throwBlockIndex, nextThrowBlockIndex }, Vector3.zero);
        yield return new WaitForSeconds(waitTime);
        blockChangeNow = false;

        //ブロック最大ライン数更新
        NowLineNumUpdate();
    }

    //========================================================================
    //次投擲ブロック生成
    //========================================================================
    void NextThrowBlockGenerate()
    {
        //生成
        int blockGeneIndex = UnityEngine.Random.Range(0, usingVegNum);
        nextThrowBlockIndex = BlockGenerate(blockGeneIndex, true);
        blockTra[nextThrowBlockIndex].SetParent(nextBlockBoardTra, false);
        blockTra[nextThrowBlockIndex].anchoredPosition = nextThrowBlockPos;
        blockObj[nextThrowBlockIndex].AddComponent<BlockController>();
        blockCollider[0] = blockObj[nextThrowBlockIndex].GetComponent<CircleCollider2D>();
    }

    //========================================================================
    //次投擲ブロックを持つ
    //========================================================================
    void HaveThrowBlock()
    {
        throwBlockIndex = nextThrowBlockIndex;
        blockTra[throwBlockIndex].SetParent(hamsterBoxTra, false);
        blockTra[throwBlockIndex].SetSiblingIndex(0);
        blockTra[throwBlockIndex].anchoredPosition = (hamsterScr.spriteNum % 2 == 0) ? throwBlockPos[0] : throwBlockPos[1];
        blockCollider[1] = blockObj[throwBlockIndex].GetComponent<CircleCollider2D>();
        int prehubInd = Array.IndexOf(blockTag, blockObj[throwBlockIndex].tag);
        hamsterScr.nowBlockColor = blockColor[prehubInd];
    }

    //========================================================================
    //次投擲ブロックタップ
    //========================================================================
    public void NextThrowBlockTap()
    {
        if (GAME_START  && !GAME_OVER && !SPECIAL_HARVEST && !FEVER_START && !SETTING_DISPLAY && !throwNow && !blockDeleteNow && !blockChangeNow)
            StartCoroutine(ThrowBlockChange());
    }

    //========================================================================
    //投擲ブロック交換
    //========================================================================
    IEnumerator ThrowBlockChange()
    {
        blockChangeNow = true;

        //交換演出
        int[] blockIndexArray = new int[] { throwBlockIndex, nextThrowBlockIndex };
        float waitTime = ThrowBlockChangeDirecting(blockIndexArray, new Vector3(0.0f, blockRotDirecting[0], 0.0f));
        yield return new WaitForSeconds(waitTime);

        //次投擲ブロックを持つ
        int nowThorwBlockIndex = throwBlockIndex;
        HaveThrowBlock();

        //投擲ブロックを次投擲ブロックに置換
        nextThrowBlockIndex = nowThorwBlockIndex;
        blockTra[nextThrowBlockIndex].SetParent(nextBlockBoardTra, false);
        blockTra[nextThrowBlockIndex].anchoredPosition = nextThrowBlockPos;
        blockCollider[0] = blockObj[nextThrowBlockIndex].GetComponent<CircleCollider2D>();

        //各ブロックをの角度を270に設定
        blockTra[throwBlockIndex].localRotation     = Quaternion.Euler(0.0f, blockRotDirecting[1], 0.0f);
        blockTra[nextThrowBlockIndex].localRotation = Quaternion.Euler(0.0f, blockRotDirecting[1], 0.0f);

        //交換演出
        waitTime = ThrowBlockChangeDirecting(blockIndexArray, Vector3.zero);
        yield return new WaitForSeconds(waitTime);

        blockChangeNow = false;
    }

    //========================================================================
    //投擲ブロック交換演出
    //========================================================================
    //indexArray;  動作オブジェクトのブロック番号
    //stopRot;     停止角度
    //return;      所要時間
    //========================================================================
    float ThrowBlockChangeDirecting(int[] blockIndexArray, Vector3 stopRot)
    {
        //動作ブロック取得
        int blockNum = blockIndexArray.Length;
        RectTransform[] traArray = new RectTransform[blockNum];
        for (int arrayIndex = 0; arrayIndex < blockNum; arrayIndex++)
        { traArray[arrayIndex] = blockTra[blockIndexArray[arrayIndex]]; }

        //移動開始
        Vector3 rotSpeed = new Vector3(0.0f, 10.0f, 0.0f);
        float waitTime   = GetRotateMoveTime(traArray[0], rotSpeed, stopRot);
        StartCoroutine(RotateMovement(traArray, rotSpeed, stopRot));

        return waitTime;
    }

    //========================================================================
    //投擲ブロック座標反転
    //========================================================================
    public void ThrowBlockPosChange(int posIndex)
    {
        blockTra[throwBlockIndex].anchoredPosition = throwBlockPos[posIndex];
    }

    //========================================================================
    //ブロック指定行数生成指示
    //========================================================================
    //generatLineNum; ライン生成数
    //========================================================================
    public IEnumerator LineBlockGenerate(int generatLineNum)
    {
        //---------------------------------------------
        //指定行数ループ
        //---------------------------------------------
        for (int lineIndex = 0; lineIndex < generatLineNum; lineIndex++)
        {
            //---------------------------------------------
            //一部の動作中は終了するまで待機
            //---------------------------------------------
            yield return new WaitWhile(() => SPECIAL_HARVEST == true);   //1行収穫中
            yield return new WaitWhile(() => throwNow == true);          //投擲
            yield return new WaitWhile(() => blockDeleteNow == true);    //ブロック削除
            yield return new WaitWhile(() => blockChangeNow == true);    //投擲ブロック切り替え

            //生成パターン数設定
            int patternNum = (int)Mathf.Floor(columnNum[0] / 2);
            int[] geneInd = new int[patternNum];

            //---------------------------------------------
            //同じブロック2つを1組として生成
            //---------------------------------------------
            for (int genePattIndex = 0; genePattIndex < patternNum; genePattIndex++)
            {
                int blockGeneIndex = UnityEngine.Random.Range(0, usingVegNum);
                geneInd[genePattIndex] = blockGeneIndex;
            }

            //---------------------------------------------
            //ブロック生成
            //---------------------------------------------
            int blockPosThirdIndex = (generatePattern == 0) ? 0 : 1;
            int outputThreeInd = (generatePattern == 0) ? UnityEngine.Random.Range(0, patternNum) : -1;  //ループ何回目に3つ1組にするか
            List<int> generateObjInd = new List<int>();
            for (int patternIndex = 0; patternIndex < patternNum; patternIndex++)
            {
                int generatezNum = (outputThreeInd == patternIndex) ? 3 : 2;
                for (int i = 0; i < generatezNum; i++)
                {
                    int objIndex = BlockGenerate(geneInd[patternIndex], false);                             //ブロック生成
                    blockTra[objIndex].anchoredPosition = blockPos[generatePattern][0][blockPosThirdIndex]; //ブロック座標指定
                    blockPosIndex[objIndex] = new int[] { generatePattern, 0, blockPosThirdIndex };         //ブロックの座標の保存
                    generateObjInd.Add(objIndex);
                    blockPosThirdIndex++;
                }
            }
            blockGenerateNow = true;

            //colliderをアクティブ(Ray接触対策)
            yield return null;
            foreach (int geneObjInd in generateObjInd)
            { blockObj[geneObjInd].GetComponent<CircleCollider2D>().enabled = true; }
            generatePattern = (generatePattern == 0) ? 1 : 0;

            //---------------------------------------------
            //一列下げる
            //---------------------------------------------
            StartCoroutine(LineDown());
            yield return new WaitWhile(() => blockGenerateNow == true);
            yield return new WaitForSeconds(0.5f);  //0.5秒遅延

            //ブロック最大ライン数更新
            NowLineNumUpdate();
            if (GAME_OVER) break;
        }
    }

    //========================================================================
    //一行下げる
    //========================================================================
    IEnumerator LineDown()
    {
        float oneFrameTime = 0.02f;           //1フレームの時間
        float speed        = 0.4f;            //下降速度
        float boundHigh    = 30.0f;           //跳ね返り高さ
        bool bound         = false;           //跳ね返り中？
        bool loopEnd       = false;           //下降終了？
        int objCount       = blockObj.Count;  //オブジェクトの数

        //---------------------------------------------
        //座標番号リスト更新
        //---------------------------------------------
        for (int posIndex = 0; posIndex < blockPosIndex.Count; posIndex++)
        { if (throwBlockIndex != posIndex && nextThrowBlockIndex != posIndex) blockPosIndex[posIndex][1]++; }

        //一行下げる
        while (true)
        {
            if (!bound)
            {
                //---------------------------------------------
                //下降
                //---------------------------------------------
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if(throwBlockIndex != objIndex && nextThrowBlockIndex != objIndex && nowDeleteIndex.IndexOf(objIndex) < 0)
                    {
                        Vector2 targetPos = blockPos[blockPosIndex[objIndex][0]][blockPosIndex[objIndex][1]][blockPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y - boundHigh;
                        blockTra[objIndex].anchoredPosition = Vector2.Lerp(blockTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blockTra[objIndex].anchoredPosition.y < targetPosY + 5.0f) bound = true;
                    }
                }
            }
            else
            {
                //---------------------------------------------
                //跳ね返り後上昇
                //---------------------------------------------
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if (throwBlockIndex != objIndex && nextThrowBlockIndex != objIndex && nowDeleteIndex.IndexOf(objIndex) < 0)
                    {
                        Vector2 targetPos = blockPos[blockPosIndex[objIndex][0]][blockPosIndex[objIndex][1]][blockPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y + boundHigh;
                        blockTra[objIndex].anchoredPosition = Vector2.Lerp(blockTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blockTra[objIndex].anchoredPosition.y > targetPos.y) loopEnd = true;
                    }
                }
                if (loopEnd) break;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }

        //定位置に戻す
        for (int objIndex = 0; objIndex < objCount; objIndex++)
        {
            if (throwBlockIndex != objIndex && nextThrowBlockIndex != objIndex && nowDeleteIndex.IndexOf(objIndex) < 0)
                blockTra[objIndex].anchoredPosition = blockPos[blockPosIndex[objIndex][0]][blockPosIndex[objIndex][1]][blockPosIndex[objIndex][2]];
        }
        blockGenerateNow = false;
    }

    //========================================================================
    //ブロックを投げる
    //========================================================================
    //linePoints; 投擲起動頂点座標
    //========================================================================
    public IEnumerator BlockThrow(Vector3[] linePoints)
    {
        throwNow = true;
        blockCollider[1].enabled = true;
        float oneFrameTime = 0.02f;
        float throwSpeed   = 50.0f;
        float maxRangeFix  = 60.0f;
        int targetIndex    = 1;
        int pointsCount    = linePoints.Length;
        while (throwNow)
        {
            Vector3 throwBlockPos = blockTra[throwBlockIndex].anchoredPosition;
            blockTra[throwBlockIndex].anchoredPosition = Vector3.MoveTowards(throwBlockPos, linePoints[targetIndex], throwSpeed);
            float[] endPos = new float[]{ linePoints[targetIndex].x + maxRangeFix, linePoints[targetIndex].x - maxRangeFix };
            if (throwBlockPos.x <= endPos[0] && throwBlockPos.x >= endPos[1])
            {
                if(pointsCount - 1 > targetIndex) targetIndex++;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }
    }

    //========================================================================
    //ブロック接続
    //========================================================================
    //obj; 接続するブロック
    //========================================================================
    public void BlockConnect(GameObject obj)
    {
        //ブロックボックスの子オブジェクトに変更
        blockTra[throwBlockIndex].SetParent(blockBoxTra, true);

        int conObjIndex       = blockObj.IndexOf(obj);                         //接続ブロックの番号取得
        int[] conObjPosIndex  = blockPosIndex[conObjIndex];                    //接続ブロックの座標番号
        Vector3 conObjPos     = blockTra[conObjIndex].anchoredPosition;        //接続ブロックの座標
        Vector3 throwBlockPos = blockTra[throwBlockIndex].anchoredPosition;    //投擲ブロックの座標

        float posJudge   = blockDiameter / 3.0f;                               //配置判定座標
        bool placedOn    = throwBlockPos.y >= conObjPos.y + posJudge;          //上に配置
        bool placedUnder = throwBlockPos.y <= conObjPos.y - posJudge;          //下に配置
        bool placedRight = throwBlockPos.x >= conObjPos.x;                     //右側に接触？
        bool conBlockPatternTen = conObjPosIndex[0] == 1;                      //接続ブロックが10列パターン？

        int[] arrangementPos = new int[3]; //投擲ブロック配置座標 0:パターン番号 1:行番号 2:列番号

        //パターン指定
        if (placedUnder || placedOn) arrangementPos[0] = (conBlockPatternTen) ? 0 : 1;
        else arrangementPos[0] = conObjPosIndex[0];

        //行指定
        if (placedUnder)   arrangementPos[1] = conObjPosIndex[1] + 1;  //下の行にセット
        else if (placedOn) arrangementPos[1] = conObjPosIndex[1] - 1;  //上の行にセット
        else               arrangementPos[1] = conObjPosIndex[1];      //同じ行にセット

        //列指定
        if (placedUnder || placedOn)
        {
            //10列パターン
            if (conBlockPatternTen) arrangementPos[2] = (placedRight) ? conObjPosIndex[2] : conObjPosIndex[2] - 1;
            //9列パターン
            else arrangementPos[2] = (placedRight) ? conObjPosIndex[2] + 1 : conObjPosIndex[2];
        }
        else arrangementPos[2] = (placedRight) ? conObjPosIndex[2] + 1 : conObjPosIndex[2] - 1; //同列配置

        //投擲ブロック停止座標指定
        blockTra[throwBlockIndex].anchoredPosition = blockPos[arrangementPos[0]][arrangementPos[1]][arrangementPos[2]];
        blockPosIndex[throwBlockIndex] = arrangementPos;

        //ブロック削除
        AdjacentSameTagBlockJudgment(throwBlockIndex);

        //投擲終了
        throwNow = false;
    }

    //========================================================================
    //ブロック上限接触
    //========================================================================
    public void UpperLimitConnect()
    {
        //投擲ブロック停止座標設定
        float throwblockPosX = blockTra[throwBlockIndex].anchoredPosition.x;
        int refPatNum = (generatePattern == 0) ? 1 : 0;
        int arrangementColumnIndex = 0;
        float provisionalPosX = CANVAS_WIDTH;
        int index = 0;
        foreach (Vector2 refPos in blockPos[refPatNum][1])
        {
            float offsetX = Mathf.Abs(refPos.x - throwblockPosX);
            if (offsetX < provisionalPosX)
            {
                provisionalPosX = offsetX;
                arrangementColumnIndex = index;
            }
            index++;
        }

        //ブロックボックスの子オブジェクトに変更
        blockTra[throwBlockIndex].SetParent(blockBoxTra, true);
        
        //座標指定
        blockTra[throwBlockIndex].anchoredPosition = blockPos[refPatNum][1][arrangementColumnIndex];
        blockPosIndex[throwBlockIndex] = new int[] { refPatNum, 1, arrangementColumnIndex };

        //ブロック削除
        AdjacentSameTagBlockJudgment(throwBlockIndex);

        //投擲終了
        throwNow = false;
    }

    //========================================================================
    //隣接同タグブロック判定
    //========================================================================
    //index; 基準のブロックの番号
    //========================================================================
    void AdjacentSameTagBlockJudgment(int index)
    {
        List<int> referenceBlockIndexList = new List<int>();  //参照するブロックのリスト
        List<int> scannedIndexList        = new List<int>();  //検索済リスト
        referenceBlockIndexList.Add(index);                   //初期基準ブロック追加
        string referenceTag = blockObj[index].tag;            //基準タグ

        //隣接した同タグブロックをすべて取得
        while (true)
        {
            //追加削除ブロックリスト
            List<int> addDeleteIndexList = new List<int>();

            foreach (int refIndex in referenceBlockIndexList)
            {
                //検索済みの場合は処理しない
                if (scannedIndexList.IndexOf(refIndex) >= 0) continue;
                else scannedIndexList.Add(refIndex);

                //追加削除リスト(隣接+同タグ)取得
                List<int[]> adjacentPosList = AdjacentAcquisition(refIndex);

                //隣接ブロック座標番号と生成ブロック座標番号の照合
                foreach (int[] adjacentArray in adjacentPosList)
                {
                    foreach (int[] posIndex in blockPosIndex)
                    {
                        bool allTrue = true;
                        for (int arrayIndex = 0; arrayIndex < posIndex.Length; arrayIndex++)
                        {
                            if (adjacentArray[arrayIndex] != posIndex[arrayIndex])
                            {
                                allTrue = false;
                                break;
                            }
                        }

                        //すべての値が等しい場合
                        if (allTrue)
                        {
                            int blockIndex = blockPosIndex.IndexOf(posIndex);
                            //同タグの場合追加
                            if (blockObj[blockIndex].tag == referenceTag)
                                addDeleteIndexList.Add(blockIndex);
                        }
                    }
                }
            }

            //新たな削除ブロックがなかった場合は終了
            bool add = false;
            foreach (int addIndex in addDeleteIndexList)
            {
                if (referenceBlockIndexList.IndexOf(addIndex) < 0)
                {
                    referenceBlockIndexList.Add(addIndex);
                    add = true;
                }
            }

            //次投擲ブロックは消さない
            referenceBlockIndexList.Remove(nextThrowBlockIndex);
            if (!add) break;
        }

        //配列に変換して削除実行
        if (referenceBlockIndexList.Count > 1)
        {
            //削除中リスト更新
            nowDeleteIndex = referenceBlockIndexList;

            //削除実行
            int[] deleteBlocks = referenceBlockIndexList.ToArray();
            StartCoroutine(BlockDeleteStart(deleteBlocks, true));
        }
        //投擲ブロック生成
        else StartCoroutine(ThrowBlockGenerate());
    }

    //========================================================================
    //自由落下ブロック判定
    //========================================================================
    //return; 自由落下ブロックの有無
    //========================================================================
    bool FreeFallBlockJudgment()
    {
        int blockObjCount            = blockObj.Count;   //現在のブロック総数
        List<int> activeBlockList    = new List<int>();  //削除予定の無いブロックリスト
        List<int> topLineConnectList = new List<int>();  //1行目(最上段)と接続されているブロックリスト
        List<int> scannedIndexList   = new List<int>();  //検索済リスト
        List<int> deleteIndexList    = new List<int>();  //削除リスト

        for (int objIndex = 0; objIndex < blockObjCount; objIndex++)
        {
            //削除予定ブロック判定
            if(nowDeleteIndex.IndexOf(objIndex) < 0 && nextThrowBlockIndex != objIndex)
            {
                //削除予定の無いブロックリスト取得
                if (throwBlockIndex != objIndex)
                    activeBlockList.Add(objIndex);

                //1行目のブロック番号取得
                if (blockPosIndex[objIndex][1] == 1)
                    topLineConnectList.Add(objIndex);
            }
        }

        //1行目のブロックの隣接ブロック取得
        while (true)
        {
            //追加隣接ブロックリスト
            List<int> addAdjacentIndexList = new List<int>();

            foreach (int refIndex in topLineConnectList)
            {
                //検索済みの場合は処理しない
                if (scannedIndexList.IndexOf(refIndex) >= 0) continue;
                else scannedIndexList.Add(refIndex);

                //追加隣接ブロックリスト取得
                List<int[]> adjacentPosList = AdjacentAcquisition(refIndex);

                //隣接ブロック座標番号と生成ブロック座標番号の照合
                foreach (int[] adjacentArray in adjacentPosList)
                {
                    foreach (int[] posIndex in blockPosIndex)
                    {
                        bool allTrue = true;
                        for (int arrayIndex = 0; arrayIndex < posIndex.Length; arrayIndex++)
                        {
                            if (adjacentArray[arrayIndex] != posIndex[arrayIndex])
                            {
                                allTrue = false;
                                break;
                            }
                        }

                        //すべての値が等しい場合
                        if (allTrue)
                        {
                            int blockIndex = blockPosIndex.IndexOf(posIndex);
                            //削除予定ブロックでない場合追加
                            if (nowDeleteIndex.IndexOf(blockIndex) < 0)
                                addAdjacentIndexList.Add(blockIndex);
                        }
                    }
                }
            }

            //新たな隣接ブロックがなかった場合は終了
            bool add = false;
            foreach (int addIndex in addAdjacentIndexList)
            {
                if (topLineConnectList.IndexOf(addIndex) < 0)
                {
                    topLineConnectList.Add(addIndex);
                    add = true;
                }
            }

            //次投擲ブロックは消さない
            topLineConnectList.Remove(nextThrowBlockIndex);
            if (!add) break;
        }

        //1行目と接続されていないブロック番号の取得
        foreach (int actIndex in activeBlockList)
        { if (topLineConnectList.IndexOf(actIndex) < 0) deleteIndexList.Add(actIndex); }

        //配列に変換して削除実行
        if (deleteIndexList.Count > 0)
        {
            //削除中リストに追加
            nowDeleteIndex.AddRange(deleteIndexList);

            //削除実行
            int[] deleteBlocks = deleteIndexList.ToArray();
            StartCoroutine(BlockDeleteStart(deleteBlocks, false));

            //削除ブロック有判定
            return true;
        }
        else return false;
    }

    //========================================================================
    //隣接ブロック取得
    //========================================================================
    //index;  基準のブロックの番号
    //return; 隣接ブロック番号リスト
    //========================================================================
    List<int[]> AdjacentAcquisition(int index)
    {
        List<int[]> adjacentPosList = new List<int[]>(); //隣接ブロック座標番号リスト
        List<int> adjacentIndexList = new List<int>();   //隣接ブロック番号リスト
        int[] refPosInd = blockPosIndex[index];          //基準ブロックの座標番号
        int columnType  = refPosInd[0];                  //配置タイプ

        bool minLine   = refPosInd[1] == 1;                          //最上段?
        bool maxLine   = refPosInd[1] == BLOCK_MAX_LINE_NUM;         //最下段?
        bool maxColumn = refPosInd[2] == columnNum[columnType] - 1;  //最右列?
        bool minColumn = refPosInd[2] == 0;                          //最左列?

        //基準ブロックが9列パターンの場合
        if (columnType == 0)
        {
            int pattern = 1;
            if (!minLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] - 1, refPosInd[2] + 1 });  //右上
            if (!maxLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] + 1, refPosInd[2] + 1 });  //右下
            if (!minLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] - 1, refPosInd[2]     });  //左上
            if (!maxLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] + 1, refPosInd[2]     });  //左下
        }
        //基準ブロックが10列パターンの場合
        else
        {
            int pattern = 0;
            if (!minLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] - 1, refPosInd[2]     });  //右上
            if (!maxLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] + 1, refPosInd[2]     });  //右下
            if (!minLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] - 1, refPosInd[2] - 1 });  //左上
            if (!maxLine) adjacentPosList.Add(new int[] { pattern, refPosInd[1] + 1, refPosInd[2] - 1 });  //左下
        }
        //共通
        if (!maxColumn) adjacentPosList.Add(new int[] { columnType, refPosInd[1], refPosInd[2] + 1 });  //右
        if (!minColumn) adjacentPosList.Add(new int[] { columnType, refPosInd[1], refPosInd[2] - 1 });  //左

        return adjacentPosList;
    }

    //========================================================================
    //ブロック削除開始処理
    //========================================================================
    //deleteObjIndex; 削除ブロック番号
    //connect;        接触削除？
    //========================================================================
    IEnumerator BlockDeleteStart(int[] deleteObjIndex, bool connect)
    {
        blockDeleteNow     = true;   //ブロック削除中フラグ
        bool blockDelete   = true;   //ブロックリスト削除実施フラグ
        float oneFrameTime = 0.02f;  //1フレームの時間

        //拡縮設定
        Vector3 scalingSpeed  = new Vector3(0.05f, 0.05f, 0.05f);  //拡縮速度
        float changeScale     = 1.5f;   //変更後の拡大率
        float defaultScale    = 1.0f;   //初期拡大率
        int   scalingTimes    = 1;      //拡縮回数
        float scalingWaitTime = GetScaleChangeTime(blockTra[0], scalingSpeed, changeScale, defaultScale, scalingTimes);  //拡縮待機時間

        //左右揺れ設定
        float shakeSpeed    = 20.0f;    //移動速度
        float shakeOffsetX  = 20.0f;    //移動座標X
        float shakeOffsetY  = 0.0f;     //移動座標Y
        int shakeTimes      = 4;        //揺れ回数
        float delayTime     = 0.0f;     //移動間の遅延時間
        float shakeWaitTime = GetSlideShakeTime(blockTra[0], shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime);  //揺れ待機時間

        //時間差設定
        int delObjCount     = deleteObjIndex.Length;   //削除ブロック数
        float[] indexArray  = new float[delObjCount];  //インデックス番号
        float[] DirectWait  = new float[delObjCount];  //演出開始時間
        float[] fallWait    = new float[delObjCount];  //落下開始時間
        bool[] directingEnd = new bool[delObjCount];   //落下前演出終了？
        bool[] fallStart    = new bool[delObjCount];   //落下開始？
        for (int index = 0; index < delObjCount; index++)
        {
            indexArray[index]   = index;
            DirectWait[index]   = (index == 0) ? 0.0f : UnityEngine.Random.Range(0.0f, 0.2f) + DirectWait[index - 1];
            fallWait[index]     = DirectWait[index] + ((connect) ? scalingWaitTime : shakeWaitTime) + 0.1f;
            directingEnd[index] = false;
            fallStart[index]    = false;

            //子オブジェクトインデックス番号最後尾に変更
            blockTra[deleteObjIndex[index]].SetSiblingIndex(blockBoxTra.childCount - 1);
        }

        int loopTimes     = 0;  //処理回数
        float elapsedTime = 0;  //経過時間
        foreach (int delInd in deleteObjIndex)
        {
            while (true)
            {
                //落下前演出
                if (!directingEnd[loopTimes] && elapsedTime >= DirectWait[loopTimes])
                {
                    directingEnd[loopTimes] = true;
                    if (connect) StartCoroutine(ScaleChange(blockTra[delInd], scalingSpeed, changeScale, defaultScale, scalingTimes));        //ブロック拡縮
                    else StartCoroutine(SlideShakeMovement(blockTra[delInd], shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime)); //ブロック左右揺れ
                }

                //落下開始
                for (int index = 0; index <= loopTimes; index++)
                {
                    if (!fallStart[index] && elapsedTime > fallWait[index])
                    {
                        blockRig[deleteObjIndex[index]].bodyType = RigidbodyType2D.Dynamic;
                        blockRig[deleteObjIndex[index]].gravityScale = 1.5f;
                        fallStart[index] = true;
                    }
                }

                elapsedTime += oneFrameTime;
                if (loopTimes == delObjCount - 1)
                { if (Array.IndexOf(fallStart, false) < 0) break; }
                else if (directingEnd[loopTimes]) break;

                yield return new WaitForFixedUpdate();
            }
            loopTimes++;
        }

        //自由落下ブロック判定
        yield return new WaitForSeconds(0.2f);
        if (connect) blockDelete = !FreeFallBlockJudgment();

        //接触削除時､自由落下ブロックが追加で発生した場合は実施しない
        if (blockDelete)
        {
            //ブロック削除待機
            yield return new WaitUntil(() => (nowDeleteIndex.Count <= fallCompleteCount) == true);

            //次投擲ブロックの保持
            GameObject nextThrowBlockObj = blockObj[nextThrowBlockIndex];

            //削除
            BlockDelete(nowDeleteIndex.ToArray());

            //クリア判定
            if (GAME_CLEAR)
            {
                StartCoroutine(ShootModeMan.GameClear());
            }
            else
            {
                //次の投擲ブロック番号更新
                nextThrowBlockIndex = blockObj.IndexOf(nextThrowBlockObj);

                //投擲ブロック生成
                StartCoroutine(ThrowBlockGenerate());

                //削除中リストリセット
                nowDeleteIndex.Clear();

                //ブロック削除中フラグリセット
                blockDeleteNow = false;
            }

            //落下完了カウントリセット
            fallCompleteCount = 0;
        }
    }

    //========================================================================
    //ブロック削除
    //========================================================================
    //objIndex; 削除ブロック番号
    //========================================================================
    void BlockDelete(int[] objIndex)
    {
        Array.Sort(objIndex);
        Array.Reverse(objIndex);
        foreach (int delInd in objIndex)
        {
            blockPosIndex.RemoveAt(delInd);
            blockTra.RemoveAt(delInd);
            blockRig.RemoveAt(delInd);
            Destroy(blockObj[delInd]);
            blockObj.RemoveAt(delInd);
        }

        //クリアでない場合
        if (!GAME_CLEAR)
        {
            //10個以上消した場合
            if (objIndex.Length >= 10) StartCoroutine(speHamScr.EraseTenBlocks());

            //ブロック全消し判定
            int nowblockCount = blockObj.Count;
            if (nowblockCount == 1 || (nowblockCount == 2 && SPECIAL_HARVEST)) ShootModeMan.Fever();
        }
    }

    //========================================================================
    //ブロック収穫(スペシャルハムスター)
    //========================================================================
    //obj; 収穫オブジェクト
    //========================================================================
    public void BlockHarvest(GameObject obj)
    {
        //収穫オブジェクトの番号取得
        int conObjIndex = blockObj.IndexOf(obj);
        if (conObjIndex >= 0)
        {
            //現在の投擲ブロック取得
            GameObject nowThrowBlockObj     = blockObj[throwBlockIndex];
            GameObject nowNextThrowBlockObj = blockObj[nextThrowBlockIndex];

            //エフェクト生成
            GameObject    effObj = Instantiate(effectPre);
            RectTransform effTra = effObj.GetComponent<RectTransform>();
            effTra.SetParent(blockBoxTra, false);
            int[] effPos = blockPosIndex[conObjIndex];
            effTra.anchoredPosition = blockPos[effPos[0]][effPos[1]][effPos[2]];

            //ブロック削除
            BlockDelete(new int[] { conObjIndex });

            //クリア判定
            if (GAME_CLEAR)
            {
                StartCoroutine(ShootModeMan.GameClear());
            }
            else
            {
                //投擲ブロック番号更新
                throwBlockIndex     = blockObj.IndexOf(nowThrowBlockObj);
                nextThrowBlockIndex = blockObj.IndexOf(nowNextThrowBlockObj);
            }
        }
    }

    //========================================================================
    //ブロックランダム生成落下(フィーバー開始)
    //========================================================================
    public IEnumerator FeverStrat(FeverHamuster ferverHumScr)
    {
        //フィーバー時間
        float feverTime   = UnityEngine.Random.Range(5.0f, 10.0f);
        float elapsedTime = 0.0f;
        int generateCount = 0;

        while (feverTime > elapsedTime)
        {
            //ブロックランダム生成
            float maxRange             = PLAY_SCREEN_WIDTH / 2.0f;
            Vector2 fallStartPos       = new Vector2(UnityEngine.Random.Range(-maxRange, maxRange), blockPosFixY);
            GameObject blockObject     = Instantiate(blockPre[UnityEngine.Random.Range(0, usingVegNum)]);
            RectTransform blockRectTra = blockObject.GetComponent<RectTransform>();
            Rigidbody2D blockRigi      = blockObject.GetComponent<Rigidbody2D>();
            CircleCollider2D blockColl = blockObject.GetComponent<CircleCollider2D>();
            blockRectTra.SetParent(blockBoxTra, false);
            blockRectTra.SetSiblingIndex(0);
            blockRectTra.anchoredPosition = fallStartPos;
            blockRigi.bodyType     = RigidbodyType2D.Dynamic;
            blockRigi.gravityScale = 0.5f;
            blockColl.enabled      = true;
            blockColl.isTrigger    = false;
            generateCount++;

            //ブロック生成スパン
            float generateTime = UnityEngine.Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(generateTime);
            elapsedTime += generateTime;
        }

        //ブロック削除待機
        yield return new WaitUntil(() => (generateCount <= fallCompleteCount) == true);
        yield return new WaitForSeconds(1.0f);
        fallCompleteCount = 0;

        //クリア判定
        if (GAME_CLEAR) StartCoroutine(ShootModeMan.GameClear());
        else
        {
            //ブロック3行生成
            StartCoroutine(LineBlockGenerate(3));
            //ハムスター元の位置へ
            StartCoroutine(ferverHumScr.ReturnFirstPosition());
        }
    }

    //========================================================================
    //現在のブロックライン数更新
    //========================================================================
    void NowLineNumUpdate()
    {
        int maxLineNumber = 0;
        foreach (int[] posInd in blockPosIndex)
        {
            int blockIndex = blockPosIndex.IndexOf(posInd);
            if (blockIndex != throwBlockIndex && blockIndex != nextThrowBlockIndex)
            {
                if (maxLineNumber < posInd[1])
                    maxLineNumber = posInd[1];
            }
        }
        nowLineNum = maxLineNumber;
        speHamScr.lowestLinePosY = blockPos[0][nowLineNum][0].y;

        //ゲームオーバー
        if (BLOCK_MAX_LINE_NUM <= nowLineNum + 1) StartCoroutine(ShootModeMan.GameOver());
    }
}