using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode_Tutorial;
using ShootMode;
using System;
using static ShootMode.ShootModeDefine;

namespace ShootMode_Tutorial
{
    public class HamsterController_Tutorial : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        LineRenderer line;                 //LineRenderer
        Renderer ren;                      //Renderer
        Camera mainCamra;                  //MainCamera
        [System.NonSerialized]
        public Color nowBlockColor;        //�����u���b�N�̐F

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:�ʏ�(�E����) 1:���](������) 2:������������(�E����) 3:������������(�E����)
        [System.NonSerialized]
        public int spriteNum = 0;         //�g�p��sprite�ԍ�

        [Header("BlockBox")]
        [SerializeField]
        RectTransform blockBoxTra;
        Rect blockBoxSize;   //backGround�̕�

        [Header("BlockManager")]
        [SerializeField]
        BlockManager_Tutorial blockMan;

        [Header("�`���[�g���A���}�l�[�W���[")]
        [SerializeField]
        TutorialManager tutorialMan;

        float magnification;               //�^�b�v�ʒu�C���{��
        float differenceX;                 //�^�b�v�ʒu�C����X
        float differenceY;                 //�^�b�v�ʒu�C����Y
        float hamsterPosX;                 //�n���X�^�[X���W
        float posY = -530.0f;              //�n���X�^�[Y���W
        float fastTapPosY;                 //�ŏ��ɐG�����ʒu
        bool  displayLine = false;         //�O�����\�����H
        float throwStartTapPos = -50.0f;   //�����n�߂�ʒu
        float topLimit;                    //�������C��Y���W���
        float[] sideLimit;                 //�������C��X���W���
        float lineStartPosX = 70.0f;       //�������C���̃X�^�[�g�ʒuX
        float lineStartPosY = -20.0f;       //�������C���̃X�^�[�g�ʒuY
        Vector3 lineStartPos;              //�������C���̃X�^�[�g�ʒu
        string[] blockTag;                 //�u���b�N�^�O���X�g

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            line      = GetComponent<LineRenderer>();
            ren       = GetComponent<Renderer>();
            mainCamra = Camera.main;

            differenceY   = CANVAS_HEIGHT / 2.0f;
            differenceX   = CANVAS_WIDTH  / 2.0f;
            magnification = CANVAS_WIDTH  / Screen.width;
            hamsterPosX   = tra.anchoredPosition.x;
            blockBoxSize  = blockBoxTra.rect;
            topLimit      = blockBoxSize.height / 2.0f + blockBoxTra.anchoredPosition.y - posY;
            sideLimit     = new float[] { blockBoxSize.width / 2.0f - hamsterPosX, -blockBoxSize.width / 2.0f - hamsterPosX };

            //�u���b�N�^�O�擾
            System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
            blockTag = new string[vegetableType.Length];
            foreach (VegetableType value in vegetableType)
            { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        }

        //========================================================================
        //�n���X�^�[�^�b�v
        //========================================================================
        public void HumsterButtonDown()
        {
            //�Q�[�����H
            if (GAME_START && !GAME_OVER && !GAME_CLEAR)
            {
                //����̓��쒆�H
                if (!blockMan.throwNow && !blockMan.blockDeleteNow && !blockMan.blockChangeNow && !SPECIAL_HARVEST && !FEVER_START && !SETTING_DISPLAY)
                {
                    if (tutorialMan.throwWait)
                    {
                        //�����w�����\��
                        tutorialMan.HandHide();

                        //sprite�ύX
                        spriteNum  = (spriteNum == 0) ? 2 : 3;
                        ima.sprite = hamsterSprite[spriteNum];

                        //�u���b�N�̈ʒu�ύX
                        tra.SetSiblingIndex(0);
                        blockMan.ThrowBlockPosChange(spriteNum);

                        //���̐F�ύX
                        ren.material.color = nowBlockColor;

                        //��������
                        StartCoroutine(PreparingThrowBlock());
                    }
                }
            }
        }

        //========================================================================
        //�u���b�N��������
        //========================================================================
        IEnumerator PreparingThrowBlock()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                //�^�b�v���W�擾
                Vector3 mousePos = Input.mousePosition;
                mousePos = new Vector3(mousePos.x * magnification - differenceX, (mousePos.y * magnification - differenceY) - posY, 0.0f);
                if (mousePos.y < throwStartTapPos)
                {
                    //�O����������
                    displayLine = true;
                    Vector3[] linePos = LineCalculation(mousePos);
                    DrawLine(linePos);

                    //�u���b�N�𓊂���
                    if (Input.GetMouseButtonUp(0))
                    {
                        line.positionCount = 0;
                        StartCoroutine(blockMan.BlockThrow(linePos));
                        break;
                    }
                }
                else
                {
                    //�O����������
                    if (displayLine)
                    {
                        line.positionCount = 0;
                        displayLine = false;
                    }

                    //��������߂�
                    if (Input.GetMouseButtonUp(0))
                    {
                        //�q�I�u�W�F�N�g�ԍ��ύX(�u���b�N�̑O�ɂł�)
                        tra.SetSiblingIndex(1);
                        blockMan.ThrowBlockPosChange((spriteNum == 2) ? 0 : 1);
                        break;
                    }
                }

                //�Q�[���I����
                if (!GAME_START || GAME_OVER || GAME_CLEAR)
                {
                    //�O����������
                    if (displayLine)
                    {
                        line.positionCount = 0;
                        displayLine = false;
                    }

                    tra.SetSiblingIndex(1);
                    blockMan.ThrowBlockPosChange((spriteNum == 2) ? 0 : 1);
                    break;
                }
            }

            //sprite��߂�
            spriteNum = (spriteNum == 2) ? 0 : 1;
            ima.sprite = hamsterSprite[spriteNum];
        }

        //========================================================================
        //�������C�����W�v�Z
        //========================================================================
        //mousePos; �}�E�X�̈ʒu
        //========================================================================
        Vector3[] LineCalculation(Vector3 mousePos)
        {
            float linePosX   = 0.0f;
            float linePosY   = 0.0f;
            bool  rightThrow = mousePos.x < hamsterPosX;                         //�E�ɓ����n�߂�?
            lineStartPos     = new Vector3(lineStartPosX, lineStartPosY, 0.0f);  //���C�������J�n���W
            float[] maxX     = new float[2];                                     //X�ő�l
            maxX[0]          = sideLimit[0];
            maxX[1]          = sideLimit[1];
            if (!rightThrow)
            {
                lineStartPos = new Vector3(-lineStartPosX, lineStartPosY, 0.0f);
                maxX[0] = sideLimit[1];
                maxX[1] = sideLimit[0];
            }
            linePosY = (maxX[0] / -(mousePos.x - hamsterPosX)) * -mousePos.y;   //Y���W�Z�o

            //---------------------------------------------
            //�n���X�^�[�̌����w��
            //---------------------------------------------
            if (rightThrow && spriteNum == 3)
            {
                spriteNum = 2;
                ima.sprite = hamsterSprite[spriteNum];
                blockMan.ThrowBlockPosChange(spriteNum);
            }
            else if(!rightThrow && (spriteNum == 2))
            {
                spriteNum = 3;
                ima.sprite = hamsterSprite[spriteNum];
                blockMan.ThrowBlockPosChange(spriteNum);
            }

            if (linePosY < topLimit)
            {
                //---------------------------------------------
                //���˗L
                //---------------------------------------------
                linePosX = maxX[0];
                List<Vector3> linePos = new List<Vector3>();
                linePos.Add(lineStartPos);
                bool lineStop = false;

                //Ray���΂�
                Vector3 lineEmdPos = FlyRay(lineStartPos, new Vector3(linePosX, linePosY, 0.0f));
                if (lineEmdPos.x != linePosX || lineEmdPos.y != linePosY) lineStop = true;
                linePos.Add(lineEmdPos);

                //---------------------------------------------
                //���ˌv�Z
                //---------------------------------------------
                bool frastReflection = true;
                while (!lineStop)
                {
                    int frontLineIndex = linePos.Count - 1;
                    float quotient     = ((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y);
                    float multiplier   = blockBoxSize.width / quotient;
                    float nextLinePosY = Mathf.Abs(multiplier) + linePos[frontLineIndex].y;
                    int   maxXIndex    = ((rightThrow && linePos[frontLineIndex].x < 0) || (!rightThrow && linePos[frontLineIndex].x > 0)) ? 0 : 1;
                    float nextLinePosX = maxX[maxXIndex];
                    if (nextLinePosY < topLimit)
                    {
                        //Ray���΂�
                        Vector3 nextLinePos = FlyRay(linePos[linePos.Count - 1], new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                        if (nextLinePos.x != nextLinePosX || nextLinePos.y != nextLinePosY) lineStop = true;
                        linePos.Add(nextLinePos);
                    }
                    else
                    {
                        float multiplierY = (topLimit - linePos[frontLineIndex].y) * (((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y));
                        nextLinePosY = topLimit;
                        nextLinePosX = linePos[frontLineIndex].x - multiplierY;

                        //Ray���΂�
                        Vector3 nextLinePos = FlyRay(linePos[linePos.Count - 1], new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                        if (nextLinePos.x != nextLinePosX || nextLinePos.y != nextLinePosY) lineStop = true;
                        linePos.Add(nextLinePos);
                        break;
                    }
                    if (frastReflection) frastReflection = false;
                }

                return linePos.ToArray();  //�������C���o��(�z��ɕϊ�)
            }
            else
            {
                //---------------------------------------------
                //���˖�
                //---------------------------------------------
                float multiplierY = topLimit / -mousePos.y;
                linePosY = topLimit;
                linePosX = multiplierY * -(mousePos.x - hamsterPosX);

                //Ray���΂�
                Vector3 lineEmdPos = FlyRay(lineStartPos, new Vector3(linePosX, linePosY, 0.0f));

                //---------------------------------------------
                //�������C���o��
                //---------------------------------------------
                return new Vector3[] { lineStartPos, lineEmdPos };
            }
        }

        //========================================================================
        //Ray���΂�
        //========================================================================
        //frontPos;  �J�n�n�_�̍��W
        //nextPos;   �I���n�_�̍��W
        //return;    ���C���I�����W
        //========================================================================
        Vector3 FlyRay(Vector3 frontPos, Vector3 nextPos)
        {
            //���W�ݒ�
            float nextLinePosX = nextPos.x;
            float nextLinePosY = nextPos.y;

            frontPos = new Vector3((frontPos.x + differenceX) / magnification, (frontPos.y + posY + differenceY) / magnification, 0.0f);
            frontPos = mainCamra.ScreenToWorldPoint(frontPos);
            nextPos  = new Vector3((nextLinePosX + differenceX) / magnification, (nextLinePosY + posY + differenceY) / magnification, 0.0f);
            nextPos  = mainCamra.ScreenToWorldPoint(nextPos);

            //Ray�̍쐬
            Ray2D ray = new Ray2D(frontPos, new Vector3(nextPos.x - frontPos.x, nextPos.y - frontPos.y, 0.0f));
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            //�ڐG����
            if (hit.collider)
            {
                string connectObjTag = hit.collider.gameObject.tag;
                if (0 <= Array.IndexOf(blockTag, connectObjTag))
                {
                    Vector3 connectPos = RectTransformUtility.WorldToScreenPoint(mainCamra, hit.point);
                    nextLinePosX = connectPos.x * magnification - differenceX;
                    nextLinePosY = (connectPos.y * magnification - differenceY) - posY;
                }
            }

            //�I�_���W��Ԃ�
            return new Vector3(nextLinePosX, nextLinePosY, 0.0f);
        }

        //========================================================================
        //��������
        //========================================================================
        //linePos; �N�����C���̒��_���W
        //========================================================================
        void DrawLine(Vector3[] linePos)
        {
            //���̒����Z�o��(x' - x)**2 + (y' - y)**2
            int posCpunt = linePos.Length;
            float lineLength = 0.0f;
            for (int i = 0; i < posCpunt - 1; i++)
            { lineLength += Mathf.Sqrt(Mathf.Pow((linePos[i + 1].x - linePos[i].x), 2.0f) + Mathf.Pow((linePos[i + 1].y - linePos[i].y), 2.0f)); }
            ren.material.mainTextureScale = new Vector2(lineLength * 0.01f, 1.0f);

            //���o��
            line.positionCount = posCpunt;
            line.startWidth    = 15.0f;
            line.endWidth      = 15.0f;
            line.SetPositions(linePos);
        }
    }
}