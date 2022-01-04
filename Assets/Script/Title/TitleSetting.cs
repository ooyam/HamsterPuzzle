using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundFunction;

public class TitleSetting : MonoBehaviour
{
    [Header("BGMスイッチ")]
    public Image bgmSwitchIma;
    private int bgmImageStatus = 0;
    [Header("SEスイッチ")]
    public Image seSwitchIma;
    private int seImageStatus = 0;
    [Header("スイッチSprite")]
    public Sprite[] switchSprite;  //0:ON 1:OFF

    private GameObject settingScreen;
    private GameObject creditScreen;
    private bool settingActive = false;
    private bool creditActive = false;
    private string settingTag;
    private GameObject SoundManObj;
    private SoundManager SoundMan;
    private Camera mainCamera;

    void Start()
    {
        Time.timeScale = 1.0f;
        mainCamera = Camera.main;
        Transform tra = this.transform;
        settingScreen = tra.GetChild(0).gameObject;
        creditScreen = tra.GetChild(1).gameObject;
        settingTag = settingScreen.transform.tag;
        SoundManObj = GameObject.FindWithTag("SoundManager");
        SoundMan = SoundManObj.GetComponent<SoundManager>();
        if (!EnvironmentalSetting.bgm)
        {
            bgmImageStatus = 1;
            SoundMan.BGM_Volume(0.0f);
            bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
        }
        if (!EnvironmentalSetting.se)
        {
            seImageStatus = 1;
            SoundMan.SE_Volume(0.0f);
            seSwitchIma.sprite = switchSprite[seImageStatus];
        }
    }

    void Update()
    {
        if (settingActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (!hit2d || hit2d.transform.gameObject.tag != settingTag)
                {
                    ExitButtonDown();
                }
            }
        }
    }
    public void SettingButtonDown()
    {
        if (!settingActive)
        {
            SoundMan.YesTapSE();
            settingScreen.SetActive(true);
            settingActive = true;
        }
    }

    public void BGM_ButtonDown()
    {
        if (bgmImageStatus == 0)
        {
            SoundMan.NoTapSE();
            bgmImageStatus = 1;
            EnvironmentalSetting.bgm = false;
            SoundMan.BGM_Volume(0.0f);
        }
        else
        {
            SoundMan.YesTapSE();
            bgmImageStatus = 0;
            EnvironmentalSetting.bgm = true;
            SoundMan.BGM_Volume(0.3f);
            SoundMan.BGM_Restart();
        }
        bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
    }

    public void SE_ButtonDown()
    {
        if (seImageStatus == 0)
        {
            seImageStatus = 1;
            EnvironmentalSetting.se = false;
            SoundMan.SE_Volume(0.0f);
        }
        else
        {
            seImageStatus = 0;
            EnvironmentalSetting.se = true;
            SoundMan.SE_Volume(1.0f);
            SoundMan.YesTapSE();
        }
        seSwitchIma.sprite = switchSprite[seImageStatus];
    }

    public void ExitButtonDown()
    {
        SoundMan.NoTapSE();
        if (creditActive)
        {
            creditScreen.SetActive(false);
            creditActive = false;
        }
        else
        {
            settingScreen.SetActive(false);
            settingActive = false;
        }
    }

    public void CreditDown()
    {
        SoundMan.YesTapSE();
        creditScreen.SetActive(true);
        creditActive = true;
    }
}
