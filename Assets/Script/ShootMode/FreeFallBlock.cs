using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

public class FreeFallBlock : MonoBehaviour
{
    IEnumerator Start()
    {
        //���E�h��ݒ�
        RectTransform tra  = this.gameObject.GetComponent<RectTransform>();
        Rigidbody2D rig    = this.gameObject.GetComponent<Rigidbody2D>();
        float shakeSpeed   = 20.0f;    //�ړ����x
        float shakeOffsetX = 20.0f;    //�ړ����WX
        float shakeOffsetY = 0.0f;     //�ړ����WY
        int shakeTimes     = 4;        //�h���
        float delayTime    = 0.0f;     //�ړ��Ԃ̒x������

        //���E�h��J�n
        yield return StartCoroutine(SlideShakeMovement(tra, shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime));

        //�����J�n
        rig.bodyType = RigidbodyType2D.Dynamic;
        rig.gravityScale = 1.5f;
    }
}
