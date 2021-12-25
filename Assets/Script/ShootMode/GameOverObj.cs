using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MoveFunction.ObjectMove;

namespace ShootMode
{
    public class GameOverObj : MonoBehaviour
    {
        public IEnumerator DirectGameOver(ShootModeManager ShootModeMan)
        {
            SoundManager soundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();

            //�ړ��ݒ�
            RectTransform tra  = GetComponent<RectTransform>();
            float moveSpeed    = 10.0f;
            float acceleRate   = 1.0f;
            Vector2 startPos   = new Vector2(0.0f, 1700.0f);
            Vector2 targetPos  = Vector2.zero;
            float moveTime     = GetMoveTime(moveSpeed, acceleRate, startPos, targetPos);

            //�g�k�ݒ�
            float scalingSpeed = 0.03f;
            float[] size       = new float[] { 0.7f, 1.0f };
            int scalingTimes   = 1;
            float scalingTime  = GetScaleChangeTime(this.gameObject, tra, scalingSpeed, size[0], size[1], scalingTimes);

            //�����l�w��
            tra.anchoredPosition = startPos;
            tra.localScale = new Vector3(size[1], size[1], size[1]);

            //�ړ�
            soundMan.BGM_Volume(0.0f);
            soundMan.GameOverSE(0);
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(moveTime);
            soundMan.GameOverSE(1);

            //�g�k
            StartCoroutine(ScaleChange(tra, scalingSpeed, size[0], size[1], scalingTimes));
            yield return new WaitForSeconds(scalingTime);
            yield return new WaitForSeconds(1.5f);

            //���g���폜
            ShootModeMan.gameOverObjDis = false;
            Destroy(this.gameObject);
        }
    }
}