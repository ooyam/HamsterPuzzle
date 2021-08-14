using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalorieGauge : MonoBehaviour
{
    private Transform spriteTra;     //RectTransform
    private Slider sliderGauge;      //スライダー
    private int maxCalorie = 30;     //最大カロリー
    private int remainingCalorie;    //残りカロリー

    private float moveSpeed = 0.01f;
    private float maxScale = 1.1f;
    private float minScale = 1.0f;
    private float scale = 1.0f;
    private bool expansion = true;

    [System.NonSerialized]
    public bool moveStart = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteTra = transform.GetChild(2).transform.GetChild(0).transform;
        sliderGauge = GetComponent<Slider>();
        sliderGauge.maxValue = maxCalorie;
        remainingCalorie = maxCalorie;
        sliderGauge.value = remainingCalorie;
    }

    //ハムスターが移動した時
    public bool HamsterMoved()
    {
        bool calorieZero = false;
        if(remainingCalorie > 0) remainingCalorie--;
        if (remainingCalorie == 0) calorieZero = true;
        spriteTra.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        sliderGauge.value = remainingCalorie;
        if (!moveStart && remainingCalorie <= 5) moveStart = true;
        return calorieZero;
    }

    //野菜収穫時
    public void VegetableHarvest(bool calRecovery)
    {
        if (calRecovery)
        {
            remainingCalorie += 10;
            if (remainingCalorie > maxCalorie)
                remainingCalorie = maxCalorie;
        }
        else if (remainingCalorie < maxCalorie) remainingCalorie++;
        spriteTra.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        sliderGauge.value = remainingCalorie;
        if (moveStart && remainingCalorie > 5)
        {
            moveStart = false;
            spriteTra.localScale = new Vector2(minScale, minScale);
        }
    }

    void FixedUpdate()
    {
        if (moveStart)
        {
            if (expansion)
            {
                scale += moveSpeed;
                if (scale > maxScale)
                    expansion = false;
            }
            else
            {
                scale -= moveSpeed;
                if (scale < minScale)
                    expansion = true;
            }
            spriteTra.localScale = new Vector2(scale, scale);
        }
    }
}
