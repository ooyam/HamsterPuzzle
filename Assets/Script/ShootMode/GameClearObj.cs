using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundFunction;
using static MoveFunction.ObjectMove;

namespace ShootMode
{
    public class GameClearObj : MonoBehaviour
    {
        public IEnumerator DirectGameClear(ShootModeManager ShootModeMan, SoundManager soundMan)
        {
            soundMan.BGM_Volume(0.0f);
            soundMan.GameClearSE();

            //�g�k�ݒ�
            RectTransform tra    = GetComponent<RectTransform>();
            Vector3 scalingSpeed = new Vector3(0.02f, 0.02f, 0.0f);
            float endSize        = 1.0f;
            int scalingTimes     = 1;
            float scalingTime    = GetScaleChangeTime(tra, scalingSpeed, endSize, endSize, scalingTimes);

            //�����l�w��
            tra.localScale = new Vector3(0.0f, 0.0f, 1.0f);

            //�g�k
            soundMan.GameClearSE();
            StartCoroutine(ScaleChange(tra, scalingSpeed, endSize, endSize, scalingTimes));
            yield return new WaitForSeconds(scalingTime);
            yield return new WaitForSeconds(4.0f);

            //�I�����}
            ShootModeMan.gameClearObjDis = false;
        }
    }
}