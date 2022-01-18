using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SoundFunction;
using ShootMode;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class ShootModeManager_Tutorial : MonoBehaviour
    {
        [Header("�w�i�I�u�W�F�N�gTra")]
        [SerializeField]
        RectTransform backGroundTra;

        [Header("�S�����t�B�[�o�[�n���X�^�[�v���n�u")]
        [SerializeField]
        GameObject feverPre;

        [Header("�Q�[���I�[�o�[�v���n�u")]
        [SerializeField]
        GameObject gameOverObjPre;
        [System.NonSerialized]
        public bool gameOverObjDis = false;  //�\�����t���O

        [Header("�Q�[���N���A�v���n�u")]
        [SerializeField]
        GameObject gameClearObjPre;
        [System.NonSerialized]
        public bool gameClearObjDis = false;  //�\�����t���O

        [Header("���U���g��ʃv���n�u")]
        [SerializeField]
        GameObject resultScreenPre;

        GameObject soundManObj;
        SoundManager soundManScr;  //SoundManger

        public static VegetableType[] tartgetVeg;   //�ڕW���
        public static int targetVegNum;             //�ڕW��؂̌�
        public static int useVegNum;                //�g�p�����؂̌�
        public static int[] targetNum;              //��ނ��Ƃ̖ڕW��
        public static float blickGenerateTime;      //��������
        public static int stageNum;                 //�X�e�[�W�ԍ�
        string[] vegName;                           //��؂̖��Oenum

        void Awake()
        {
            //�e�t���O���Z�b�g
            FlagReset();

            //�Q�[���J�n
            GAME_START = true;

            //SoundoManager�擾
            soundManObj = GameObject.FindWithTag("SoundManager");
            soundManScr = soundManObj.GetComponent<SoundManager>();

            //enum�擾
            var vegetableType = Enum.GetValues(typeof(VegetableType));
            vegName = new string[vegetableType.Length];
            foreach (VegetableType vegeValue in vegetableType)
            { vegName[(int)vegeValue] = Enum.GetName(typeof(VegetableType), vegeValue); }

            //�萔�ݒ�
            BLOCK_GENERATE_TIME         = blickGenerateTime;
            RectTransform canvasTra     = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            RectTransform playScreenTra = canvasTra.GetChild(1).GetComponent<RectTransform>();
            CANVAS_WIDTH       = canvasTra.sizeDelta.x;
            CANVAS_HEIGHT      = canvasTra.sizeDelta.y;
            PLAY_SCREEN_WIDTH  = playScreenTra.sizeDelta.x;
            PLAY_SCREEN_HEIGHT = playScreenTra.sizeDelta.y;

            //BGM�J�n
            int bgmIndex = 1;
            if (stageNum >= 8 && stageNum <= 14) bgmIndex = 2;
            else if (stageNum >= 15) bgmIndex = 3;
            soundManScr.BGM_Start(bgmIndex);
        }

        //========================================================================
        //�t�B�[�o�[
        //========================================================================
        public void Fever()
        {
            if (!FEVER_START)
            {
                FEVER_START = true;

                //�t�B�[�o�[�I�u�W�F�N�g����
                GameObject feverObj = Instantiate(feverPre);
                feverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(feverObj.GetComponent<FeverHamuster_Tutorial>().FeverStart());
            }
        }


        //========================================================================
        //�Q�[���I�[�o�[
        //========================================================================
        public IEnumerator GameOver()
        {
            if (!gameOverObjDis)
            {
                GAME_OVER = true;

                //�Q�[���I�[�o�[�I�u�W�F�N�g����
                gameOverObjDis = true;
                GameObject gameOverObj = Instantiate(gameOverObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameOverObj_Tutorial>().DirectGameOver(this, soundManScr));
                yield return new WaitWhile(() => gameOverObjDis == true);

                //���U���g��ʕ\��
                GameObject resultScreenObj = Instantiate(resultScreenPre);
                resultScreenObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
            }
        }

        //========================================================================
        //�Q�[���N���A
        //========================================================================
        public IEnumerator GameClear()
        {
            if (!gameClearObjDis)
            {
                //�Q�[���N���A�I�u�W�F�N�g����
                gameClearObjDis = true;
                GameObject gameOverObj = Instantiate(gameClearObjPre);
                gameOverObj.GetComponent<RectTransform>().SetParent(backGroundTra, false);
                StartCoroutine(gameOverObj.GetComponent<GameClearObj_Tutorial>().DirectGameClear(this, soundManScr));
                yield return new WaitWhile(() => gameClearObjDis == true);

                //�N���A�X�e�[�W�ԍ���������
                GameObject.FindWithTag("SaveDataManager").GetComponent<SaveDataManager>().WriteShootModeSaveData(stageNum);

                //�^�C�g����ʂ֖߂�
                Destroy(soundManObj);
                SceneNavigator.Instance.Change("TitleScene", 1.0f);
            }
        }
    }
}