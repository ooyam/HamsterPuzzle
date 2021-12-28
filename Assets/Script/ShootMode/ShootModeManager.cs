using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

        public static VegetableType[] tartgetVeg;   //�ڕW���
        public static int targetVegetableNum;       //�ڕW��؂̌�
        public static int[] targetNum;              //��ނ��Ƃ̖ڕW��
        public static float blickGenerateTime;      //��������
        public static int stageNum;                 //�X�e�[�W�ԍ�
        public static bool tutorial;        �@      //�`���[�g���A���H
        string[] vegName;                           //��؂̖��Oenum

        void Awake()
        {
            FlagReset();

            //enum�擾
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegName = new string[vegetableType.Length];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            //�X�e�[�W�ݒ�(��)
            tartgetVeg = new VegetableType[] { VegetableType.Broccoli };
            targetVegetableNum = 1;
            targetNum = new int[] { 1 };
            blickGenerateTime = 15.0f;
            stageNum = 0;
            tutorial = false;
            BLOCK_GENERATE_TIME = blickGenerateTime;
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