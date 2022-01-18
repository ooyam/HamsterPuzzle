using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class GameOverObj_Tutorial : MonoBehaviour
    {
        public IEnumerator DirectGameOver(ShootModeManager_Tutorial ShootModeMan, SoundManager soundMan)
        {
            //�ړ��ݒ�
            RectTransform tra  = GetComponent<RectTransform>();
            float moveSpeed    = 20.0f;
            float acceleRate   = 1.0f;
            Vector2 startPos   = new Vector2(0.0f, 1700.0f);
            Vector2 targetPos  = Vector2.zero;
            float moveTime     = GetMoveTime(tra, moveSpeed, acceleRate, targetPos);

            //�g�k�ݒ�
            Vector3 scalingSpeed = new Vector3(0.0f, -0.03f, 0.0f);
            float[] size         = new float[] { 0.7f, 1.0f };
            int scalingTimes     = 1;
            float scalingTime    = GetScaleChangeTime(tra, scalingSpeed, size[0], size[1], scalingTimes);

            //�����l�w��
            tra.anchoredPosition = startPos;
            tra.localScale = new Vector3(size[1], size[1], size[1]);

            //�ړ�
            soundMan.BGM_Volume(0.0f);
            soundMan.GameOverSE(0);
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(moveTime * 0.95f);
            soundMan.GameOverSE(1);

            //�g�k
            StartCoroutine(ScaleChange(tra, scalingSpeed, size[0], size[1], scalingTimes));
            yield return new WaitForSeconds(scalingTime);
            yield return new WaitForSeconds(1.0f);

            //�I�����}
            ShootModeMan.gameOverObjDis = false;

            //���g���폜
            Destroy(this.gameObject);
        }
    }
}