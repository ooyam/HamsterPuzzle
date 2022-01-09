using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShootMode
{
    //野菜の種類
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
        public static int   BLOCK_MAX_LINE_NUM  = 12;     //ブロック最大数
        public static float BLOCK_GENERATE_TIME = 20.0f;  //ブロック生成時間
        public static float CANVAS_WIDTH;                 //Canvas幅
        public static float CANVAS_HIGH;                  //Canvas高さ

        //各フラグ
        public static bool GAME_START      = false;  //ゲーム開始？
        public static bool GAME_OVER       = false;  //ゲームオーバー？
        public static bool GAME_CLEAR      = false;  //ゲームクリア？
        public static bool SPECIAL_HARVEST = false;  //スペシャルハムスター動作中？

        //フラグリセット
        public static void FlagReset()
        {
            GAME_START      = false;
            GAME_OVER       = false;
            GAME_CLEAR      = false;
            SPECIAL_HARVEST = false;
        }
    }
}