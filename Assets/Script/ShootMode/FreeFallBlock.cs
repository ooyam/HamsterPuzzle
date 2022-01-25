using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

public class FreeFallBlock : MonoBehaviour
{
    IEnumerator Start()
    {
        //左右揺れ設定
        RectTransform tra  = this.gameObject.GetComponent<RectTransform>();
        Rigidbody2D rig    = this.gameObject.GetComponent<Rigidbody2D>();
        float shakeSpeed   = 20.0f;    //移動速度
        float shakeOffsetX = 20.0f;    //移動座標X
        float shakeOffsetY = 0.0f;     //移動座標Y
        int shakeTimes     = 4;        //揺れ回数
        float delayTime    = 0.0f;     //移動間の遅延時間

        //左右揺れ開始
        yield return StartCoroutine(SlideShakeMovement(tra, shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime));

        //落下開始
        rig.bodyType = RigidbodyType2D.Dynamic;
        rig.gravityScale = 1.5f;
    }
}
