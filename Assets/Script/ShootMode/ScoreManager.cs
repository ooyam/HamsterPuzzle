using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("�����X�v���C�g")]
        [SerializeField]
        Sprite[] numberSpr;

        [Header("�X�R�A�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�j���W���@3:�p�v���J�@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] scoreObj;

        RectTransform[] scoreObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] numTra;      //���n��RectTransform
        Image[][] numIma;            //�o�͐���
        int maxDigit = 2;            //�ő包��

        string[] vegName; //��؂̖��O
        int vegTypeNum;   //�g�p�����؂̐�
        int[] harvestNum; //���n��

        void Start()
        {
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegTypeNum = vegetableType.Length;
            vegName = new string[vegTypeNum];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            scoreObjTra = new RectTransform[vegTypeNum];
            numTra      = new RectTransform[vegTypeNum];
            numIma      = new Image[vegTypeNum][];
            for (int index = 0; index < vegTypeNum; index++)
            {
                scoreObjTra[index] = scoreObj[index].GetComponent<RectTransform>();
                numTra[index]      = scoreObjTra[index].GetChild(0).GetComponent<RectTransform>();
                numIma[index]      = new Image[maxDigit];
                numIma[index][0]   = numTra[index].GetChild(0).GetComponent<Image>();
                numIma[index][1]   = numTra[index].GetChild(1).GetComponent<Image>();
            }
            harvestNum = new int[vegTypeNum];
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
                numIma[vegIndex][0].sprite = numberSpr[onesPlace];
                numIma[vegIndex][1].sprite = (tensPlace == 0) ? numberSpr[ten] : numberSpr[tensPlace];
            }
        }
    }
}
