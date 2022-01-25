using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ShootMode.ShootModeDefine;
using static ShootMode.ShootModeManager;

namespace ShootMode
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("�X�R�A�p�����X�v���C�g")]
        [SerializeField]
        Sprite[] scoreNumSpr;

        [Header("�^�[�Q�b�g�p�����X�v���C�g")]
        [SerializeField]
        Sprite[] targetNumSpr;

        [Header("�X�R�A�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�p�v���J�@3:�j���W���@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] scoreNumTra; //���n��RectTransform
        Image[][] scoreNumIma;       //�o�͐���

        [Header("�^�[�Q�b�g�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�p�v���J�@3:�j���W���@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] targetObj;
        RectTransform[] targetObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] targetNumTra; //���n��RectTransform
        Image[][] targetNumIma;       //�o�͐���
        int maxDigit = 2;             //�ő包��
        float tarNumPosFixX = -26.0f; //�^�[�Q�b�g���ʕ\�����W�C���l

        VegetableType[] tarVeg;  //�ڕW���
        int tarVegNum;           //�ڕW��؂̌�
        int usingVegNum;         //�g�p�����؂̌�
        int[] tarNum;            //��ނ��Ƃ̖ڕW��
        int staNum;              //�X�e�[�W�ԍ�

        string[] vegName;  //��؂̖��O
        int vegTypeNum;    //��؂̐�
        int[] harvestNum;  //���n��
        int[] targetIndex; //�ڕW��؂̃C���f�N�X�ԍ�
        bool[] clearJudge; //�N���A����

        void Start()
        {
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegTypeNum = vegetableType.Length;
            vegName = new string[vegTypeNum];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            usingVegNum = useVegNum;
            Vector2[] scoreVegPos = new Vector2[usingVegNum];
            float[] scoreVegPosY  = new float[] { -65.0f, -135.0f, -200.0f };
            switch (usingVegNum)
            {
                case 3:
                    scoreVegPos[0] = new Vector2(-450.0f, scoreVegPosY[1]);
                    scoreVegPos[1] = new Vector2(-245.0f, scoreVegPosY[1]);
                    scoreVegPos[2] = new Vector2(-25.0f,  scoreVegPosY[1]);
                    break;
                case 4:
                    scoreVegPos[0] = new Vector2(-400.0f, scoreVegPosY[0]);
                    scoreVegPos[1] = new Vector2(-145.0f, scoreVegPosY[0]);
                    scoreVegPos[2] = new Vector2(-260.0f, scoreVegPosY[2]);
                    scoreVegPos[3] = new Vector2(20.0f,   scoreVegPosY[2]);
                    break;
                case 5:
                    scoreVegPos[0] = new Vector2(-300.0f, scoreVegPosY[0]);
                    scoreVegPos[1] = new Vector2(-85.0f,  scoreVegPosY[0]);
                    scoreVegPos[2] = new Vector2(-380.0f, scoreVegPosY[2]);
                    scoreVegPos[3] = new Vector2(-175.0f, scoreVegPosY[2]);
                    scoreVegPos[4] = new Vector2(45.0f,   scoreVegPosY[2]);
                    break;
            }

            scoreObjTra  = new RectTransform[usingVegNum];
            scoreNumTra  = new RectTransform[usingVegNum];
            scoreNumIma  = new Image[usingVegNum][];
            targetObjTra = new RectTransform[usingVegNum];
            targetNumTra = new RectTransform[usingVegNum];
            targetNumIma = new Image[usingVegNum][];
            for (int index = 0; index < usingVegNum; index++)
            {
                //�^�[�Q�b�g�I�u�W�F�N�g
                targetObjTra[index]    = targetObj[index].GetComponent<RectTransform>();
                targetNumTra[index]    = targetObjTra[index].GetChild(1).GetComponent<RectTransform>();
                targetNumIma[index]    = new Image[maxDigit];
                targetNumIma[index][0] = targetNumTra[index].GetChild(0).GetComponent<Image>();
                targetNumIma[index][1] = targetNumTra[index].GetChild(1).GetComponent<Image>();

                //�X�R�A�I�u�W�F�N�g
                scoreObjTra[index]    = scoreObj[index].GetComponent<RectTransform>();
                scoreNumTra[index]    = scoreObjTra[index].GetChild(0).GetComponent<RectTransform>();
                scoreNumIma[index]    = new Image[maxDigit];
                scoreNumIma[index][0] = scoreNumTra[index].GetChild(0).GetComponent<Image>();
                scoreNumIma[index][1] = scoreNumTra[index].GetChild(1).GetComponent<Image>();

                //�X�R�A�I�u�W�F�N�g���W�w��
                scoreObj[index].SetActive(true);
                if (vegTypeNum > usingVegNum) scoreObjTra[index].anchoredPosition = scoreVegPos[index];
            }
            harvestNum = new int[usingVegNum];

            //�X�e�[�W�ݒ�
            tarVeg      = tartgetVeg;          //�ڕW���
            tarVegNum   = targetVegNum;        //�ڕW��؂̌�
            tarNum      = targetNum;           //��ނ��Ƃ̖ڕW��
            staNum      = stageNum;            //�X�e�[�W�ԍ�
            targetIndex = new int[tarVegNum];  //�ڕW��؂̃C���f�N�X�ԍ�
            clearJudge  = new bool[tarVegNum]; //�N���A����
            int ten     = 10;
            for (int index = 0; index < tarVegNum; index++)
            {
                int tensPlace = (int)Mathf.Floor(tarNum[index] / ten); //10�̈�
                int onesPlace = tarNum[index] % ten;                   //1�̈�
                targetIndex[index] = (int)tarVeg[index];
                targetNumIma[targetIndex[index]][0].sprite = targetNumSpr[onesPlace];
                targetNumIma[targetIndex[index]][1].sprite = (tensPlace == 0) ? targetNumSpr[ten] : targetNumSpr[tensPlace];
                if (tensPlace == 0) targetNumTra[targetIndex[index]].anchoredPosition = new Vector2(tarNumPosFixX, 0.0f);
                clearJudge[index] = false;
            }

            //�^�[�Q�b�g�I�u�W�F�N�g���W�w��
            float valueX = 130.0f;
            float valueY = -20.0f;
            float fixX   = valueX / 2.0f * (tarVegNum - 1);
            for (int index = 0; index < tarVegNum; index++)
            {
                targetObjTra[targetIndex[index]].anchoredPosition = new Vector2(valueX * index - fixX, valueY);
                targetObj[targetIndex[index]].SetActive(true);
            }

        }


        //========================================================================
        //��؎��n
        //========================================================================
        //vegetableName; ��؂̖��O(�^�O��)
        //========================================================================
        public void HarvestVegetable(string vegetableName)
        {
            int vegIndex = Array.IndexOf(vegName, vegetableName);
            if (vegIndex >= 0)
            {
                if (harvestNum[vegIndex] < 99) harvestNum[vegIndex]++;  //�J���X�g99

                //�������f
                int ten = 10;
                int tensPlace = (int)Mathf.Floor(harvestNum[vegIndex] / ten); //10�̈�
                int onesPlace = harvestNum[vegIndex] % ten;                   //1�̈�

                //�����X�V
                scoreNumIma[vegIndex][0].sprite = scoreNumSpr[onesPlace];
                scoreNumIma[vegIndex][1].sprite = (tensPlace == 0) ? scoreNumSpr[ten] : scoreNumSpr[tensPlace];

                //�ڕW��؎��n�������f
                int targetIndexIndex = Array.IndexOf(targetIndex, vegIndex);
                if (targetIndexIndex >= 0 && !clearJudge[targetIndexIndex] && tarNum[targetIndexIndex] <= harvestNum[vegIndex])
                {
                    //���n��������
                    clearJudge[targetIndexIndex] = true;
                    targetNumIma[vegIndex][0].sprite = targetNumSpr[11];
                    targetNumIma[vegIndex][1].sprite = targetNumSpr[10];
                    targetNumTra[vegIndex].anchoredPosition = new Vector2(0.0f, 0.0f);

                    //�N���A����
                    if (!GAME_CLEAR && Array.IndexOf(clearJudge, false) < 0)
                        GAME_CLEAR = true;
                }
            }
        }
    }
}
