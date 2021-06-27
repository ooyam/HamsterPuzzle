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
        return calorieZero;
    }

    //野菜収穫時
    public void VegetableHarvest()
    {
        if (remainingCalorie < maxCalorie) remainingCalorie++;
        spriteTra.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        sliderGauge.value = remainingCalorie;
    }
}
