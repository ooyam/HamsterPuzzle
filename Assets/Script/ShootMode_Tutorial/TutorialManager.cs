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

        HamsterController_Tutorial hamsterCon;   //HamsterController_Tutorial
        BlockManager_Tutorial blockMan;          //BlockManager_Tutorial
        SoundManager SoundMan;                   //SoundManager

        GameObject[] filterObj;       //�t�B���^�[Obj
        Image fullFilterIma;          //�S�ʃt�B���^�[gImae
        Image[][] fadeFilter;         //�t�F�[�hImage
        int displayFilterIndex;       //�\������Image�ԍ�
        GameObject[] textObj;         //�e�L�X�gObj
        Image[] fadeTextFilter;       //�t�F�[�hText�p�t�B���^�[
        int displayTextIndex;         //�\������Text�ԍ�

        [System.NonSerialized]
        public int descriptionNum = 0;  //�����ԍ�
        float minDispalyTime   = 3.0f;  //�����Œ�\������
        float alphaChangeSpeed = 0.2f;  //�t�F�[�h���x
        float destroyWaitTime  = 0.3f;  //�t�F�[�h�A�E�g�ҋ@����
        Color[] handColor   = new Color[] { Color.white, new Color(1, 1, 1, 0) };               //�� �� ����
        Color[] textColor   = new Color[] { Color.white, new Color(1, 1, 1, 0) };               //�� �� ����
        Color[] filterColor = new Color[] { new Color(0, 0, 0, 160.0f / 255.0f), Color.clear }; //�������� �� ����
        int[] alphaFadeComp = new int[] { 3, 3 };  //��r�ԍ��w��z��(0:R 1:G 2:B 3:A)

        //��̈ʒu 0�1:�I���t�ړ� 2:�̗̓Q�[�W 3:�^�[�� 4:�ڕW
        Vector2[] handStartPos =
            new Vector2[] {
            new Vector2(0.0f, -30.0f),
            new Vector2(0.0f, -200.0f),
            new Vector2(100.0f, -600.0f),
            new Vector2(475.0f, 580.0f),
            new Vector2(400.0f, 770.0f)
            };
        Vector2[] handEndPos =
            new Vector2[] {
            new Vector2(0.0f, -200.0f),
            new Vector2(-360.0f, -200.0f),
            new Vector2(100.0f, -650.0f),
            new Vector2(475.0f, 530.0f),
            new Vector2(350.0f, 770.0f)
            };

        void Start()
        {
            //Time.timeScale = 1.0f;
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

            StartCoroutine(Description());
        }

        //========================================================================
        //�^�b�v��҂�
        //========================================================================
        IEnumerator WaitTap()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (Input.GetMouseButtonDown(0))
                {
                    descriptionNum++;
                    StartCoroutine(Description());
                    break;
                }
            }
        }

        //========================================================================
        //����
        //========================================================================
        IEnumerator Description()
        {
            switch (descriptionNum)
            {
                case 0:
                    //�V���[�g���[�h�ւ悤�����`
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(0);                                      //�e�L�X�g[0]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 1:
                    //��ʂ̏�̕��ɖ�؃u���b�N�������邩�ȁ`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(0, true);                              //�t�B���^�[[0]�\��
                    TextShow(1);                                      //�e�L�X�g[1]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 2:
                    //�܂��̓I���t������^�b�v���ā`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    TextShow(2);                                      //�e�L�X�g[2]�\��
                    FilterShow(1, true);                              //�t�B���^�[[1]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 3:
                    //�p�v���J�����ҋ@
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 4:
                    //�p�v���J���Ȃ������ˁ`
                    SoundMan.YesTapSE();
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FilterShow(2, true);                              //�t�B���^�[[2]�\��
                    TextShow(3);                                      //�e�L�X�g[3]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 5:
                    //���n�ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 6:
                    //���n������؂́`
                    SoundMan.YesTapSE();
                    FilterShow(3, true);                              //�t�B���^�[[3]�\��
                    TextShow(4);                                      //�e�L�X�g[4]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 7:
                    //���n������؂́`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    TextShow(5);                                      //�e�L�X�g[5]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 8:
                    //���ɁA���̑��̎��n���@���`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(6);                                      //�e�L�X�g[6]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 9:
                    //�L���x�c�����ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(4, true);                              //�t�B���^�[[4]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 10:
                    //�L���x�c�����n�������`
                    SoundMan.YesTapSE();
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(7);                                      //�e�L�X�g[7]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 11:
                    //��ʉE���̃I���t���񂪁`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(5, true);                              //�t�B���^�[[5]�\��
                    TextShow(8);                                      //�e�L�X�g[8]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 12:
                    //�X�y�V�����I���t�ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 13:
                    //�X�y�V�����I���t����́`
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(9);                                      //�e�L�X�g[9]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 14:
                    //�����ƁA���Ɏ����Ă���u���b�N�́`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(6, true);                              //�t�B���^�[[6]�\��
                    TextShow(10);                                     //�e�L�X�g[10]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 15:
                    //�莝���̃u���b�N������ւ�����ˁ`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    TextShow(11);                                     //�e�L�X�g[11]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 16:
                    //���Ƀt�B�[�o�[�^�C���ɂ��ā`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(12);                                     //�e�L�X�g[12]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 17:
                    //�u���b�R���[�����ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 18:
                    //�u���b�N�����ׂĎ��n�o�����ˁ`
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(13);                                     //�e�L�X�g[13]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 19:
                    //�t�B�[�o�[�J�n���o�ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 20:
                    //�t�B�[�o�[�^�C���́`
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(14);                                     //�e�L�X�g[14]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 21:
                    //�n�ʂɗ������u���b�N�͎��n�͂ł��Ȃ���`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    TextShow(15);                                     //�e�L�X�g[15]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 22:
                    //�t�B�[�o�[�ҋ@
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 23:
                    //���܂��L���b�`�ł������ȁ`
                    SoundMan.YesTapSE();
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(16);                                     //�e�L�X�g[16]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 24:
                    //���̃��C����艺�Ƀu���b�N��������`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    FullFilterSwitch(true, false);                    //�S�ʃt�B���^�[�t�F�[�h��\��
                    FilterShow(7, true);                              //�t�B���^�[[7]�\��
                    TextShow(17);                                     //�e�L�X�g[17]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;

                case 25:
                    //������@�͕����������ȁ`
                    SoundMan.YesTapSE();
                    StartCoroutine(TextHide());                       //�e�L�X�g��\��
                    yield return new WaitForSeconds(destroyWaitTime); //�e�L�X�g��\���ҋ@
                    StartCoroutine(FilterHide());                     //�t�B���^�[��\��
                    FullFilterSwitch(true, true);                     //�S�ʃt�B���^�[�t�F�[�h�\��
                    TextShow(18);                                     //�e�L�X�g[18]�\��
                    yield return new WaitForSeconds(minDispalyTime);  //�Œ�\�����ԑҋ@
                    StartCoroutine(WaitTap());                        //�^�b�v�ҋ@
                    break;
            }
        }

        //========================================================================
        //�e�L�X�g�\��
        //========================================================================
        //textIndex;  ��������e�L�X�g�ԍ�
        //========================================================================
        void TextShow(int textIndex)
        {
            //�e�L�X�g����
            textObj[textIndex].SetActive(true);
            displayTextIndex = textIndex;

            //�e�L�X�g�t�F�[�h(�Ώ�Image, �Ώ�Text, �ύX���x, �ύX�F�̔z��(0:���݂̐F), ��r�ԍ�(0:R 1:G 2:B 3:A), �����)
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, alphaChangeSpeed, textColor, alphaFadeComp, 1));
        }

        //========================================================================
        //�e�L�X�g��\��
        //========================================================================
        IEnumerator TextHide()
        {
            StartCoroutine(PaletteChange(fadeTextFilter[displayTextIndex], null, alphaChangeSpeed, new Color[] { textColor[1], textColor[0] }, alphaFadeComp, 1));
            yield return new WaitForSeconds(destroyWaitTime);
            textObj[displayTextIndex].SetActive(false);
        }

        //========================================================================
        //�t�B���^�[����
        //========================================================================
        //filterIndex;  ��������t�B���^�[�ԍ�
        //fade;         �t�F�[�h����H
        //========================================================================
        void FilterShow(int filterIndex, bool fade)
        {
            //�e�L�X�g����
            filterObj[filterIndex].SetActive(true);
            displayFilterIndex = filterIndex;

            foreach (Image filter in fadeFilter[filterIndex])
            {
                if (fade) StartCoroutine(PaletteChange(filter, null, alphaChangeSpeed, new Color[] { filterColor[1], filterColor[0] }, alphaFadeComp, 1));
                else filter.color = filterColor[0];
            }
        }

        //========================================================================
        //�t�B���^�[��\��
        //========================================================================
        IEnumerator FilterHide()
        {
            foreach (Image filter in fadeFilter[displayFilterIndex])
            {
                StartCoroutine(PaletteChange(filter, null, 0.02f, filterColor, alphaFadeComp, 1));
            }
            yield return new WaitForSeconds(destroyWaitTime);
            filterObj[displayFilterIndex].SetActive(false);
        }

        //========================================================================
        //�S�ʃt�B���^�[�̃t�F�[�h
        //========================================================================
        //fade;     �t�F�[�h����H
        //display;  �\���H
        //========================================================================
        void FullFilterSwitch(bool fade, bool display)
        {
            if (fade)
            {
                //�e�L�X�g�t�F�[�h(�Ώ�Image, �Ώ�Text, �ύX���x, �ύX�F�̔z��(0:���݂̐F), ��r�ԍ�(0:R 1:G 2:B 3:A), �����)
                Color[] ColArray = (display) ? new Color[] { filterColor[1], filterColor[0] } : filterColor;
                StartCoroutine(PaletteChange(fullFilterIma, null, alphaChangeSpeed, ColArray, alphaFadeComp, 1));
            }
            else
            {
                fullFilterIma.color = (display) ? filterColor[0] : filterColor[1];
            }
        }
    }
}