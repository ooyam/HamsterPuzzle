using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SoundFunction;
using UnityEngine.SceneManagement;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class SettingController_Tutorial : MonoBehaviour
    {
        [Header("UI�J����")]
        [SerializeField]
        Camera uiCamera;

        [Header("BGM�X�C�b�`")]
        [SerializeField]
        Image bgmSwitchIma;
        int bgmImageStatus = 0;

        [Header("SE�X�C�b�`")]
        [SerializeField]
        Image seSwitchIma;
        int seImageStatus = 0;

        [Header("�X�C�b�`Sprite")]
        [SerializeField]
        Sprite[] switchSprite;  //0:ON 1:OFF

        [Header("���ݒ���")]
        [SerializeField]
        GameObject settingScreen;

        [Header("�m�F���")]
        [SerializeField]
        GameObject messageBox;

        [Header("�n���X�^�[")]
        [SerializeField]
        HamsterController_Tutorial hamsterCon;

        AudioSource Audio;                 //AudioSource
        bool settingActive = false;        //�ݒ��ʕ\�����H
        string settingTag;                 //�ݒ��ʂ̃^�O
        GameObject[] messageText;          //���b�Z�[�W�{�b�N�X
        bool messageActive = false;        //���b�Z�[�W�{�b�N�X�\�����H
        string messageTag;                 //���b�Z�[�W�{�b�N�X�^�O
        GameObject SoundManObj;            //SoundManager�I�u�W�F�N�g
        SoundManager SoundMan;             //SoundManager�X�N���v�g

        void Start()
        {
            Time.timeScale = 1.0f;
            Audio = this.GetComponent<AudioSource>();
            Transform tra = this.transform;
            settingTag = settingScreen.transform.tag;
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

            //�e�{�^���Ɋ֐��w��
            Transform[] childrenTra = new Transform[] { tra.GetChild(0), tra.GetChild(1) };
            tra.GetComponent<Button>().onClick.AddListener(() => SettingButtonDown());                          //�ݒ�{�^��
            childrenTra[0].GetChild(0).GetComponent<Button>().onClick.AddListener(() => BGM_ButtonDown());      //BGM
            childrenTra[0].GetChild(1).GetComponent<Button>().onClick.AddListener(() => SE_ButtonDown());       //SE
            childrenTra[0].GetChild(2).GetComponent<Button>().onClick.AddListener(() => TitlBackButtonDown());  //�^�C�g���֖߂�
            childrenTra[0].GetChild(3).GetComponent<Button>().onClick.AddListener(() => RetryButtonDown());     //��蒼��
            childrenTra[0].GetChild(4).GetComponent<Button>().onClick.AddListener(() => ExitButtonDown());      //Exit
            childrenTra[1].GetChild(2).GetComponent<Button>().onClick.AddListener(() => YesButtonDown());       //�͂�
            childrenTra[1].GetChild(3).GetComponent<Button>().onClick.AddListener(() => NoButtonDown());        //������
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
        }

        //========================================================================
        //���ݒ��ʏo��
        //========================================================================
        void SettingButtonDown()
        {
            if (!settingActive)
            {
                SoundMan.YesTapSE();
                Time.timeScale = 0.0f;
                SETTING_DISPLAY = true;
                settingScreen.SetActive(true);
                settingActive = true;
            }
        }

        //========================================================================
        //BGM�I���I�t
        //========================================================================
        void BGM_ButtonDown()
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
                SoundMan.BGM_Restart();
            }
            bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
        }

        //========================================================================
        //SE�I���I�t
        //========================================================================
        void SE_ButtonDown()
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
        void TitlBackButtonDown()
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
        void RetryButtonDown()
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
        void YesButtonDown()
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
        void NoButtonDown()
        {
            SoundMan.NoTapSE();
            settingScreen.SetActive(true);
            settingActive = true;
            messageBox.SetActive(false);
            messageActive = false;
        }

        //========================================================================
        //���ݒ��ʂ����
        //========================================================================
        void ExitButtonDown()
        {
            SoundMan.NoTapSE();
            Time.timeScale = 1.0f;
            SETTING_DISPLAY = false;
            settingScreen.SetActive(false);
            settingActive = false;
        }
    }
}