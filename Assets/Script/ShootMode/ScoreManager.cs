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
        [Header("�X�R�A�\���I�u�W�F�N�g")]   //0�F�u���b�R���[�@1:�L���x�c�@2�F�p�v���J�@3:�j���W���@4�F�J�{�`���@5�F�g�E�����R�V
        [SerializeField]
        GameObject[] scoreObj;
        RectTransform[] scoreObjTra; //�I�u�W�F�N�gRectTransform
        RectTransform[] numTra;      //���n��RectTransform
        Image[][] numIma;            //�o�͐���
        int maxDigit = 2;            //�ő包��

        int vegTypeNum = Enum.GetValues(typeof(VegetableType)).Length; //�g�p�����؂̐�
        int[] harvestNum; //���n��

        void Start()
        {
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
    }
}
