using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShootMode;
using System;

namespace ShootMode
{
    public class HamsterController : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        RectTransform canvasTra;           //Canvas��RectTransform
        LineRenderer line;                 //LineRenderer
        Camera mainCamra;                  //MainCamera

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:�ʏ�(�E����) 1:���](������)
        [System.NonSerialized]
        public bool spriteDefault = true; //0�Ԏg�p���H

        [Header("BackGround")]
        [SerializeField]
        RectTransform backGroundTra;
        Vector2 backGroundSize;   //backGround�̕�

        [Header("BlockBox")]
        [SerializeField]
        RectTransform blockBoxTra;

        [Header("BlockManager")]
        [SerializeField]
        BlockManager blockMan;

        bool tapStart = false;             //�^�b�v�J�n
        float magnification;               //�^�b�v�ʒu�C���{��
        float differenceX;                 //�^�b�v�ʒu�C����X
        float differenceY;                 //�^�b�v�ʒu�C����Y
        float canvasHigh;                  //Canvas�̍���
        float canvasWidth;                 //Canvas�̕�
        float hamsterPosX;                 //�n���X�^�[X���W
        float posY = -530.0f;              //�n���X�^�[Y���W
        float fastTapPosY;                 //�ŏ��ɐG�����ʒu
        bool  displayLine = false;         //�O�����\�����H
        float throwStartTapPos = -50.0f;   //�����n�߂�ʒu
        float topLimit;                    //�������C��Y���W���
        float[] sideLimit;                 //�������C��X���W���
        float lineStartPosX = 70.0f;       //�������C���̃X�^�[�g�ʒuX
        float lineStartPosY = 30.0f;       //�������C���̃X�^�[�g�ʒuY
        Vector3 lineStartPos;              //�������C���̃X�^�[�g�ʒu
        [System.NonSerialized]
        public bool gameStart = false;     //�Q�[���J�n�H
        string[] blockTag;                 //�u���b�N�^�O���X�g
        string NextBlockBoardTag = "NextBlockBoard"; //�������\���{�[�h�^�O

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            line      = GetComponent<LineRenderer>();
            mainCamra = Camera.main;

            canvasHigh     = canvasTra.sizeDelta.y;
            canvasWidth    = canvasTra.sizeDelta.x;
            differenceX    = canvasWidth / 2.0f;
            differenceY    = canvasHigh  / 2.0f;
            magnification  = canvasWidth / Screen.width;
            hamsterPosX    = tra.anchoredPosition.x;
            backGroundSize = backGroundTra.sizeDelta;
            topLimit  = backGroundSize.y / 2.0f + blockBoxTra.anchoredPosition.y - posY - 100.0f;
            sideLimit = new float[] { backGroundSize.x / 2.0f - hamsterPosX, -backGroundSize.x / 2.0f - hamsterPosX };

            //�u���b�N�^�O�擾
            System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
            blockTag = new string[vegetableType.Length];
            foreach (VegetableType value in vegetableType)
            { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        }

        //========================================================================
        //�^�b�v����
        //========================================================================
        void FixedUpdate()
        {
            if (gameStart)
            {
                if (!blockMan.throwNow && !blockMan.blockDeleteNow&& !blockMan.blockChangeNow)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        //�������{�[�h�^�b�v���͌v�Z���Ȃ�
                        Ray ray = mainCamra.ScreenPointToRay(Input.mousePosition);
                        RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                        if (!(hit2d && hit2d.transform.gameObject.tag == NextBlockBoardTag))
                        {
                            tapStart = true;
                        }
                    }
                    if (tapStart)
                    {
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
                                tapStart = false;
                                line.positionCount = 0;
                                StartCoroutine(blockMan.BlockThrow(linePos));
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
                                tapStart = false;
                        }
                    }
                }
            }
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
            if (rightThrow && !spriteDefault)
            {
                ima.sprite = hamsterSprite[0];
                blockMan.ThrowBlockPosChange(0);
                spriteDefault = true;
            }
            else if(!rightThrow && spriteDefault)
            {
                ima.sprite = hamsterSprite[1];
                blockMan.ThrowBlockPosChange(1);
                spriteDefault = false;
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
                    float multiplier   = backGroundSize.x / quotient;
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
            line.positionCount = linePos.Length;
            line.startWidth    = 20.0f;
            line.endWidth      = 20.0f;
            line.SetPositions(linePos);
        }
    }
}