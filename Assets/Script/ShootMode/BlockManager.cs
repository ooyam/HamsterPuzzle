using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using ShootMode;
using static MoveFunction.ObjectMove;

public class BlockManager : MonoBehaviour
{
    [Header("ブロックの取得")]
    [SerializeField]
    GameObject[] blockPre;
    List<GameObject> blockObj    = new List<GameObject>();     //生成ブロックobject
    List<RectTransform> blockTra = new List<RectTransform>();  //生成ブロックRectTransform
    List<int[]> blockPosIndex    = new List<int[]>();          //生成ブロック座標番号
    List<int> nowDeleteIndex     = new List<int>();            //削除中ブロックのオブジェクト番号リスト
    int throwBlockIndex;                                       //投擲ブロックのリスト番号
    CircleCollider2D throwBlockCollider;                       //投擲ブロックのCollider

    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform blockBoxTra;

    [Header("ハムスターボックス")]
    [SerializeField]
    Transform hamsterBox;
    HamsterController hamsterScr;  //HamsterController

    [Header("カメラの取得")]
    [SerializeField]
    Camera cameraMain;        //カメラの取得
    RectTransform canvasTra;  //CanvasのRectTransform
    float differenceX;        //座標修正数X
    float differenceY;        //座標修正数Y

    int[] columnNum = new int[] { 9, 8 };      //1行の列数
    Vector2[][][] blockPos;                    //ブロック配置座標 0:パターン番号 1:行番号 2:列番号
    [System.NonSerialized]
    public int nowLineNum   = 0;               //現在の行数
    int maxLineNum          = 12;              //最大行数
    [System.NonSerialized]
    public float blockPosY  = 103.8f;          //ブロック生成位置Y
    float[][] blockPosX     = new float[2][];  //ブロック生成位置X
    Vector2[] throwBlockPos = new Vector2[2];  //投擲ブロック生成座標
    bool generateEnd        = false;           //生成終了？
    [System.NonSerialized]
    public bool throwNow    = false;           //投擲中？
    [System.NonSerialized]
    public bool blockDeleteNow = false;        //ブロック削除中？
    [System.NonSerialized]
    public float blockDiameter = 120.0f;       //ブロック直径

    //int stageNum = 0;   //ステージ番号
    int vegTypeNum = Enum.GetValues(typeof(VegetableType)).Length; //使用する野菜の数

    void Start()
    {
        hamsterScr  = hamsterBox.GetChild(0).gameObject.GetComponent<HamsterController>();
        canvasTra   = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        differenceX = canvasTra.sizeDelta.x / 2;
        differenceY = canvasTra.sizeDelta.y;
        throwBlockPos[0] = new Vector2(70.0f, -10.0f);
        throwBlockPos[1] = new Vector2(-throwBlockPos[0].x, throwBlockPos[0].y);

        //ブロック配置座標指定
        float[] posXFix = new float[] { 480.0f, 420.0f };
        int patternNum = columnNum.Length;
        blockPos = new Vector2[patternNum][][];
        for (int ind_1 = 0; ind_1 < patternNum; ind_1++)
        {
            blockPos[ind_1] = new Vector2[maxLineNum][];
            for (int ind_2 = 0; ind_2 < maxLineNum; ind_2++)
            {
                blockPos[ind_1][ind_2] = new Vector2[columnNum[ind_1]];
                for (int ind_3 = 0; ind_3 < columnNum[ind_1]; ind_3++)
                {
                    float posX = blockDiameter * ind_3 - posXFix[ind_1];       //X座標計算
                    float posY = -blockPosY * ind_2 - blockDiameter / 2.0f;    //Y座標計算
                    blockPos[ind_1][ind_2][ind_3] = new Vector2(posX, posY);
                }
            }
        }

        //投擲用ブロック生成
        ThrowBlockGenerate();
        int firstGenerateLinesNum = 3;

        //ブロックを3列生成
        StartCoroutine(LineBlockGenerate(firstGenerateLinesNum));
    }

    //========================================================================
    //ブロック生成
    //========================================================================
    //blockPreIndex; ブロックのプレハブ番号
    //throwBloc;     投擲ブロック？
    //========================================================================
    int BlockGenerate(int blockPreIndex, bool throwBlock)
    {
        GameObject blockObject = Instantiate(blockPre[blockPreIndex]);
        RectTransform blockRectTra = blockObject.GetComponent<RectTransform>();
        blockRectTra.SetParent((throwBlock) ? hamsterBox : blockBoxTra, false);
        blockRectTra.SetSiblingIndex(0);
        blockObj.Add(blockObject);
        blockTra.Add(blockRectTra);
        blockPosIndex.Add(new int[0]);
        return blockTra.Count - 1;
    }

    //========================================================================
    //投擲ブロック生成指示
    //========================================================================
    public void ThrowBlockGenerate()
    {
        int blockGeneIndex = UnityEngine.Random.Range(0, vegTypeNum);
        int index = BlockGenerate(blockGeneIndex, true);
        blockTra[index].anchoredPosition = (hamsterScr.spriteDefault) ? throwBlockPos[0] : throwBlockPos[1];
        throwBlockIndex = index;
        blockObj[index].AddComponent<BlockController>();
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
        int patternNum = 4;
        int[] geneInd = new int[patternNum];
        //---------------------------------------------
        //指定行数ループ
        //---------------------------------------------
        for (int lineIndex = 0; lineIndex < generatLineNum; lineIndex++)
        {
            //---------------------------------------------
            //同じブロック2つを1組として､4組生成
            //---------------------------------------------
            for (int genePattIndex = 0; genePattIndex < patternNum; genePattIndex++)
            {
                int blockGeneIndex = UnityEngine.Random.Range(0, vegTypeNum);
                geneInd[genePattIndex] = blockGeneIndex;
            }

            int blockPosFirstIndex = nowLineNum % 2;  //ブロック座標FirstIndex番号
            int blockPosThirdIndex = 0;               //ブロック座標ThirdIndex番号
            int outputThreeInd = (blockPosFirstIndex == 0) ? UnityEngine.Random.Range(0, patternNum) : -1;  //ループ何回目に3つ1組にするか

            //---------------------------------------------
            //パターン数分ループ
            //---------------------------------------------
            for (int patternIndex = 0; patternIndex < patternNum; patternIndex++)
            {
                int generatezNum = (outputThreeInd == patternIndex) ? 3 : 2;
                for (int i = 0; i < generatezNum; i++)
                {
                    int objIndex = BlockGenerate(geneInd[patternIndex], false);                             //ブロック生成
                    blockTra[objIndex].anchoredPosition = blockPos[blockPosFirstIndex][0][blockPosThirdIndex]; //ブロック座標指定
                    blockPosIndex[objIndex] = new int[] { blockPosFirstIndex, 0, blockPosThirdIndex };        //ブロックの座標の保存
                    blockPosThirdIndex++;
                }
            }
            //---------------------------------------------
            //一列下げる
            //---------------------------------------------
            nowLineNum++;
            generateEnd = false;
            StartCoroutine(LineDown());
            yield return new WaitUntil(() => generateEnd == true);
            yield return new WaitForSeconds(0.5f);  //0.5秒遅延
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

        //一行下げる
        while (true)
        {
            if (!bound)
            {
                //---------------------------------------------
                //跳ね返り後上昇
                //---------------------------------------------
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if(throwBlockIndex != objIndex && nowDeleteIndex.IndexOf(objIndex) < 0)
                    {
                        Vector2 targetPos = blockPos[blockPosIndex[objIndex][0]][blockPosIndex[objIndex][1] + 1][blockPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y - boundHigh;
                        blockTra[objIndex].anchoredPosition = Vector2.Lerp(blockTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blockTra[objIndex].anchoredPosition.y < targetPosY + 5.0f) bound = true;
                    }
                }
            }
            else
            {
                //---------------------------------------------
                //下降
                //---------------------------------------------
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if (throwBlockIndex != objIndex && nowDeleteIndex.IndexOf(objIndex) < 0)
                    {
                        Vector2 targetPos = blockPos[blockPosIndex[objIndex][0]][blockPosIndex[objIndex][1] + 1][blockPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y + boundHigh;
                        blockTra[objIndex].anchoredPosition = Vector2.Lerp(blockTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blockTra[objIndex].anchoredPosition.y > targetPos.y) loopEnd = true;
                    }
                }
                if (loopEnd) break;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }

        //---------------------------------------------
        //座標番号リスト更新
        //---------------------------------------------
        for (int posIndex = 0; posIndex < blockPosIndex.Count; posIndex++)
        { if (throwBlockIndex != posIndex) blockPosIndex[posIndex][1]++; }
        generateEnd = true;
    }

    //========================================================================
    //ブロックを投げる
    //========================================================================
    //linePoints; 投擲起動頂点座標
    //========================================================================
    public IEnumerator BlockThrow(Vector3[] linePoints)
    {
        throwNow = true;
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
    //ブロック接触
    //========================================================================
    //obj;    接触したブロック
    //conPos; 接触箇所
    //========================================================================
    public void BlockConnect(GameObject obj)
    {
        //投擲終了
        throwNow = false;

        //ブロックボックスの子オブジェクトに変更
        blockTra[throwBlockIndex].SetParent(blockBoxTra, true);

        int connectObjIndex = blockObj.IndexOf(obj);                           //接触したブロックの番号取得
        Vector3 connectObjPos = blockTra[connectObjIndex].anchoredPosition;    //接触したブロックの座標
        Vector3 throwBlockPos = blockTra[throwBlockIndex].anchoredPosition;    //投擲ブロックの座標

        float sameLineJudge = 40.0f;       //同列判定配置座標
        int[] arrangementPos = new int[3]; //投擲ブロック配置座標 0:パターン番号 1:行番号 2:列番号

        //下の行にセットする
        //(接触ブロックが8列パターン && ((接触ブロックが最左 && 左に接触) || (接触ブロックが最右 && 右に接触))) || 接触Y座標が一定以下
        if ((blockPosIndex[connectObjIndex][0] == 1 &&
            ((blockPosIndex[connectObjIndex][2] == 0 && connectObjPos.x >= throwBlockPos.x) ||
            (blockPosIndex[connectObjIndex][2] == columnNum[1] - 1 && connectObjPos.x <= throwBlockPos.x))) ||
            throwBlockPos.y <= connectObjPos.y - sameLineJudge)
        {
            arrangementPos[0] = (blockPosIndex[connectObjIndex][0] == 0) ? 1 : 0;
            arrangementPos[1] = blockPosIndex[connectObjIndex][1] + 1;

            //接触ブロックが9列パターン && (ブロックの左側に接触 || 接触ブロックが最左)
            if (blockPosIndex[connectObjIndex][0] == 0 && ((blockPosIndex[connectObjIndex][2] != 0 && connectObjPos.x >= throwBlockPos.x) || blockPosIndex[connectObjIndex][2] == columnNum[0] - 1))
                arrangementPos[2] = blockPosIndex[connectObjIndex][2] - 1;
            else if (blockPosIndex[connectObjIndex][0] == 1 && connectObjPos.x <= throwBlockPos.x)
                arrangementPos[2] = blockPosIndex[connectObjIndex][2] + 1;
            else arrangementPos[2] = blockPosIndex[connectObjIndex][2];
        }
        //同じ行にセットする
        else
        {
            arrangementPos[0] = (blockPosIndex[connectObjIndex][0] == 0) ? 0 : 1;
            arrangementPos[1] = blockPosIndex[connectObjIndex][1];
            arrangementPos[2] = (connectObjPos.x <= throwBlockPos.x) ? blockPosIndex[connectObjIndex][2] + 1 : blockPosIndex[connectObjIndex][2] - 1;
        }

        //投擲ブロック停止座標指定
        blockTra[throwBlockIndex].anchoredPosition = blockPos[arrangementPos[0]][arrangementPos[1]][arrangementPos[2]];
        blockPosIndex[throwBlockIndex] = arrangementPos;

        //ブロック削除
        AdjacentSameTagBlockJudgment(throwBlockIndex);


        //if (posLineInd > nowLineNum) nowLineNum = posLineInd;
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
        else ThrowBlockGenerate();
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
            bool deleteblock = nowDeleteIndex.IndexOf(objIndex) >= 0;

            //削除予定の無いブロックリスト取得
            if (throwBlockIndex != objIndex && !deleteblock)
                activeBlockList.Add(objIndex);

            //1行目のブロック番号取得
            if (blockPosIndex[objIndex][1] == 1 && !deleteblock)
                topLineConnectList.Add(objIndex);
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
        bool maxLine   = refPosInd[1] == maxLineNum;                 //最下段?
        bool maxColumn = refPosInd[2] == columnNum[columnType] - 1;  //最右列?
        bool minColumn = refPosInd[2] == 0;                          //最左列?

        //基準ブロックが9列パターンの場合
        if (columnType == 0)
        {
            //右端以外
            if (!maxColumn)
            {
                adjacentPosList.Add(new int[] { columnType, refPosInd[1], refPosInd[2] + 1 });                   //右
                if (!minLine) adjacentPosList.Add(new int[] { columnType + 1, refPosInd[1] - 1, refPosInd[2] }); //右上
                if (!maxLine) adjacentPosList.Add(new int[] { columnType + 1, refPosInd[1] + 1, refPosInd[2] }); //右下
            }

            //左端以外
            if (!minColumn)
            {
                adjacentPosList.Add(new int[] { columnType, refPosInd[1], refPosInd[2] - 1 });                       //左
                if (!minLine) adjacentPosList.Add(new int[] { columnType + 1, refPosInd[1] - 1, refPosInd[2] - 1 }); //左上
                if (!maxLine) adjacentPosList.Add(new int[] { columnType + 1, refPosInd[1] + 1, refPosInd[2] - 1 }); //左下
            }
        }
        //基準ブロックが8列パターンの場合
        else
        {
            if (!maxColumn) adjacentPosList.Add(new int[] { columnType,     refPosInd[1],     refPosInd[2] + 1 });  //右
            if (!minLine)   adjacentPosList.Add(new int[] { columnType - 1, refPosInd[1] - 1, refPosInd[2] + 1 });  //右上
            if (!maxLine)   adjacentPosList.Add(new int[] { columnType - 1, refPosInd[1] + 1, refPosInd[2] + 1 });  //右下
            if (!minColumn) adjacentPosList.Add(new int[] { columnType,     refPosInd[1],     refPosInd[2] - 1 });  //左
            if (!minLine)   adjacentPosList.Add(new int[] { columnType - 1, refPosInd[1] - 1, refPosInd[2] });      //左上
            if (!maxLine)   adjacentPosList.Add(new int[] { columnType - 1, refPosInd[1] + 1, refPosInd[2] });      //左下
        }

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
        //ブロック削除中フラグ
        blockDeleteNow = true;

        //拡縮設定
        bool blockDelete      = true;   //ブロックリスト削除実施フラグ
        float oneFrameTime    = 0.02f;  //1フレームの時間
        float scalingSpeed    = 0.05f;  //拡縮速度
        float changeScale     = 1.5f;   //変更後の拡大率
        float defaultScale    = 1.0f;   //初期拡大率
        int   scalingTimes    = 1;      //拡縮回数
        float scalingWaitTime = Mathf.Abs((changeScale - defaultScale) * 2 / scalingSpeed * oneFrameTime);  //拡縮待機時間

        //左右揺れ設定
        float shakeSpeed    = 20.0f;    //移動速度
        float shakeOffsetX  = 20.0f;    //移動座標X
        float shakeOffsetY  = 0.0f;     //移動座標Y
        int shakeTimes      = 4;        //揺れ回数
        float delayTime     = 0.0f;     //移動間の遅延時間
        float shakeWaitTime = GetSlideShakeTime(shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime);  //揺れ待機時間

        //落下設定
        float fallSpeed     = 5.0f;     //移動速度
        float acceleRate    = 1.1f;     //移動速度の加速率
        float fallTarget    = -1500.0f; //落下座標
        float moveWaitTime  = 0.0f;     //落下待機時間

        //時間差設定
        int nowBlockCount   = blockObj.Count;          //現在のブロックの総数
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
            fallWait[index]     = DirectWait[index] + ((connect) ? scalingWaitTime : shakeWaitTime);
            directingEnd[index] = false;
            fallStart[index]    = false;

            //子オブジェクトインデックス番号変更
            blockTra[deleteObjIndex[index]].SetSiblingIndex(nowBlockCount);
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
                        fallStart[index] = true;
                        moveWaitTime = BlockFallStart(deleteObjIndex[index], fallSpeed, acceleRate, fallTarget);
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
        yield return new WaitForSeconds(moveWaitTime / 4);
        if (connect) blockDelete = !FreeFallBlockJudgment();

        //接触削除時､自由落下ブロックが追加で発生した場合は実施しない
        if (blockDelete)
        {
            //ブロック削除
            yield return new WaitForSeconds(moveWaitTime * 3 / 4); //落下待機
            yield return new WaitForSeconds(0.5f);                 //落下後0.5秒待機
            BlockDelete(nowDeleteIndex.ToArray());

            //投擲ブロック生成
            ThrowBlockGenerate();

            //削除中リストリセット
            nowDeleteIndex.Clear();

            //ブロック削除中フラグリセット
            blockDeleteNow = false;
        }
    }

    //========================================================================
    //ブロック落下
    //========================================================================
    //objIndex;     落下ブロック番号
    //fallSpeed;    落下速度
    //acceleRate;   速度の加速率
    //fallTarget;   落下座標
    //return;       移動所要時間
    //========================================================================
    float BlockFallStart(int objIndex, float fallSpeed, float acceleRate, float fallTarget)
    {
        Vector2 nowPos    = blockTra[objIndex].anchoredPosition;
        Vector2 targetPos = new Vector2(nowPos.x, fallTarget);
        StartCoroutine(MoveMovement(blockTra[objIndex], fallSpeed, acceleRate, targetPos));
        //blockObj[objIndex].SetActive(false);
        return GetMoveTime(fallSpeed, acceleRate, nowPos, targetPos);
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
            Destroy(blockObj[delInd]);
            blockObj.RemoveAt(delInd);
        }
    }
}