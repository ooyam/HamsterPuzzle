using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingController : MonoBehaviour
{
    [Header("BGM�X�C�b�`")]
    public Image bgmSwitchIma;
    private int bgmImageStatus = 0;
    [Header("SE�X�C�b�`")]
    public Image seSwitchIma;
    private int seImageStatus = 0;
    [Header("�X�C�b�`Sprite")]
    public Sprite[] switchSprite;  //0:ON 1:OFF

    [Header("���ݒ���")]
    public GameObject settingScreen;
    [Header("�q���g���")]
    public GameObject hintScreen;
    [Header("�m�F���")]
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
    [Header("UI�J����")]
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
    //���ݒ��ʊO���^�b�v�����Ƃ�
    //========================================================================
    void Update()
    {
        //���ݒ��ʏo�͎�
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

        //�u�͂��v�u�������v�o�͎�
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

        //�q���g��ʏo�͎�
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
    //���ݒ��ʏo��
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
    //BGM�I���I�t
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
    //SE�I���I�t
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
    //�^�C�g���֖߂�
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
    //��蒼��
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
    //��͂��
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
    //��������
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
    //�q���g��ʏo��
    //========================================================================
    //display; �\�� or �폜
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
    //���ݒ��ʂ����
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