using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootMode
{
    //��؂̎��
    public enum VegetableType
    {
        Broccoli = 0,
        Cabbage,
        Paprika,
        Carrot,
        Pumpkin,
        Corn
    }

    public class ShootModeDefine : MonoBehaviour
    {
        public static bool  GAME_START = false;           //�Q�[���J�n�H
        public static bool  GAME_OVER  = false;           //�Q�[���I�[�o�[�H
        public static bool  GAME_CLEAR = false;           //�Q�[���N���A�H
        public static int   BLOCK_MAX_LINE_NUM  = 12;     //�u���b�N�ő吔
        public static float BLOCK_GENERATE_TIME = 20.0f;  //�u���b�N��������

        //�t���O���Z�b�g
        public static void FlagReset()
        {
            GAME_START = false;
            GAME_OVER  = false;
            GAME_CLEAR = false;
        }
    }
}