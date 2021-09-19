using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode;

public class BlocManager : MonoBehaviour
{
    public enum VegetableType
    {
        broccoli,
        cabbage,
        carrot,
        paprika,
        pumpkin,
        corn
    }
    VegetableType vegType;

    [Header("ブロックの取得")]
    [SerializeField]
    GameObject[] blocPre;
    List<GameObject> blocObj = new List<GameObject>();        //生成ブロックobject
    List<RectTransform> blocTra = new List<RectTransform>();  //生成ブロックRectTransform
    List<int[]> blocPosIndex = new List<int[]>();             //生成ブロックPositionIndex
    int throwBlocIndex;                                       //投擲ブロックのリスト番号

    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform blocBoxTra;

    [Header("ハムスターの取得")]
    [SerializeField]
    GameObject hamsterObj;        //ハムスターオブジェクト
    RectTransform hamsterTra;     //ハムスターRectTransform
    Vector2 hamsterPos;           //ハムスター位置

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
    bool blocConnect = false;              //ブロック接触

    int stageNum = 0;   //ステージ番号
    int vegTypeNum = 6; //使用する野菜の数

    void Start()
    {
        hamsterTra = hamsterObj.GetComponent<RectTransform>();
        hamsterPos = hamsterTra.anchoredPosition;

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
    //blocPreIndex;   ブロックのプレハブ番号
    //throwBloc;      投擲ブロック？
    //========================================================================
    int BlocGenerate(int blocPreIndex, bool throwBloc)
    {
        GameObject blocObject = Instantiate(blocPre[blocPreIndex]);
        RectTransform blocRectTra = blocObject.GetComponent<RectTransform>();
        blocRectTra.SetParent((throwBloc) ? hamsterTra : blocBoxTra, false);
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
        blocTra[index].anchoredPosition = new Vector2(20.0f, -40.0f);
        throwBlocIndex = index;
        blocObj[index].AddComponent<BlocController>();
    }

    //========================================================================
    //ブロック指定行数生成指示
    //========================================================================
    //generatLineNum;   ライン生成数
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
                    blocPosIndex[objIndex] = new int[] { blocPosFirstIndex, 0, blocPosThirdIndex };        //オブジェクトの座標の保存
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
    //linePoints;   投擲起動頂点座標
    //========================================================================
    public IEnumerator BlocThrow(Vector3[] linePoints)
    {
        throwNow = true;
        float oneFrameTime = 0.02f;
        float throwSpeed = 50.0f;
        int targetIndex = 1;
        int pointsCount = linePoints.Length;
        while (true)
        {
            Vector3 throwBlocPos = blocTra[throwBlocIndex].anchoredPosition;
            blocTra[throwBlocIndex].anchoredPosition = Vector3.MoveTowards(throwBlocPos, linePoints[targetIndex], throwSpeed);
            float[] endPos = new float[]{ linePoints[targetIndex].x + 1.0f, linePoints[targetIndex].x - 1.0f };
            if (throwBlocPos.x <= endPos[0] && throwBlocPos.x >= endPos[1])
            {
                if(pointsCount - 1 > targetIndex) targetIndex++;
            }
            yield return new WaitForSeconds(oneFrameTime);
            if (blocConnect) break;
        }

        //投擲ブロック生成
        ThrowBlocGenerate();
        throwNow = false;
        blocConnect = false;
    }

    //========================================================================
    //ブロック接触
    //========================================================================
    //obj;     接触したオブジェクト
    //conPos;  接触箇所
    //========================================================================
    public void BlocConnect(GameObject obj, Vector3 conPos)
    {
        int connectObjIndex = blocObj.IndexOf(obj);  //
        int posFirstIndex = (blocPosIndex[connectObjIndex][0] == 0) ? 1 : 0;
        blocTra[throwBlocIndex].anchoredPosition = blocPos[posFirstIndex][blocPosIndex[connectObjIndex][1] + 1][blocPosIndex[connectObjIndex][2]];

    }
}
