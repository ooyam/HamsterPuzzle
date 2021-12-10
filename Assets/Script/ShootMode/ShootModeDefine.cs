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
        Carrot,
        Paprika,
        Pumpkin,
        Corn
    }

    public class ShootModeDefine : MonoBehaviour
    {
        public static int   BLOCK_MAX_LINE_NUM  = 12;     //ブロック最大数
        public static float BLOCK_GENERATE_TIME = 20.0f;  //ブロック生成時間
    }
}