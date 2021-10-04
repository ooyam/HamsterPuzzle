using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingController : MonoBehaviour
{
    [Header("BGMスイッチ")]
    public Image bgmSwitchIma;
    private int bgmImageStatus = 0;
    [Header("SEスイッチ")]
    public Image seSwitchIma;
    private int seImageStatus = 0;
    [Header("スイッチSprite")]
    public Sprite[] switchSprite;  //0:ON 1:OFF

    [Header("環境設定画面")]
    public GameObject settingScreen;
    [Header("ヒント画面")]
    public GameObject hintScreen;
    [Header("確認画面")]
    public GameObject messageBox;

    private AudioSource Audio;
    private bool settingActive = false;
    private string settingTag;
    private GameObject[] messageText;
    private bool messageActive = false;
    private string messageTag;
    private bool hintActive = false;
    private string hintTag;
    private GameObject SoundManObj;
    private SoundManager SoundMan;
    private HamsterPanelController hamsterCon;
    [Header("UIカメラ")]
    public Camera uiCamera;

    void Start()
    {
        Time.timeScale = 1.0f;
        Audio = this.GetComponent<AudioSource>();
        Transform tra = this.transform;
        settingTag = settingScreen.transform.tag;
        hintTag = hintScreen.transform.tag;
        Transform mesTra = messageBox.transform;
        messageTag = mesTra.tag;
        messageText = new GameObject[]
        {
            mesTra.GetChild(0).gameObject,
            mesTra.GetChild(1).gameObject
        };
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
            Audio.volume = 0.0f;
        }
    }

    //========================================================================
    //環境設定画面外をタップしたとき
    //========================================================================
    void Update()
    {
        //環境設定画面出力時
        if (settingActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (!hit2d || hit2d.transform.gameObject.tag != settingTag)
                {
                    ExitButtonDown();
                }
            }
        }

        //「はい」「いいえ」出力時
        if (messageActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (!hit2d || hit2d.transform.gameObject.tag != messageTag)
                {
                    NoButtonDown();
                }
            }
        }

        //ヒント画面出力時
        if (hintActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (!hit2d || hit2d.transform.gameObject.tag != hintTag)
                {
                    HintButtonDown(false);
                }
            }
        }
    }

    //========================================================================
    //環境設定画面出力
    //========================================================================
    public void SettingButtonDown()
    {
        if (!settingActive)
        {
            if (hamsterCon == null) hamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterPanelController>();
            SoundMan.YesTapSE();
            Time.timeScale = 0.0f;
            hamsterCon.setting = true;
            settingScreen.SetActive(true);
            settingActive = true;
        }
    }

    //========================================================================
    //BGMオンオフ
    //========================================================================
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
            SoundMan.BGM_Volume(1.0f);
            StartCoroutine(SoundMan.BGM_Start(SoundMan.bgmIndex));
        }
        bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
    }

    //========================================================================
    //SEオンオフ
    //========================================================================
    public void SE_ButtonDown()
    {
        if (seImageStatus == 0)
        {
            seImageStatus = 1;
            EnvironmentalSetting.se = false;
            SoundMan.SE_Volume(0.0f);
            Audio.volume = 0.0f;
        }
        else
        {
            seImageStatus = 0;
            EnvironmentalSetting.se = true;
            SoundMan.SE_Volume(1.0f);
            Audio.volume = 1.0f;
            SoundMan.YesTapSE();
        }
        seSwitchIma.sprite = switchSprite[seImageStatus];
    }

    //========================================================================
    //タイトルへ戻る
    //========================================================================
    public void TitlBackButtonDown()
    {
        SoundMan.YesTapSE();
        settingScreen.SetActive(false);
        settingActive = false;
        messageBox.SetActive(true);
        messageActive = true;
        messageText[0].SetActive(true);
        messageText[1].SetActive(false);
    }

    //========================================================================
    //やり直す
    //========================================================================
    public void RetryButtonDown()
    {
        SoundMan.YesTapSE();
        settingScreen.SetActive(false);
        settingActive = false;
        messageBox.SetActive(true);
        messageActive = true;
        messageText[0].SetActive(false);
        messageText[1].SetActive(true);
    }

    //========================================================================
    //｢はい｣
    //========================================================================
    public void YesButtonDown()
    {
        if (messageText[0].activeSelf)
        {
            Audio.Play();
            Destroy(SoundManObj);
            SceneNavigator.Instance.Change("TitleScene", 0.5f);
        }
        else if (messageText[1].activeSelf)
        {
            SoundMan.YesTapSE();
            SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
        }
    }

    //========================================================================
    //｢いいえ｣
    //========================================================================
    public void NoButtonDown()
    {
        SoundMan.NoTapSE();
        settingScreen.SetActive(true);
        settingActive = true;
        messageBox.SetActive(false);
        messageActive = false;
    }

    //========================================================================
    //ヒント画面出力
    //========================================================================
    //display; 表示 or 削除
    //========================================================================
    public void HintButtonDown(bool display)
    {
        if (display)
        {
            SoundMan.YesTapSE();
            settingScreen.SetActive(false);
            settingActive = false;
            hintScreen.SetActive(true);
            hintActive = true;
        }
        else
        {
            SoundMan.NoTapSE();
            settingScreen.SetActive(true);
            settingActive = true;
            hintScreen.SetActive(false);
            hintActive = false;
        }
    }

    //========================================================================
    //環境設定画面を閉じる
    //========================================================================
    public void ExitButtonDown()
    {
        SoundMan.NoTapSE();
        Time.timeScale = 1.0f;
        hamsterCon.setting = false;
        settingScreen.SetActive(false);
        hintScreen.SetActive(false);
        settingActive = false;
    }
}