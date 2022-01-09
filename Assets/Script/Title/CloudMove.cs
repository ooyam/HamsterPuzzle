using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

public class CloudMove : MonoBehaviour
{
    void Start()
    {
        RectTransform tra = GetComponent<RectTransform>();
        float moveSpeed   = 0.01f;
        float maxScale    = 1.1f;
        float minScale    = 0.9f;
        float scale       = Random.Range(minScale, maxScale);
        tra.localScale    = new Vector2(scale, scale);
        int scalingTimes  = -1;

        StartCoroutine(ScaleChange(tra, Vector3.one * moveSpeed, maxScale, minScale, scalingTimes));
    }
}