using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode;
using static MoveFunction.ObjectMove;

public class BlocManager : MonoBehaviour
{
    [Header("ブロックの取得")]
    [SerializeField]
    GameObject[] blocPre;
    List<GameObject> blocObj = new List<GameObject>();        //生成ブロックobject
    List<RectTransform> blocTra = new List<RectTransform>();  //生成ブロックRectTransform
    List<int[]> blocPosIndex = new List<int[]>();             //生成ブロック座標番号
    int throwBlocIndex;                                       //投擲ブロックのリスト番号

    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform blocBoxTra;

    [Header("ハムスターボックス")]
    [SerializeField]
    Transform hamsterBox;

    [Header("カメラの取得")]
    [SerializeField]
    Camera cameraMain;            //カメラの取得
    RectTransform canvasTra;      //CanvasのRectTransform
    float differenceX;            //座標修正数X
    float differenceY;            //座標修正数Y

    int[] columnNum = new int[] { 9, 8 };  //1行の列数
    Vector2[][][] blocPos;                 //ブロック配置座標 0:パターン番号 1:行番号 2:列番号
    [System.NonSerialized]
    public int nowLineNum = 0;             //現在の行数
    int maxLineNum = 12;                   //最大行数
    [System.NonSerialized]
    public float blocPosY = 103.8f;        //ブロック生成位置Y
    float[][] blocPosX = new float[2][];   //ブロック生成位置X
    float blocDiameter = 120.0f;           //ブロック直径
    bool generateEnd = false;              //生成終了？
    [System.NonSerialized]
    public bool throwNow = false;          //投擲中？

    //int stageNum = 0;   //ステージ番号
    int vegTypeNum = 6; //使用する野菜の数

    void Start()
    {
        canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        differenceX = canvasTra.sizeDelta.x / 2;
        differenceY = canvasTra.sizeDelta.y;

        //ブロック配置座標指定
        float[] posXFix = new float[] { 480.0f, 420.0f };
        int patternNum = columnNum.Length;
        blocPos = new Vector2[patternNum][][];
        for (int ind_1 = 0; ind_1 < patternNum; ind_1++)
        {
            blocPos[ind_1] = new Vector2[maxLineNum][];
            for (int ind_2 = 0; ind_2 < maxLineNum; ind_2++)
            {
                blocPos[ind_1][ind_2] = new Vector2[columnNum[ind_1]];
                for (int ind_3 = 0; ind_3 < columnNum[ind_1]; ind_3++)
                {
                    float posX = blocDiameter * ind_3 - posXFix[ind_1];      //X座標計算
                    float posY = -blocPosY * ind_2 - blocDiameter / 2.0f;    //Y座標計算
                    blocPos[ind_1][ind_2][ind_3] = new Vector2(posX, posY);
                }
            }
        }

        ThrowBlocGenerate();  //投擲用ブロック生成
        int firstGenerateLinesNum = 3;
        StartCoroutine(LineBlocGenerate(firstGenerateLinesNum)); //ブロックを3列生成
    }

    //========================================================================
    //ブロック生成
    //========================================================================
    //blocPreIndex; ブロックのプレハブ番号
    //throwBloc;    投擲ブロック？
    //========================================================================
    int BlocGenerate(int blocPreIndex, bool throwBloc)
    {
        GameObject blocObject = Instantiate(blocPre[blocPreIndex]);
        RectTransform blocRectTra = blocObject.GetComponent<RectTransform>();
        blocRectTra.SetParent((throwBloc) ? hamsterBox : blocBoxTra, false);
        blocRectTra.SetSiblingIndex(0);
        blocObj.Add(blocObject);
        blocTra.Add(blocRectTra);
        blocPosIndex.Add(new int[0]);
        return blocTra.Count - 1;
    }

    //========================================================================
    //投擲ブロック生成指示
    //========================================================================
    public void ThrowBlocGenerate()
    {
        int blocGeneIndex = Random.Range(0, vegTypeNum);
        int index = BlocGenerate(blocGeneIndex, true);
        blocTra[index].anchoredPosition = new Vector2(70.0f, -10.0f);
        throwBlocIndex = index;
        blocObj[index].AddComponent<BlocController>();
    }

    //========================================================================
    //ブロック指定行数生成指示
    //========================================================================
    //generatLineNum; ライン生成数
    //========================================================================
    public IEnumerator LineBlocGenerate(int generatLineNum)
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
                int blocGeneIndex = Random.Range(0, vegTypeNum);
                geneInd[genePattIndex] = blocGeneIndex;
            }

            int blocPosFirstIndex = nowLineNum % 2;  //ブロック座標FirstIndex番号
            int blocPosThirdIndex = 0;               //ブロック座標ThirdIndex番号
            int outputThreeInd = (blocPosFirstIndex == 0) ? Random.Range(0, patternNum) : -1;  //ループ何回目に3つ1組にするか

            //---------------------------------------------
            //パターン数分ループ
            //---------------------------------------------
            for (int patternIndex = 0; patternIndex < patternNum; patternIndex++)
            {
                int generatezNum = (outputThreeInd == patternIndex) ? 3 : 2;
                for (int i = 0; i < generatezNum; i++)
                {
                    int objIndex = BlocGenerate(geneInd[patternIndex], false);                             //ブロック生成
                    blocTra[objIndex].anchoredPosition = blocPos[blocPosFirstIndex][0][blocPosThirdIndex]; //ブロック座標指定
                    blocPosIndex[objIndex] = new int[] { blocPosFirstIndex, 0, blocPosThirdIndex };        //ブロックの座標の保存
                    blocPosThirdIndex++;
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
        float oneFrameTime = 0.02f;
        float speed = 0.4f;
        float boundHigh = 30.0f;
        bool bound = false;
        bool loopEnd = false;
        int objCount = blocObj.Count;
        while (true)
        {
            if (!bound)
            {
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if(throwBlocIndex != objIndex)
                    {
                        Vector2 targetPos = blocPos[blocPosIndex[objIndex][0]][blocPosIndex[objIndex][1] + 1][blocPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y - boundHigh;
                        blocTra[objIndex].anchoredPosition = Vector2.Lerp(blocTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blocTra[objIndex].anchoredPosition.y < targetPosY + 5.0f) bound = true;
                    }
                }
            }
            else
            {
                for (int objIndex = 0; objIndex < objCount; objIndex++)
                {
                    if (throwBlocIndex != objIndex)
                    {
                        Vector2 targetPos = blocPos[blocPosIndex[objIndex][0]][blocPosIndex[objIndex][1] + 1][blocPosIndex[objIndex][2]];
                        float targetPosY = targetPos.y + boundHigh;
                        blocTra[objIndex].anchoredPosition = Vector2.Lerp(blocTra[objIndex].anchoredPosition, new Vector2(targetPos.x, targetPosY), speed);
                        if (objIndex == objCount - 1 && blocTra[objIndex].anchoredPosition.y > targetPos.y) loopEnd = true;
                    }
                }
                if (loopEnd) break;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }

        //---------------------------------------------
        //ポジション更新
        //---------------------------------------------
        for (int posIndex = 0; posIndex < blocPosIndex.Count; posIndex++)
        { if (throwBlocIndex != posIndex) blocPosIndex[posIndex][1]++; }
        generateEnd = true;
    }

    //========================================================================
    //ブロックを投げる
    //========================================================================
    //linePoints; 投擲起動頂点座標
    //========================================================================
    public IEnumerator BlocThrow(Vector3[] linePoints)
    {
        throwNow = true;
        float oneFrameTime = 0.02f;
        float throwSpeed = 50.0f;
        float maxRangeFix = 60.0f;
        int targetIndex = 1;
        int pointsCount = linePoints.Length;
        while (throwNow)
        {
            Vector3 throwBlocPos = blocTra[throwBlocIndex].anchoredPosition;
            blocTra[throwBlocIndex].anchoredPosition = Vector3.MoveTowards(throwBlocPos, linePoints[targetIndex], throwSpeed);
            float[] endPos = new float[]{ linePoints[targetIndex].x + maxRangeFix, linePoints[targetIndex].x - maxRangeFix };
            if (throwBlocPos.x <= endPos[0] && throwBlocPos.x >= endPos[1])
            {
                if(pointsCount - 1 > targetIndex) targetIndex++;
            }
            yield return new WaitForSeconds(oneFrameTime);
        }

        //投擲ブロック生成
        ThrowBlocGenerate();
    }

    //========================================================================
    //ブロック接触
    //========================================================================
    //obj;    接触したブロック
    //conPos; 接触箇所
    //========================================================================
    public void BlocConnect(GameObject obj)
    {
        throwNow = false;                                     //投擲終了
        blocTra[throwBlocIndex].SetParent(blocBoxTra, true);  //ブロックボックスの子オブジェクトに変更

        int connectObjIndex = blocObj.IndexOf(obj);                           //接触したブロックの番号取得
        Vector3 connectObjPos = blocTra[connectObjIndex].anchoredPosition;    //接触したブロックの座標
        Vector3 throwBlocPos = blocTra[throwBlocIndex].anchoredPosition;      //投擲ブロックの座標

        float sameLineJudge = 40.0f;       //同列判定配置座標
        int[] arrangementPos = new int[3]; //投擲ブロック配置座標 0:パターン番号 1:行番号 2:列番号

        //下の行にセットする
        //(接触ブロックが8列パターン && ((接触ブロックが最左 && 左に接触) || (接触ブロックが最右 && 右に接触))) || 接触Y座標が一定以下
        if ((blocPosIndex[connectObjIndex][0] == 1 &&
            ((blocPosIndex[connectObjIndex][2] == 0 && connectObjPos.x >= throwBlocPos.x) ||
            (blocPosIndex[connectObjIndex][2] == columnNum[1] - 1 && connectObjPos.x <= throwBlocPos.x))) ||
            throwBlocPos.y <= connectObjPos.y - sameLineJudge)
        {
            arrangementPos[0] = (blocPosIndex[connectObjIndex][0] == 0) ? 1 : 0;
            arrangementPos[1] = blocPosIndex[connectObjIndex][1] + 1;

            //接触ブロックが9列パターン && (ブロックの左側に接触 || 接触ブロックが最左)
            if (blocPosIndex[connectObjIndex][0] == 0 && ((blocPosIndex[connectObjIndex][2] != 0 && connectObjPos.x >= throwBlocPos.x) || blocPosIndex[connectObjIndex][2] == columnNum[0] - 1))
                arrangementPos[2] = blocPosIndex[connectObjIndex][2] - 1;
            else if (blocPosIndex[connectObjIndex][0] == 1 && connectObjPos.x <= throwBlocPos.x)
                arrangementPos[2] = blocPosIndex[connectObjIndex][2] + 1;
            else arrangementPos[2] = blocPosIndex[connectObjIndex][2];
        }
        //同じ行にセットする
        else
        {
            arrangementPos[0] = (blocPosIndex[connectObjIndex][0] == 0) ? 0 : 1;
            arrangementPos[1] = blocPosIndex[connectObjIndex][1];
            arrangementPos[2] = (connectObjPos.x <= throwBlocPos.x) ? blocPosIndex[connectObjIndex][2] + 1 : blocPosIndex[connectObjIndex][2] - 1;
        }

        //投擲ブロック停止座標指定
        blocTra[throwBlocIndex].anchoredPosition = blocPos[arrangementPos[0]][arrangementPos[1]][arrangementPos[2]];
        blocPosIndex[throwBlocIndex] = arrangementPos;

        //ブロック削除
        Debug.Log("throwBlocIndex : " + throwBlocIndex);
        AdjacentJudgment(new int[] { throwBlocIndex });


        //if (posLineInd > nowLineNum) nowLineNum = posLineInd;
    }

    //========================================================================
    //隣接同タグブロック判定
    //========================================================================
    //index; 基準のブロックの番号
    //========================================================================
    void AdjacentJudgment(int[] index)
    {

        //テスト
        int[] refPosInd = blocPosIndex[index[0]];      //基準ブロックの座標番号
        int columnType = refPosInd[0];                 //配置タイプ
        List<int[]> adjacentList = new List<int[]>();  //隣接ブロックリスト

        bool minLine   = refPosInd[1] == 0;                          //最上段?
        bool maxLine   = refPosInd[1] == maxLineNum;                 //最下段?
        bool maxColumn = refPosInd[2] == columnNum[columnType] - 1;  //最右列?
        bool minColumn = refPosInd[2] == 0;                          //最左列?

        //基準ブロックが9列パターンの場合
        if (columnType == 0)
        {
            //右端以外
            if (!maxColumn)
            {
                adjacentList.Add(new int[] { refPosInd[0], refPosInd[1], refPosInd[2] + 1 });               //右
                if (!minLine) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] - 1, refPosInd[2] }); //右上
                if (!maxLine) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] + 1, refPosInd[2] }); //右下
            }

            //左端以外
            if (!minColumn)
            {
                adjacentList.Add(new int[] { refPosInd[0], refPosInd[1], refPosInd[2] - 1 });                   //左
                if (!minLine) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] - 1, refPosInd[2] - 1 }); //左上
                if (!maxLine) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] + 1, refPosInd[2] - 1 }); //左下
            }
        }
        //基準ブロックが8列パターンの場合
        else
        {
            if (!maxColumn) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1]    , refPosInd[2] + 1 });  //右
            if (!minLine)   adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] - 1, refPosInd[2] + 1 });  //右上
            if (!maxLine)   adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] + 1, refPosInd[2] + 1 });  //右下
            if (!minColumn) adjacentList.Add(new int[] { refPosInd[0], refPosInd[1]    , refPosInd[2] - 1 });  //左
            if (!minLine)   adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] - 1, refPosInd[2] });      //左上
            if (!maxLine)   adjacentList.Add(new int[] { refPosInd[0], refPosInd[1] + 1, refPosInd[2] });      //左下
        }

        //同タグ判定
        List<int> deleteList = new List<int>();  //削除ブロック番号リスト
        string blockTag = blocObj[index[0]].tag;
        Debug.Log("throwBlocTag : " + blockTag);
        foreach (int[] posIndex in adjacentList)
        {
            Debug.Log("adjacentList[" + adjacentList.IndexOf(posIndex) + "] : " + posIndex[0] + ", " + posIndex[1] + ", " + posIndex[2]);

            //↓ここが取得できていない
            int blockIndex = blocPosIndex.IndexOf(posIndex);
            //if (blockIndex >= 0) Debug.Log("adjacentObjTag : " + blocObj[blockIndex].tag);
            if (blockIndex >= 0 && blocObj[blockIndex].tag == blockTag)
                deleteList.Add(blockIndex);
        }

        //配列に変換して削除実行
        if (deleteList.Count > 0)
        {
            foreach (var item in deleteList)
            {
                Debug.Log("index : " + item);
                Debug.Log("tag : " + blocObj[item].tag);
            }
            int[] deleteBlocks = deleteList.ToArray();
            BlocDelete(deleteBlocks, true);
        }







        /*
        //削除ブロック番号
        List<int> deleteBlocIndex = new List<int>();

        while (true)
        {
            //座標番号取得
            int indexLength = index.Length;
            int[][] posIndex = new int[indexLength][];
            for (int i = 0; i < indexLength; i++)
            { posIndex[i] = blocPosIndex[index[i]]; }

            //隣接の有無
            bool adjacent = false;

            foreach (int[] posInd in posIndex)
            {
                //削除ブロック座標番号
                List<int[]> adjacentPosIndex = new List<int[]>();

                //右端以外
                if (posInd[2] != columnNum[posInd[0]])
                {
                    adjacentPosIndex.Add(new int[] { posInd[0], posInd[1], posInd[2] + 1 }); //右
                    if (posInd[1] != 0) adjacentPosIndex.Add(new int[] { posInd[0], posInd[1] - 1, posInd[2] + columnNum[posInd[0]] });         //右上
                    if (posInd[1] < maxLineNum) adjacentPosIndex.Add(new int[] { posInd[0], posInd[1] + 1, posInd[2] + columnNum[posInd[0]] }); //右下
                }
                //左端以外
                if (posInd[2] != 0)
                {
                    adjacentPosIndex.Add(new int[] { posInd[0], posInd[1], posInd[2] - 1 }); //左
                    if (posInd[1] != 0) adjacentPosIndex.Add(new int[] { posInd[0], posInd[1] - 1, posInd[2] + columnNum[posInd[0]] - 1 });         //左上
                    if (posInd[1] < maxLineNum) adjacentPosIndex.Add(new int[] { posInd[0], posInd[1] - 1, posInd[2] + columnNum[posInd[0]] - 1 }); //左下
                }

                int blocObjCount = blocObj.Count;
                foreach (int[] adjPosInd in adjacentPosIndex)
                {
                    //オブジェクト番号取得
                    int deleteObjIndex = blocObjCount;
                    for (int posListIndex = 0; posListIndex < blocObjCount; posListIndex++)
                    {
                        if (blocPosIndex[posListIndex][0] == adjPosInd[0] &&
                            blocPosIndex[posListIndex][1] == adjPosInd[1] &&
                            blocPosIndex[posListIndex][2] == adjPosInd[2])
                            deleteObjIndex = posListIndex;
                    }

                    //オブジェクト取得
                    List<int> deleteBlocIndexDummy = new List<int>();
                    if (deleteObjIndex < blocObjCount)
                    {
                        if (blocObj[deleteObjIndex].tag == blocObj[index[0]].tag)
                        {
                            deleteBlocIndex.Add(deleteObjIndex);
                            deleteBlocIndexDummy.Add(deleteObjIndex);
                            adjacent = true;
                        }
                    }

                    //配列に変化して次の隣接ブロック判定
                    index = deleteBlocIndexDummy.ToArray();
                }
            }
            if (!adjacent) break;
        }

        //配列に変化してブロック削除
        if (deleteBlocIndex.Count > 0) BlocDelete(deleteBlocIndex.ToArray(), true);
        foreach (int delBloInd in deleteBlocIndex)
        {
            Debug.Log(blocObj[delBloInd].tag);
        }*/
    }

    //========================================================================
    //ブロック削除
    //========================================================================
    //deleteObjIndex; 削除ブロック番号
    //connect;        接触削除？
    //========================================================================
    void BlocDelete(int[] deleteObjIndex, bool connect)
    {
        if (connect)
        {
            float movespeed = 30.0f;

            foreach (int delInd in deleteObjIndex)
            {
                Vector2 nowPos = blocTra[delInd].anchoredPosition;
                Vector2 targetPos = new Vector2(nowPos.x, -1000.0f);
                StartCoroutine(MoveMovement(blocTra[delInd], movespeed, targetPos, false));
            }
        }
    }
}