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

        [Header("�X�R�A�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�j���W���@3:�p�v���J�@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] scoreNumTra; //���n��RectTransform
        Image[][] scoreNumIma;       //�o�͐���

        [Header("�^�[�Q�b�g�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�j���W���@3:�p�v���J�@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] targetObj;
        RectTransform[] targetObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] targetNumTra; //���n��RectTransform
        Image[][] targetNumIma;       //�o�͐���
        int maxDigit = 2;             //�ő包��

        VegetableType[] tartgetVeg_;  //�ڕW���
        int targetVegetableNum_;      //�ڕW��؂̌�
        int[] targetNum_;             //��ނ��Ƃ̖ڕW��
        int stageNum_;                //�X�e�[�W�ԍ�

        string[] vegName;  //��؂̖��O
        int vegTypeNum;    //�g�p�����؂̐�
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

            scoreObjTra  = new RectTransform[vegTypeNum];
            scoreNumTra  = new RectTransform[vegTypeNum];
            scoreNumIma  = new Image[vegTypeNum][];
            targetObjTra = new RectTransform[vegTypeNum];
            targetNumTra = new RectTransform[vegTypeNum];
            targetNumIma = new Image[vegTypeNum][];
            for (int index = 0; index < vegTypeNum; index++)
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
            }
            harvestNum = new int[vegTypeNum];

            //�X�e�[�W�ݒ�
            tartgetVeg_         = tartgetVeg;            //�ڕW���
            targetVegetableNum_ = targetVegetableNum;    //�ڕW��؂̌�
            targetNum_          = targetNum;             //��ނ��Ƃ̖ڕW��
            stageNum_           = stageNum;              //�X�e�[�W�ԍ�
            targetIndex = new int[targetVegetableNum_];  //�ڕW��؂̃C���f�N�X�ԍ�
            clearJudge  = new bool[targetVegetableNum_]; //�N���A����
            int ten = 10;
            for (int index = 0; index < targetVegetableNum_; index++)
            {
                int tensPlace = (int)Mathf.Floor(targetNum_[index] / ten); //10�̈�
                int onesPlace = targetNum_[index] % ten;                   //1�̈�
                targetIndex[index] = (int)tartgetVeg_[index];
                targetNumIma[targetIndex[index]][0].sprite = targetNumSpr[onesPlace];
                targetNumIma[targetIndex[index]][1].sprite = (tensPlace == 0) ? targetNumSpr[ten] : targetNumSpr[tensPlace];
                clearJudge[index] = false;
            }
        }

        //��؎��n
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
                if (targetIndexIndex >= 0 && targetNum_[targetIndexIndex] <= harvestNum[vegIndex])
                {
                    clearJudge[targetIndexIndex] = true;

                    //�N���A����
                    if (Array.IndexOf(clearJudge, false) < 0)
                        Debug.Log("�N���A�ł�����");
                }
            }
        }
    }
}
