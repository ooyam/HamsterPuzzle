using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockManager : MonoBehaviour
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
    GameObject[] blockPre;
    List<GameObject> blockObj = new List<GameObject>();
    List<RectTransform> blockTra = new List<RectTransform>();
    
    [Header("ブロックボックス")]
    [SerializeField]
    RectTransform[] blockBoxTra;
    int activeBoxIndex = 0;      //active中のインデックス番号
    Vector2 blockBoxStartPosY;   //開始位置

    [Header("ハムスターの取得")]
    [SerializeField]
    GameObject hamsterObj;
    RectTransform hamsterTra;

    int[] columnNum = new int[] { 9, 8 }; //1行の列数
    int nowLineNum = 0;                   //現在の行数
    int maxLineNum = 15;                  //最大行数
    int blockPosYMultiplier = 0;          //ブロック生成位置Y修正数
    float blockPosY = 103.8f;             //ブロック生成位置Y
    float[][] blockPosX = new float[2][]; //ブロック生成位置X
    int throwBlockIndex;                  //投擲ブロックのリスト番号

    int stageNum = 0;   //ステージ番号
    int vegTypeNum = 6; //使用する野菜の数

    void Start()
    {
        hamsterTra = hamsterObj.GetComponent<RectTransform>();
        blockBoxStartPosY = blockBoxTra[0].anchoredPosition;

        float blockDiameter = 120.0f;
        float[] posXFix = new float[] { 480.0f, 420.0f };
        for (int posXFirstInd = 0; posXFirstInd < blockPosX.Length; posXFirstInd++)
        {
            blockPosX[posXFirstInd] = new float[columnNum[posXFirstInd]];
            for (int posXSecondInd = 0; posXSecondInd < columnNum[posXFirstInd]; posXSecondInd++)
            {
                blockPosX[posXFirstInd][posXSecondInd] = blockDiameter * posXSecondInd - posXFix[posXFirstInd];
            }
        }
        float radius = blockDiameter / 2;
        ThrowBlocGenerate();
        int firstGenerateLinesNum = 3;
        LineBlocGenerate(firstGenerateLinesNum);
        StartCoroutine(LineBlocGenerateInterval());
    }

    //ブロック生成
    int BlocGenerate(int blockPreIndex, bool throwBlock)
    {
        GameObject blockObject = Instantiate(blockPre[blockPreIndex]);
        RectTransform blockRectTra = blockObject.GetComponent<RectTransform>();
        blockRectTra.SetParent((throwBlock) ? hamsterTra : blockBoxTra[activeBoxIndex], false);
        blockObj.Add(blockObject);
        blockTra.Add(blockRectTra);
        return blockTra.Count - 1;
    }

    //投擲ブロック生成指示
    public void ThrowBlocGenerate()
    {
        int blockGeneIndex = Random.Range(0, vegTypeNum);
        int index = BlocGenerate(blockGeneIndex, true);
        Vector2 hamsterPos = hamsterTra.anchoredPosition;
        blockTra[index].anchoredPosition = new Vector2(20.0f, -40.0f);
        throwBlockIndex = index;
    }

    //ブロック指定行数生成指示
    void LineBlocGenerate(int generatLineNum)
    {
        int patternNum = 4;
        int[] geneInd = new int[patternNum];
        for (int lineIndex = 0; lineIndex < generatLineNum; lineIndex++)
        {
            //同じブロック2つを1組として､4組生成
            for (int genePattIndex = 0; genePattIndex < patternNum; genePattIndex++)
            {
                int blockGeneIndex = Random.Range(0, vegTypeNum);
                geneInd[genePattIndex] = blockGeneIndex;
            }

            int blockPosIndex = nowLineNum % 2;
            int outputPatternInd = 0;
            int outputThreeInd = (blockPosIndex == 0) ? Random.Range(0, patternNum) : -1;
            int colIndex = 0;
            for (int pattIndex = 0; pattIndex < patternNum; pattIndex++)
            {
                int outputNum = (outputThreeInd == pattIndex) ? 3 : 2;
                for (int loopTimes = 0; loopTimes < outputNum; loopTimes++)
                {
                    int index = BlocGenerate(geneInd[outputPatternInd], false);
                    blockTra[index].anchoredPosition = new Vector2(blockPosX[blockPosIndex][colIndex], blockPosY * blockPosYMultiplier);
                    colIndex++;
                }
                outputPatternInd++;
            }
            blockPosYMultiplier++;
            nowLineNum++;
        }
        StartCoroutine(LineDown(generatLineNum));
    }

    //1行下がる
    IEnumerator LineDown(int downNum)
    {
        float oneFrameTime = 0.02f;
        float speed = 0.5f;
        float boundHigh = 40.0f;
        bool bound = false;
        Vector2 nowPos = blockBoxTra[activeBoxIndex].anchoredPosition;
        Vector2 targetPos = new Vector2(nowPos.x, nowPos.y - (blockPosY * downNum + boundHigh));
        while (true)
        {
            nowPos = blockBoxTra[activeBoxIndex].anchoredPosition;
            if (!bound)
            {
                blockBoxTra[activeBoxIndex].anchoredPosition = Vector2.Lerp(nowPos, targetPos, speed);
                if (nowPos.y <= targetPos.y + 5.0f) bound = true;
            }
            else
            {
                blockBoxTra[activeBoxIndex].anchoredPosition = Vector2.Lerp(nowPos, new Vector2(targetPos.x, targetPos.y + boundHigh), speed);
                if (nowPos.y >= targetPos.y + boundHigh - 1.0f)
                {
                    blockBoxTra[activeBoxIndex].anchoredPosition = new Vector2(targetPos.x, targetPos.y + boundHigh);
                    break;
                }
            }
            yield return new WaitForSeconds(oneFrameTime);
        }

        //座標の限界値が10万の為､Y軸-1万程でリセット
        if (blockPosYMultiplier > 100)
        {
            activeBoxIndex = (activeBoxIndex == 0) ? 1 : 0;
            blockBoxTra[activeBoxIndex].anchoredPosition = blockBoxStartPosY;
            foreach (RectTransform bloTra in blockTra)
            { bloTra.SetParent(blockBoxTra[activeBoxIndex], true); }
            blockPosYMultiplier = 0;
        }
    }

    //時間経過で1行生成指示
    IEnumerator LineBlocGenerateInterval()
    {
        float generateTime = 3.0f;
        switch (stageNum)
        {
            case 10:
                generateTime = 20.0f;
                break;
            default:
                break;
        }
        while (true)
        {
            yield return new WaitForSeconds(generateTime);
            LineBlocGenerate(1);
        }
    }
}
