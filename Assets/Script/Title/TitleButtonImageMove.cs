using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

public class TitleButtonImageMove : MonoBehaviour
{
    RectTransform tra;
    bool  scaling      = false;

    void Awake()
    {
        //クローン作製された場合はインスタンス削除
        if (this.gameObject.name.Contains("(Clone)")) Destroy(this);
        tra = GetComponent<RectTransform>();
    }

    //アクティブ時実行
    void OnEnable()
    {
        tra.localScale = Vector3.one;
        scaling = true;
        StartCoroutine(ScalingStart());
    }

    //非アクティブ時実行
    void OnDisable()
    {
        scaling = false;
    }

    //拡縮開始
    IEnumerator ScalingStart()
    {
        Vector3 moveSpeed = Vector3.one * 0.002f;
        float maxScale    = 1.03f;
        float minScale    = 1.0f;
        int scalingTimes  = 1;
        float scalingTime = GetScaleChangeTime(tra, moveSpeed, maxScale, minScale, scalingTimes);
        while (scaling)
        {
            StartCoroutine(ScaleChange(tra, moveSpeed, maxScale, minScale, scalingTimes));
            yield return new WaitForSeconds(scalingTime);
        }
    }
}