using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ShootModeManager : MonoBehaviour
    {
        [Header("�w�i�I�u�W�F�N�gTra")]
        [SerializeField]
        RectTransform backGroundTra;

        [Header("�Q�[���I�[�o�[�v���n�u")]
        [SerializeField]
        GameObject gameOverObjPre;
        [System.NonSerialized]
        public bool gameOverObjDis = false;  //�\�����t���O

        [Header("�Q�[���N���A�v���n�u")]
        [SerializeField]
        GameObject gameClearObjPre;

        [Header("���U���g��ʃv���n�u")]
        [SerializeField]
        GameObject resultScreenPre;

        void Awake()
        {
            FlagReset();
        }

        public IEnumerator GameOver()
        {
            if (!gameOverObjDis)
            {
                GAME_OVER = true;

                //�Q�[���I�[�o�[�I�u�W�F�N�g����
                gameOverObjDis = true;
                GameObject gameOverObj = Instantiate(gameOverObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameOverObj>().DirectGameOver(this));
                yield return new WaitWhile(() => gameOverObjDis == true);

                //���U���g��ʕ\��
                GameObject resultScreenObj = Instantiate(resultScreenPre);
                resultScreenObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
            }
        }
    }
}