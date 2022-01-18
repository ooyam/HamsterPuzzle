using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundFunction;
using GFramework; //SimpleRoundedImage���g�p����
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [Header("�t�B���^�[�擾")]
        public GameObject[] filter;
        [Header("�t�B���^�[�O�i�[�{�b�N�X")]
        public GameObject frontBoxObj;
        [System.NonSerialized]
        public Transform frontBoxTra;
        [Header("�e�L�X�g�{�b�N�X")]
        public GameObject[] textBox;
        GameObject textObj;
        Transform textTra;
        Text textText;
        Image textIma;
        SimpleRoundedImage hintIma;
        [Header("�e�L�X�g�i�[�{�b�N�X")]
        public Transform textBoxTra;
        [Header("�X�e�[�^�X�{�[�h")]
        public Transform statusBoardTra;
        [Header("��擾")]
        public GameObject hand;
        RectTransform handTra;
        Image handIma;
        [Header("�J�����[�Q�[�W")]
        public Transform calGaugeTra;
        [Header("�^�[���\��")]
        public Transform turnTra;
        [Header("�^�[�Q�b�g�\��")]
        public Transform targetTra;
        [Header("���n���X�g")]
        public Transform harvestTra;

        HamsterPanelController HamsterCon;   //�n���X�^�[�p�l���X�N���v�g
        PanelManager PanelMan;               //PanelManger
        SoundManager SoundMan;               //SoundManager
        [System.NonSerialized]
        public int tupNum = 0;               //�^�b�v��
        bool waiting = false;        //�ҋ@���H
        bool description = false;    //�������H
        float displayTime = 2.0f;    //�����̍Œ�\������
        [System.NonSerialized]
        public bool ColDescription = false;  //�̗͐��������H
        bool imageFade = false;      //�C���[�W���t�F�[�h����H
        bool textIndexTen = false;   //�e�L�X�g�ԍ�10�H
        float textTenColor = 180.0f / 255.0f;  //�e�L�X�g�ԍ�10�̐F��G�l

        bool textDestroy = false;    //�e�L�X�g�������H
        bool textDisplay = false;    //�e�L�X�g�\���r���H
        float[] textColAlpha = new float[] { 1.0f, 0.0f };   //�e�L�X�g�̃A���t�@�l

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
        Color[] handColor = new Color[] { new Color(1, 1, 1, 1), new Color(1, 1, 1, 0) };

        void Start()
        {
            Time.timeScale = 1.0f;
            PanelMan = GameObject.FindWithTag("PanelManager").GetComponent<PanelManager>();
            SoundMan = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            frontBoxTra = frontBoxObj.transform;
            handTra = hand.GetComponent<RectTransform>();
            handIma = hand.GetComponent<Image>();
            TextDisplay(0);
            frontBoxObj.SetActive(true);
            StartCoroutine(DescriptionStart());
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!description && !waiting && !textDestroy)
                {
                    tupNum++;
                    StartCoroutine(NextDescription(tupNum));
                }
            }
        }

        //========================================================================
        //���̐���
        //========================================================================
        //descriptionNum;   �����ԍ�
        //========================================================================
        IEnumerator NextDescription(int descriptionNum)
        {
            COLOR_CHANGE_INFINITE_END = false;
            switch (descriptionNum)
            {
                case 1:
                    SoundMan.YesTapSE();
                    frontBoxObj.SetActive(false);
                    FilterDisplay(0);
                    StartCoroutine(TextDestroy(true));
                    yield return new WaitWhile(() => textDestroy == true);
                    StartCoroutine(DescriptionStart());
                    hand.SetActive(true);
                    StartCoroutine(HandMove(0));
                    HamsterCon.description = true;
                    TextDisplay(1);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 2:
                case 6:
                    SoundMan.YesTapSE();
                    HamsterCon.description = false;
                    hand.SetActive(false);
                    description = true;
                    break;
                case 3:
                    SoundMan.YesTapSE();
                    TimeScaleChange(1.0f);
                    FilterDestroy(1);
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    StartCoroutine(DescriptionStart());
                    description = true;
                    break;
                case 4:
                    SoundMan.YesTapSE();
                    calGaugeTra.SetParent(statusBoardTra, true);
                    turnTra.SetParent(frontBoxTra, true);
                    StartCoroutine(HandMove(3));
                    handTra.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    StartCoroutine(DescriptionStart());
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    FilterDestroy(1);
                    TextDisplay(4);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 5:
                    SoundMan.YesTapSE();
                    turnTra.SetParent(statusBoardTra, true);
                    hand.SetActive(true);
                    StartCoroutine(HandMove(1));
                    HamsterCon.MovingLimit(false);
                    HamsterCon.description = true;
                    frontBoxObj.SetActive(false);
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    StartCoroutine(DescriptionStart());
                    FilterDisplay(2);
                    TextDisplay(5);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 7:
                    SoundMan.YesTapSE();
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    StartCoroutine(DescriptionStart());
                    TextDisplay(7);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 8:
                    SoundMan.YesTapSE();
                    TimeScaleChange(1.0f);
                    FilterDestroy(3);
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    StartCoroutine(DescriptionStart());
                    description = true;
                    break;
                case 9:
                    SoundMan.YesTapSE();
                    targetTra.SetParent(statusBoardTra, true);
                    harvestTra.SetParent(statusBoardTra, true);
                    hand.SetActive(false);
                    StartCoroutine(DescriptionStart());
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    TextDisplay(9);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 10:
                    SoundMan.YesTapSE();
                    StartCoroutine(DescriptionStart());
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    TextDisplay(10);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 11:
                    SoundMan.YesTapSE();
                    StartCoroutine(DescriptionStart());
                    StartCoroutine(TextDestroy(false));
                    yield return new WaitWhile(() => textDestroy == true);
                    TextDisplay(11);
                    yield return new WaitWhile(() => textDisplay == true);
                    break;
                case 12:
                    SoundMan.YesTapSE();
                    HamsterCon.tutorial = false;
                    HamsterCon.MovingLimit(false);
                    PanelMan.tutorial = false;
                    this.gameObject.SetActive(false);
                    break;
            }

        }

        //========================================================================
        //��t�F�[�h
        //========================================================================
        //posIndex;   �w����W�ԍ�
        //========================================================================
        IEnumerator HandMove(int posIndex)
        {
            int nowTapNum = tupNum;
            float oneFrameTime = 0.02f;
            bool firstMoveEnd = false;
            float moveSpeed = 0.0f;
            float handColAlpha = 1.0f;
            handIma.color = new Color(1, 1, 1, handColAlpha);
            bool fade = (posIndex <= 1) ? true : false;
            float moveSpeedFix = (fade) ? 1.5f : 1.0f;
            float handPos = 0.0f;
            float comparisonPos = 0.0f;
            Vector2 targetPos = handEndPos[posIndex];
            handTra.anchoredPosition = handStartPos[posIndex];

            while (nowTapNum == tupNum)
            {
                if (!firstMoveEnd || !fade)
                {
                    moveSpeed += oneFrameTime / moveSpeedFix;
                    handTra.anchoredPosition = Vector2.Lerp(handTra.anchoredPosition, targetPos, moveSpeed);
                    switch (posIndex)
                    {
                        case 1:
                        case 4:
                            handPos = handTra.anchoredPosition.x;
                            comparisonPos = targetPos.x;
                            break;
                        default:
                            handPos = handTra.anchoredPosition.y;
                            comparisonPos = targetPos.y;
                            break;
                    }

                    if (!firstMoveEnd && handPos - 1 <= comparisonPos)
                    {
                        firstMoveEnd = true;
                        moveSpeed = 0.0f;
                        if (!fade) targetPos = handStartPos[posIndex];
                    }
                    else if (firstMoveEnd && handPos + 1 >= comparisonPos)
                    {
                        firstMoveEnd = false;
                        moveSpeed = 0.0f;
                        targetPos = handEndPos[posIndex];
                    }
                }
                else
                {
                    handColAlpha -= oneFrameTime * 2.5f;
                    handIma.color = new Color(1, 1, 1, handColAlpha);
                    if (handColAlpha <= 0.0f)
                    {
                        firstMoveEnd = false;
                        handColAlpha = 1.0f;
                        handIma.color = new Color(1, 1, 1, handColAlpha);
                        handTra.anchoredPosition = handStartPos[posIndex];
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
            handIma.color = new Color(1, 1, 1, 1);
        }

        //========================================================================
        //�e�L�X�g�\��
        //========================================================================
        //textIndex;   �e�L�X�g�ԍ�
        //========================================================================
        public void TextDisplay(int textIndex)
        {
            textColAlpha[1] = 0.0f;
            textObj = Instantiate(textBox[textIndex]);
            textTra = textObj.transform;
            textText = textTra.GetChild(0).gameObject.GetComponent<Text>();
            textText.color = new Color(0, 0, 0, 0);
            if (textIndex == 8 || textIndex == 10 || textIndex == 11)
            {
                imageFade = true;
                if (textIndex == 10)
                {
                    textIndexTen = true;
                    hintIma = textTra.GetChild(2).gameObject.GetComponent<SimpleRoundedImage>();
                    hintIma.color = new Color(1, textTenColor, 0, 0);
                }
                textIma = textTra.GetChild(1).gameObject.GetComponent<Image>();
                textIma.color = new Color(1, 1, 1, 0);
            }
            textTra.SetParent(textBoxTra, false);
            textDisplay = true;
            StartCoroutine(TextFade());
        }

        //========================================================================
        //�e�L�X�g����
        //========================================================================
        //firstDes;   ���֐��N���H
        //========================================================================
        public IEnumerator TextDestroy(bool firstDes)
        {
            if (firstDes) HamsterCon = GameObject.FindWithTag("Hamster").GetComponent<HamsterPanelController>();
            textColAlpha[0] = 1.0f;
            textDestroy = true;
            StartCoroutine(TextFade());
            yield return new WaitWhile(() => textDestroy == true);
            if (textObj != null) Destroy(textObj);
        }

        //========================================================================
        //�e�L�X�g�t�F�[�h
        //========================================================================
        IEnumerator TextFade()
        {
            float oneFrameTime = 0.02f;
            while (true)
            {
                if (textDestroy)
                {
                    textColAlpha[0] -= oneFrameTime * 2.5f;
                    if (textObj != null)
                    {
                        textText.color = new Color(0, 0, 0, textColAlpha[0]);
                        if (imageFade) textIma.color = new Color(1, 1, 1, textColAlpha[0]);
                        if (textIndexTen) hintIma.color = new Color(1, textTenColor, 0, textColAlpha[0]);
                    }
                    if (textColAlpha[0] <= 0.0f)
                    {
                        textDestroy = false;
                        break;
                    }
                }
                if (textDisplay)
                {
                    textColAlpha[1] += oneFrameTime * 2.5f;
                    if (textObj != null)
                    {
                        textText.color = new Color(0, 0, 0, textColAlpha[1]);
                        if (imageFade) textIma.color = new Color(1, 1, 1, textColAlpha[1]);
                        if (textIndexTen) hintIma.color = new Color(1, textTenColor, 0, textColAlpha[1]);
                    }
                    if (textColAlpha[1] >= 1.0f)
                    {
                        textDestroy = false;
                        if (imageFade) imageFade = false;
                        if (textIndexTen) textIndexTen = false;
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(oneFrameTime);
            }
        }

        //========================================================================
        //�t�B���^�[�\��
        //========================================================================
        //filterIndex;   �t�B���^�[�ԍ�
        //========================================================================
        public void FilterDisplay(int filterIndex)
        {
            filter[filterIndex].SetActive(true);
        }

        //========================================================================
        //�t�B���^�[����
        //========================================================================
        //filterIndex;   �t�B���^�[�ԍ�
        //========================================================================
        public void FilterDestroy(int filterIndex)
        {
            filter[filterIndex].SetActive(false);
        }

        //========================================================================
        //�����J�n
        //========================================================================
        public IEnumerator DescriptionStart()
        {
            waiting = true;
            yield return new WaitForSecondsRealtime(displayTime);
            waiting = false;
        }

        //========================================================================
        //���ԕύX
        //========================================================================
        //timeScale;   ���Ԃ̎w��
        //========================================================================
        void TimeScaleChange(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        //========================================================================
        //���n����    
        //========================================================================
        public IEnumerator HarvestComplete()
        {
            if (!ColDescription)
            {
                description = false;
                frontBoxObj.SetActive(true);
                hand.SetActive(true);
                handTra.rotation = Quaternion.Euler(180.0f, 0.0f, 0.0f);
                StartCoroutine(HandMove(2));
                StartCoroutine(DescriptionStart());
                calGaugeTra.SetParent(frontBoxTra, true);
                ColDescription = true;
                TextDisplay(3);
                yield return new WaitWhile(() => textDisplay == true);
            }
            else
            {
                description = false;
                frontBoxObj.SetActive(true);
                targetTra.SetParent(frontBoxTra, true);
                harvestTra.SetParent(frontBoxTra, true);
                hand.SetActive(true);
                handTra.rotation = Quaternion.Euler(180.0f, 0.0f, 90.0f);
                StartCoroutine(HandMove(4));
                StartCoroutine(DescriptionStart());
                TextDisplay(8);
                yield return new WaitWhile(() => textDisplay == true);
            }
        }

        //========================================================================
        //�n���X�^�[�ړ�����
        //========================================================================
        //textIndex;   �e�L�X�g�ԍ�
        //filterIndex; �t�B���^�[�ԍ�
        //========================================================================
        public IEnumerator HamsterMovingComplete(int textIndex, int filterIndex)
        {
            description = false;
            StartCoroutine(TextDestroy(false));
            yield return new WaitWhile(() => textDestroy == true);
            StartCoroutine(DescriptionStart());
            TimeScaleChange(0.0f);
            FilterDestroy(filterIndex - 1);
            FilterDisplay(filterIndex);
            TextDisplay(textIndex);
            yield return new WaitWhile(() => textDisplay == true);
        }
    }
}