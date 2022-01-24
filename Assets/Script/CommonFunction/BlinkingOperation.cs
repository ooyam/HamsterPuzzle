using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MoveFunction.ObjectMove;

public class BlinkingOperation : MonoBehaviour
{
    [Header("Iamge")]
    public Image ima;

    [Header("Text")]
    public Text tex;

    [Header("変更速度")]
    public float changeSpeed;

    [Header("変更色の配列")]
    public Color[] colArray;

    [Header("比較番号指定配列(0:R 1:G 2:B 3:A)")]
    public int[] compArray;

    [Header("ループ回数(配列1周で1カウント,-1でカウント停止無し)")]
    public int chengeCount;

    [Header("初期遅延時間")]
    public float waitTime;

    [Header("停止時間(-1で時間停止無し)")]
    public float endTime;

    void OnEnable()
    {
        if (tex == null) ima.color = colArray[0];
        else tex.color = colArray[0];
        StartCoroutine(BlinkingStart());
    }

    IEnumerator BlinkingStart()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        if (ima != null) StartCoroutine(ImagePaletteChange(ima, changeSpeed, colArray, compArray, chengeCount));
        if (tex != null) StartCoroutine(TextPaletteChange(tex, changeSpeed, colArray, compArray, chengeCount));
        if(endTime > 0)
        {
            yield return new WaitForSecondsRealtime(endTime);
            COLOR_CHANGE_INFINITE_END = true;
        }
    }
}
