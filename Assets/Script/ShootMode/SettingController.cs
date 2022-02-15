using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SoundFunction;
using UnityEngine.SceneManagement;
using static ShootMode.ShootModeDefine;

namespace ShootMode
{
    public class SettingController : MonoBehaviour
    {
        [Header("UIカメラ")]
        [SerializeField]
        Camera uiCamera;

        [Header("BGMスイッチ")]
        [SerializeField]
        Image bgmSwitchIma;
        int bgmImageStatus = 0;

        [Header("SEスイッチ")]
        [SerializeField]
        Image seSwitchIma;
        int seImageStatus = 0;

        [Header("スイッチSprite")]
        [SerializeField]
        Sprite[] switchSprite;  //0:ON 1:OFF

        [Header("環境設定画面")]
        [SerializeField]
        GameObject settingScreen;

        [Header("確認画面")]
        [SerializeField]
        GameObject messageBox;

        [Header("ハムスター")]
        [SerializeField]
        HamsterController hamsterCon;

        AudioSource Audio;                 //AudioSource
        bool settingActive = false;        //設定画面表示中？
        string settingTag;                 //設定画面のタグ
        GameObject[] messageText;          //メッセージボックス
        bool messageActive = false;        //メッセージボックス表示中？
        string messageTag;                 //メッセージボックスタグ
        GameObject soundManObj;            //soundManagerオブジェクト
        SoundManager soundMan;             //soundManagerスクリプト

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
            soundManObj = GameObject.FindWithTag("SoundManager");
            soundMan = soundManObj.GetComponent<SoundManager>();

            if (!EnvironmentalSetting.bgm)
            {
                bgmImageStatus = 1;
                soundMan.BGM_Volume(0.0f);
                bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
            }
            if (!EnvironmentalSetting.se)
            {
                seImageStatus = 1;
                soundMan.SE_Volume(0.0f);
                seSwitchIma.sprite = switchSprite[seImageStatus];
                Audio.volume = 0.0f;
            }

            //各ボタンに関数指定
            Transform[] childrenTra = new Transform[] { tra.parent.GetChild(1), tra.parent.GetChild(2) };
            tra.GetComponent<Button>().onClick.AddListener(() => SettingButtonDown());                          //設定ボタン
            childrenTra[0].GetChild(0).GetComponent<Button>().onClick.AddListener(() => BGM_ButtonDown());      //BGM
            childrenTra[0].GetChild(1).GetComponent<Button>().onClick.AddListener(() => SE_ButtonDown());       //SE
            childrenTra[0].GetChild(2).GetComponent<Button>().onClick.AddListener(() => TitlBackButtonDown());  //タイトルへ戻る
            childrenTra[0].GetChild(3).GetComponent<Button>().onClick.AddListener(() => RetryButtonDown());     //やり直す
            childrenTra[0].GetChild(4).GetComponent<Button>().onClick.AddListener(() => ExitButtonDown());      //Exit
            childrenTra[1].GetChild(2).GetComponent<Button>().onClick.AddListener(() => YesButtonDown());       //はい
            childrenTra[1].GetChild(3).GetComponent<Button>().onClick.AddListener(() => NoButtonDown());        //いいえ
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
        }

        //========================================================================
        //環境設定画面出力
        //========================================================================
        void SettingButtonDown()
        {
            if (!settingActive)
            {
                soundMan.YesTapSE();
                Time.timeScale = 0.0f;
                SETTING_DISPLAY = true;
                settingScreen.SetActive(true);
                settingActive = true;
            }
        }

        //========================================================================
        //BGMオンオフ
        //========================================================================
        void BGM_ButtonDown()
        {
            if (bgmImageStatus == 0)
            {
                soundMan.NoTapSE();
                bgmImageStatus = 1;
                EnvironmentalSetting.bgm = false;
                soundMan.BGM_Volume(0.0f);
            }
            else
            {
                soundMan.YesTapSE();
                bgmImageStatus = 0;
                EnvironmentalSetting.bgm = true;
                soundMan.BGM_Volume(1.0f);
                soundMan.BGM_Restart();
            }
            bgmSwitchIma.sprite = switchSprite[bgmImageStatus];
        }

        //========================================================================
        //SEオンオフ
        //========================================================================
        void SE_ButtonDown()
        {
            if (seImageStatus == 0)
            {
                seImageStatus = 1;
                EnvironmentalSetting.se = false;
                soundMan.SE_Volume(0.0f);
                Audio.volume = 0.0f;
            }
            else
            {
                seImageStatus = 0;
                EnvironmentalSetting.se = true;
                soundMan.SE_Volume(1.0f);
                Audio.volume = 1.0f;
                soundMan.YesTapSE();
            }
            seSwitchIma.sprite = switchSprite[seImageStatus];
        }

        //========================================================================
        //タイトルへ戻る
        //========================================================================
        void TitlBackButtonDown()
        {
            soundMan.YesTapSE();
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
        void RetryButtonDown()
        {
            soundMan.YesTapSE();
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
        void YesButtonDown()
        {
            if (messageText[0].activeSelf)
            {
                Audio.Play();
                Destroy(soundManObj);
                SceneNavigator.Instance.Change("TitleScene", 0.5f);
            }
            else if (messageText[1].activeSelf)
            {
                soundMan.YesTapSE();
                SceneNavigator.Instance.Change(SceneManager.GetActiveScene().name, 0.5f);
            }
        }

        //========================================================================
        //｢いいえ｣
        //========================================================================
        void NoButtonDown()
        {
            soundMan.NoTapSE();
            settingScreen.SetActive(true);
            settingActive = true;
            messageBox.SetActive(false);
            messageActive = false;
        }

        //========================================================================
        //環境設定画面を閉じる
        //========================================================================
        void ExitButtonDown()
        {
            soundMan.NoTapSE();
            Time.timeScale = 1.0f;
            SETTING_DISPLAY = false;
            settingScreen.SetActive(false);
            settingActive = false;
        }
    }
}