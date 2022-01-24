using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundFunction;
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("�G�t�F�N�g")]
        [SerializeField]
        GameObject pointEffPre;

        [Header("�t�B���^�[�{�b�N�X")]
        [SerializeField]
        Transform filterBoxTra;

        [Header("�e�L�X�g�{�b�N�X")]
        [SerializeField]
        Transform textBoxTra;

        [Header("�G�t�F�N�g�{�b�N�X")]
        [SerializeField]
        Transform effBoxTra;

        [Header("��")]
        [SerializeField]
        RectTransform handTra;
        GameObject handObj;
        Image handIma;
        bool handShow;     //��\���t���O
        bool handNowMove;  //�蓮�쒆�t���O

        [Header("�Q�[���I�[�o�[���C��")]
        [SerializeField]
        Image gameOverLineIma;

        HamsterController_Tutorial hamsterCon;   //HamsterController_Tutorial
        BlockManager_Tutorial blockMan;          //BlockManager_Tutorial
        SoundManager SoundMan;                   //SoundManager

        GameObject[] filterObj;       //�t�B���^�[Obj
        Image fullFilterIma;          //�S�ʃt�B���^�[gImae
        Image[][] fadeFilter;         //�t�F�[�hImage
        GameObject[] textObj;         //�e�L�X�gObj
        Image[] fadeTextFilter;       //�t�F�[�hText�p�t�B���^�[

        [System.NonSerialized]
        public int descriptionNum = 0;     //�����ԍ�
        float minDispalyTime      = 1.5f;  //�����Œ�\������
        float imaFadeSpeed        = 0.2f;  //�t�F�[�h���x

        int[] alphaFadeComp = new int[] { 3, 3 };                                                //��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        Color[] appearance  = new Color[] { new Color(1, 1, 1, 0), Color.white };                //���� �� ��
        Color[] transparent = new Color[] { Color.white, new Color(1, 1, 1, 0) };                //�� �� ����
        Color[] filterColor = new Color[] { new Color(0, 0, 0, 160.0f / 255.0f), Color.clear };  //�������� �� ����
        Color[] blackout    = new Color[] { Color.clear, Color.black };                          //���� �� ��
        Color[] lightUp     = new Color[] { Color.black, new Color(0, 0, 0, 160.0f / 255.0f) };  //�� �� ��������
        Color[] lineColor   = new Color[] { Color.white, new Color(1, 1, 1, 50.0f / 255.0f) };   //�� �� ������

        [System.NonSerialized]
        public bool throwWait;            //�����ҋ@�t���O
        [System.NonSerialized]
        public bool specialHamsterWait;   //�X�y�V�����I���t�ҋ@�t���O
        [System.NonSerialized]
        public bool throwBlockChangeWait; //next�u���b�N�^�b�v�ҋ@�t���O
        bool rethrow;                     //������蒼���t���O

        void Start()
        {
            hamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterController_Tutorial>();
            blockMan   = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
            SoundMan   = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            handObj    = handTra.gameObject;
            handIma    = handObj.GetComponent<Image>();

            //�t�B���^�[�擾
            int filterCount = filterBoxTra.childCount;
            filterObj       = new GameObject[filterCount];
            fadeFilter      = new Image[filterCount][];
            for (int filterInd = 0; filterInd < filterCount; filterInd++)
            {
                filterObj[filterInd]   = filterBoxTra.GetChild(filterInd).gameObject;
                Transform filterObjTra = filterObj[filterInd].transform;
                int filterCount_1      = filterObjTra.childCount;
                fadeFilter[filterInd]  = new Image[filterCount_1];
                for (int filterInd_1 = 0; filterInd_1 < filterCount_1; filterInd_1++)
                { fadeFilter[filterInd][filterInd_1] = filterObjTra.GetChild(filterInd_1).GetComponent<Image>(); }
            }
            fullFilterIma = filterBoxTra.GetChild(filterCount - 1).gameObject.GetComponent<Image>();

            //�e�L�X�gText�擾
            int textCount  = textBoxTra.childCount;
            textObj        = new GameObject[textCount];
            fadeTextFilter = new Image[textCount];
            for (int textInd = 0; textInd < textCount; textInd++)
            {
                textObj[textInd] = textBoxTra.GetChild(textInd).gameObject;
                fadeTextFilter[textInd] = textObj[textInd].transform.Find("TextFilter").GetComponent<Image>();
            }

            //�����J�n
            StartCoroutine(Description());
        }

        //========================================================================
        //�^�b�v��҂�
        //========================================================================
        IEnumerator WaitTap()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.02f);
                if (Input.GetMouseButtonDown(0))
                {
                    NextDescriptionStart();
                    break;
                }
            }
        }

        //========================================================================
        //��������
        //========================================================================
        public void NextDescriptionStart()
        {
            descriptionNum++;
            StartCoroutine(Description());
        }

        //========================================================================
        //������蒼��
        //========================================================================
        public IEnumerator RedoThrow()
        {
            throwWait = false;             //�����\�t���Ofalse
            rethrow   = true;              //������蒼���t���Otrue
            descriptionNum--;              //�����ԍ��߂�
            if (descriptionNum == 2) StartCoroutine(FilterHide(1));  //�t�B���^�[��\��[1](�p�v���J����)
            else StartCoroutine(FilterHide(4));                      //�t�B���^�[��\��[4](�L���x�c����)
            yield return StartCoroutine(ImagePaletteChange(fullFilterIma, imaFadeSpeed, blackout, alphaFadeComp, 1)); //�Ó]
            StartCoroutine(ImagePaletteChange(fullFilterIma, imaFadeSpeed, lightUp, alphaFadeComp, 1));               //���]

            if (descriptionNum == 2) TextShow(19);            //�e�L�X�g[19]�\��(�p�v���J����)
            else TextShow(20);                                //�e�L�X�g[20]�\��(�L���x�c����)
            yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
            StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
        }
        //========================================================================
        //�Ó]���ԎZ�o
        //========================================================================
        public float GetBlackOutTime()
        {
            return GetImagePaletteChangeTime(fullFilterIma, imaFadeSpeed, blackout, alphaFadeComp, 1);
        }

        //========================================================================
        //����
        //========================================================================
        IEnumerator Description()
        {
            switch (descriptionNum)
            {
                case 0:  //�V���[�g���[�h�ւ悤�����`
                    SoundMan.YesTapSE();
                    TextShow(0);                                      //�e�L�X�g[0]�\��
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 1:  //��ʂ̏�̕��ɖ�؃u���b�N�������邩�ȁ`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(0));         //�e�L�X�g��\��[0]
                    TextShow(1);                                      //�e�L�X�g[1]�\��
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(0, true);                              //�t�B���^�[[0]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 2:  //�܂��̓I���t������^�b�v���ā`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(1));         //�e�L�X�g��\��[1]
                    TextShow(2);                                      //�e�L�X�g[2]�\��
                    StartCoroutine(FilterHide(0));                    //�t�B���^�[��\��[0]
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 3:  //�p�v���J�����ҋ@
                    SoundMan.YesTapSE();

                    if (rethrow)
                    {
                        yield return StartCoroutine(TextHide(19));    //�e�L�X�g��\��[19](������蒼����)
                        rethrow = false;                              //������蒼���t���Ofalse
                    }
                    else yield return StartCoroutine(TextHide(2));    //�e�L�X�g��\��[2](�ʏ펞)

                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(1, true);                              //�t�B���^�[[1]�\��
                    StartCoroutine(HandMove(0));                      //��\��[0]
                    throwWait = true;                                 //�����\�t���Otrue
                    break;

                case 4:  //�p�v���J���Ȃ������ˁ`
                    SoundMan.YesTapSE();
                    throwWait = false;                                        //�����\�t���Ofalse
                    HandHide();                                               //���\��
                    StartCoroutine(FilterHide(1));                            //�t�B���^�[��\��[1]
                    FilterShow(2, true);                                      //�t�B���^�[[2]�\��
                    TextShow(3);                                              //�e�L�X�g[3]�\��
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                                //�^�b�v�ҋ@
                    break;

                case 5:  //���n�ҋ@
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(3));         //�e�L�X�g��\��[3]
                    Time.timeScale = 1;                               //���Ԃ�߂�
                    StartCoroutine(FilterHide(2));                    //�t�B���^�[��\��[2]
                    break;

                case 6:  //���n������؂́`
                    SoundMan.YesTapSE();
                    TextShow(4);                                      //�e�L�X�g[4]�\��
                    FilterShow(3, true);                              //�t�B���^�[[3]�\��
                    StartCoroutine(HandMove(1));                      //��\��[1]
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 7:  //��ʏ㕔�Ł`
                    SoundMan.YesTapSE();
                    HandHide();                                       //���\��
                    yield return StartCoroutine(TextHide(4));         //�e�L�X�g��\��[4]
                    TextShow(5);                                      //�e�L�X�g[5]�\��
                    StartCoroutine(HandMove(2));                      //��\��[2]
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 8:  //���ɁA���̑��̎��n���@���`
                    SoundMan.YesTapSE();
                    HandHide();                                       //���\��
                    yield return StartCoroutine(TextHide(5));         //�e�L�X�g��\��[5]
                    TextShow(6);                                      //�e�L�X�g[6]�\��
                    StartCoroutine(FilterHide(3));                    //�t�B���^�[��\��[3]
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 9:  //�L���x�c�����ҋ@
                    SoundMan.YesTapSE();

                    if (rethrow)
                    {
                        yield return StartCoroutine(TextHide(20));    //�e�L�X�g��\��[20](������蒼����)
                        rethrow = false;                              //������蒼���t���Ofalse
                    }
                    else yield return StartCoroutine(TextHide(6));    //�e�L�X�g��\��[6](�ʏ펞)

                    throwWait = true;                                 //�����\�t���Otrue
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(4, true);                              //�t�B���^�[[4]�\��
                    StartCoroutine(HandMove(3));                      //��\��[3]
                    break;

                case 10:  //�L���x�c�����n�������`
                    SoundMan.YesTapSE();
                    throwWait = false;                                //�����\�t���Ofalse
                    HandHide();                                       //���\��
                    TextShow(7);                                      //�e�L�X�g[7]�\��
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 11:  //��ʉE���̃I���t���񂪁`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(7));         //�e�L�X�g��\��[7]
                    TextShow(8);                                      //�e�L�X�g[8]�\��
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(5, true);                              //�t�B���^�[[5]�\��
                    StartCoroutine(HandMove(4));                      //��\��[4]
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    specialHamsterWait = true;                        //�X�y�V�����I���t�ҋ@�t���Otrue
                    break;

                case 12:  //�X�y�V�����I���t�ҋ@
                    SoundMan.YesTapSE();
                    specialHamsterWait = false;                       //�X�y�V�����I���t�ҋ@�t���Ofalse
                    yield return StartCoroutine(TextHide(8));         //�e�L�X�g��\��[8]
                    StartCoroutine(FilterHide(5));                    //�t�B���^�[��\��[5]
                    break;

                case 13:  //�X�y�V�����I���t����́`
                    SoundMan.YesTapSE();
                    TextShow(9);                                      //�e�L�X�g[9]�\��
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 14:  //�����ƁA���Ɏ����Ă���u���b�N�́`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(9));         //�e�L�X�g��\��[9]
                    TextShow(10);                                     //�e�L�X�g[10]�\��
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(6, true);                              //�t�B���^�[[6]�\��
                    StartCoroutine(HandMove(5));                      //��\��[5]
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    throwBlockChangeWait = true;                      //next�u���b�N�^�b�v�ҋ@�t���Otrue
                    break;

                case 15:  //�莝���̃u���b�N������ւ�����ˁ`
                    SoundMan.YesTapSE();
                    throwBlockChangeWait = false;                     //next�u���b�N�^�b�v�ҋ@�t���Ofalse
                    HandHide();                                       //���\��
                    yield return StartCoroutine(TextHide(10));        //�e�L�X�g��\��[10]
                    TextShow(11);                                     //�e�L�X�g[11]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 16:  //���Ƀt�B�[�o�[�^�C���ɂ��ā`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(11));        //�e�L�X�g��\��[11]
                    TextShow(12);                                     //�e�L�X�g[12]�\��
                    StartCoroutine(FilterHide(6));                    //�t�B���^�[��\��[6]
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 17:  //�u���b�R���[�����ҋ@
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(12));        //�e�L�X�g��\��[12]
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    StartCoroutine(HandMove(6));                      //��\��[6]
                    throwWait = true;                                 //�����\�t���Otrue
                    break;

                case 18:  //�u���b�N�����ׂĎ��n�o�����ˁ`
                    SoundMan.YesTapSE();
                    Time.timeScale = 0;                                       //���Ԓ�~
                    throwWait = false;                                        //�����\�t���Ofalse
                    HandHide();                                               //���\��
                    TextShow(13);                                             //�e�L�X�g[13]�\��
                    FullFilterSwitch(true);                                   //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                                //�^�b�v�ҋ@
                    break;

                case 19:  //�t�B�[�o�[�J�n���o�ҋ@
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(13));        //�e�L�X�g��\��[13]
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    Time.timeScale = 1;                               //���Ԃ�߂�
                    break;

                case 20:  //�t�B�[�o�[�^�C���́`
                    SoundMan.YesTapSE();
                    Time.timeScale = 0;                                       //���Ԓ�~
                    TextShow(14);                                             //�e�L�X�g[14]�\��
                    FullFilterSwitch(true);                                   //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                                //�^�b�v�ҋ@
                    break;

                case 21:  //�n�ʂɗ������u���b�N�͎��n�͂ł��Ȃ���`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(14));                //�e�L�X�g��\��[14]
                    TextShow(15);                                             //�e�L�X�g[15]�\��
                    yield return new WaitForSecondsRealtime(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                                //�^�b�v�ҋ@
                    break;

                case 22:  //�t�B�[�o�[�ҋ@
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(15));        //�e�L�X�g��\��[15]
                    Time.timeScale = 1;                               //���Ԃ�߂�
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    StartCoroutine(HandMove(7));                      //��\��[7]
                    break;

                case 23:  //���܂��L���b�`�ł������ȁ`
                    SoundMan.YesTapSE();
                    HandHide();                                       //���\��
                    TextShow(16);                                     //�e�L�X�g[16]�\��
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 24:  //���̃��C����艺�Ƀu���b�N��������`
                    SoundMan.YesTapSE();
                    yield return StartCoroutine(TextHide(16));        //�e�L�X�g��\��[16]
                    TextShow(17);                                     //�e�L�X�g[17]�\��
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(7, true);                              //�t�B���^�[[7]�\��
                    gameOverLineIma.color = lineColor[0];             //�Q�[���I�[�o�[���C���͂�����
                    StartCoroutine(HandMove(8));                      //��\��[8]
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 25:  //������@�͕����������ȁ`
                    SoundMan.YesTapSE();
                    gameOverLineIma.color = lineColor[1];             //�Q�[���I�[�o�[���C���߂�
                    HandHide();                                       //���\��
                    yield return StartCoroutine(TextHide(17));        //�e�L�X�g��\��[17]
                    TextShow(18);                                     //�e�L�X�g[18]�\��
                    StartCoroutine(FilterHide(7));                    //�t�B���^�[��\��[7]
                    FullFilterSwitch(true);                           //�S�ʃt�B���^�[�t�F�[�h�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 26:  //�����N���A
                    yield return StartCoroutine(TextHide(18));        //�e�L�X�g��\��[18]
                    FullFilterSwitch(false);                          //�S�ʃt�B���^�[�t�F�[�h��\��
                    GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>().ForcedClear();
                    break;
            }
        }

        //========================================================================
        //�e�L�X�g�\��
        //========================================================================
        //textIndex;  �\������e�L�X�g�ԍ�
        //========================================================================
        void TextShow(int textIndex)
        {
            //�e�L�X�g����
            textObj[textIndex].SetActive(true);

            //�e�L�X�g�t�F�[�h(�Ώ�Image, �Ώ�Text, �ύX���x, �ύX�F�̔z��(0:���݂̐F), ��r�ԍ�(0:R 1:G 2:B 3:A), �����)
            StartCoroutine(ImagePaletteChange(fadeTextFilter[textIndex], imaFadeSpeed, transparent, alphaFadeComp, 1));
        }

        //========================================================================
        //�e�L�X�g��\��
        //========================================================================
        //textIndex;  ��\���ɂ���e�L�X�g�ԍ�
        //========================================================================
        IEnumerator TextHide(int textIndex)
        {
            yield return StartCoroutine(ImagePaletteChange(fadeTextFilter[textIndex], imaFadeSpeed, appearance, alphaFadeComp, 1));
            textObj[textIndex].SetActive(false);
        }

        //========================================================================
        //�t�B���^�[�\��
        //========================================================================
        //filterIndex;  �\������t�B���^�[�ԍ�
        //fade;         �t�F�[�h����H
        //========================================================================
        void FilterShow(int filterIndex, bool fade)
        {
            //�e�L�X�g����
            filterObj[filterIndex].SetActive(true);

            foreach (Image filter in fadeFilter[filterIndex])
            {
                if (fade) StartCoroutine(ImagePaletteChange(filter, imaFadeSpeed, new Color[] { filterColor[1], filterColor[0] }, alphaFadeComp, 1));
                else filter.color = filterColor[0];
            }
        }

        //========================================================================
        //�t�B���^�[��\��
        //========================================================================
        //filterIndex;  ��\���ɂ���t�B���^�[�ԍ�
        //========================================================================
        public IEnumerator FilterHide(int filterIndex)
        {
            int filterCount = fadeFilter[filterIndex].Length;
            foreach (Image filter in fadeFilter[filterIndex])
            { StartCoroutine(ImagePaletteChange(filter, imaFadeSpeed, filterColor, alphaFadeComp, 1)); }

            float waitTime = GetImagePaletteChangeTime(fadeFilter[filterIndex][filterCount - 1], imaFadeSpeed, filterColor, alphaFadeComp, 1);
            yield return new WaitForSecondsRealtime(waitTime);
            filterObj[filterIndex].SetActive(false);
        }

        //========================================================================
        //�S�ʃt�B���^�[�̕\���؂�ւ�
        //========================================================================
        //display;  �\���H
        //========================================================================
        void FullFilterSwitch(bool display)
        {
            //�e�L�X�g�t�F�[�h(�Ώ�Image, �Ώ�Text, �ύX���x, �ύX�F�̔z��(0:���݂̐F), ��r�ԍ�(0:R 1:G 2:B 3:A), �����)
            Color[] ColArray = (display) ? new Color[] { filterColor[1], filterColor[0] } : filterColor;
            StartCoroutine(ImagePaletteChange(fullFilterIma, imaFadeSpeed, ColArray, alphaFadeComp, 1));
        }


        //========================================================================
        //��𓮂���
        //========================================================================
        //moveNum; ����ԍ�
        //========================================================================
        IEnumerator HandMove(int moveNum)
        {
            //�葼���쒆�͏I���܂őҋ@
            yield return new WaitWhile(() => handNowMove == true);

            //��̕\��
            handObj.SetActive(true);
            Vector2[] handPos = new Vector2[2];
            float pointFingerSpeed = 1.0f;
            switch (moveNum)
            {
                case 0:  //�p�v���J����
                case 3:  //�L���x�c����
                case 6:  //�u���b�R���[����
                    StartCoroutine(ThrowingInstructions()); //�����w������
                    break;

                case 1:  //�X�R�A�w����
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //�p�x�ݒ�
                    handPos[0] = new Vector2(-230.0f, -560.0f);                //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(-230.0f, -540.0f);                //�w��������ܕԂ����W�ݒ�
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //�w��������
                    break;

                case 2:  //�^�[�Q�b�g�w����
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);     //�p�x�w��
                    handPos[0] = new Vector2(20.0f, 590.0f);                   //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(20.0f, 570.0f);                   //�w��������ܕԂ����W�ݒ�
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //�����w������
                    break;

                case 4:  //�X�y�V�����I���t�w����
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //�p�x�w��
                    handPos[0] = new Vector2(400.0f, -630.0f);                 //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(400.0f, -610.0f);                 //�w��������ܕԂ����W�ݒ�
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //�����w������
                    break;

                case 5:  //next�u���b�N�w����
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);   //�p�x�w��
                    handPos[0] = new Vector2(295.0f, -550.0f);                 //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(295.0f, -530.0f);                 //�w��������ܕԂ����W�ݒ�
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //�����w������
                    break;

                case 7:  //�t�B�[�o�[���X���C�v
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);                  //�p�x�w��
                    handPos[0] = new Vector2(-400.0f, -650.0f);                             //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(420.0f, -650.0f);                              //�w��������ܕԂ����W�ݒ�
                    handTra.anchoredPosition = new Vector2(20.0f, -650.0f);                 //��ʒ�����
                    handIma.color   = appearance[1];                                        //alpha�l�߂�
                    float moveSpeed = 7.0f;                                                 //��̈ړ����x
                    float moveTime  = GetMoveTime(handTra, moveSpeed, 1.0f, handPos[0]);    //���쎞�Ԏ擾
                    StartCoroutine(MoveMovement(handTra, moveSpeed, 1.0f, handPos[0]));     //�������獶��
                    yield return new WaitForSeconds(moveTime);                              //����ҋ@
                    StartCoroutine(HandRoundTrip(handPos, moveSpeed));                      //�����w������
                    break;

                case 8:  //�Q�[���I�[�o�[���C���w����
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);     //�p�x�w��
                    handPos[0] = new Vector2(400.0f, -550.0f);                 //�w��������J�n���W�ݒ�
                    handPos[1] = new Vector2(400.0f, -570.0f);                 //�w��������ܕԂ����W�ݒ�
                    StartCoroutine(HandRoundTrip(handPos, pointFingerSpeed));  //�����w������
                    break;
            }
        }

        //========================================================================
        //�蓊���w������
        //========================================================================
        IEnumerator ThrowingInstructions()
        {
            //�e�l�w��
            Vector2[] handPos = new Vector2[] { new Vector2(20.0f, -600.0f), new Vector2(20.0f, -840.0f) };
            float handMoveSpeed = 7.0f;
            Vector3[] handScalingSpeed = new Vector3[] { new Vector3(0.02f, 0.02f, 0.0f), new Vector3(-0.02f, -0.02f, 0.0f) };
            float[] handSize = new float[] { 1.0f, 1.2f };

            //�����l�ݒ�
            handTra.anchoredPosition = handPos[0];
            handTra.localScale       = new Vector3(handSize[1], handSize[1], handSize[1]);
            handTra.rotation         = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            handIma.color            = appearance[0];

            //���쎞�Ԍv�Z
            float moveTime  = GetMoveTime(handTra, handMoveSpeed, 1.0f, handPos[1]);
            float scaleTime = GetScaleChangeTime(handTra, handScalingSpeed[1], handSize[0], handSize[0], 1);
            float stopTime  = 0.2f;
            float interval  = 1.0f;

            //����J�n
            handNowMove = true;
            handShow    = true;
            while (handShow)
            {
                handTra.anchoredPosition = handPos[0];                                                      //����W�w��
                StartCoroutine(ImagePaletteChange(handIma, imaFadeSpeed, appearance, alphaFadeComp, 1));    //�o��
                StartCoroutine(ScaleChange(handTra, handScalingSpeed[1], handSize[0], handSize[0], 1));     //�k��
                yield return new WaitForSeconds(scaleTime);                                                 //�k���ҋ@
                yield return new WaitForSeconds(stopTime);                                                  //��U��~
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[1]));                     //���Ɉړ�
                yield return new WaitForSeconds(moveTime);                                                  //�ړ��ҋ@
                yield return new WaitForSeconds(stopTime);                                                  //��U��~
                StartCoroutine(ImagePaletteChange(handIma, imaFadeSpeed, transparent, alphaFadeComp, 1));   //����
                StartCoroutine(ScaleChange(handTra, handScalingSpeed[0], handSize[1], handSize[1], 1));     //�g��
                yield return new WaitForSeconds(scaleTime);                                                 //�g��ҋ@
                yield return new WaitForSeconds(interval);                                                  //�C���^�[�o��
            }

            //����I��
            handObj.SetActive(false);
            handIma.color = appearance[1];
            handTra.localScale = Vector3.one;
            handNowMove = false;
        }

        //========================================================================
        //��������
        //========================================================================
        //handPos;       ��̍��W 1:�J�n���W 2:�I�����W
        //handMoveSpeed; ��̈ړ����x
        //========================================================================
        IEnumerator HandRoundTrip(Vector2[] handPos, float handMoveSpeed)
        {
            //�����l�ݒ�
            handTra.anchoredPosition = handPos[0];
            handIma.color = appearance[1];
            float moveTime = GetMoveTime(handTra, handMoveSpeed, 1.0f, handPos[1]);

            //����J�n
            handNowMove = true;
            handShow    = true;
            while (handShow)
            {
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[1])); //�ڕW���W��
                yield return new WaitForSeconds(moveTime);
                StartCoroutine(MoveMovement(handTra, handMoveSpeed, 1.0f, handPos[0])); //�����W��
                yield return new WaitForSeconds(moveTime);
            }

            //����I��
            handObj.SetActive(false);
            handTra.localScale = Vector3.one;
            handNowMove = false;
        }

        //========================================================================
        //���\��(����I��)
        //========================================================================
        public void HandHide()
        {
            handShow = false;
            handObj.SetActive(false);
        }
    }
}