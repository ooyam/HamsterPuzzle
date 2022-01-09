using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

public class TitleButton : MonoBehaviour
{
    RectTransform tra;
    Vector3 moveSpeed  = Vector3.one * 0.01f;
    float maxScale     = 1.1f;
    float minScale     = 0.9f;
    int   scalingTimes = 1;
    bool  scaling      = false;

    void Awake()
    {
        tra = GetComponent<RectTransform>();
    }

    //�A�N�e�B�u�����s
    void OnEnable()
    {
        tra.localScale = Vector3.one;
        scaling = true;
        StartCoroutine(ScalingStart());
    }

    //��A�N�e�B�u�����s
    void OnDisable()
    {
        scaling = false;
    }

    //�g�k�J�n
    IEnumerator ScalingStart()
    {
        float scalingTime = GetScaleChangeTime(tra, moveSpeed, maxScale, minScale, scalingTimes);
        while (scaling)
        {
            StartCoroutine(ScaleChange(tra, moveSpeed, maxScale, minScale, scalingTimes));
            yield return new WaitForSeconds(scalingTime);
        }
    }
}