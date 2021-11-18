using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShootMode
{
    public class HamsterController : MonoBehaviour
    {
        RectTransform tra;                 //RectTransform
        Image ima;                         //Image
        RectTransform canvasTra;           //Canvas��RectTransform
        LineRenderer line;                 //LineRenderer

        [Header("Sprite")]
        public Sprite[] hamsterSprite;    //0:�ʏ�(�E����) 1:���](������)
        [System.NonSerialized]
        public bool spriteDefault = true; //0�Ԏg�p���H

        [Header("BlockManager")]
        public BlockManager blockMan;
        bool tapStart = false;             //�^�b�v�J�n
        float magnification;               //�^�b�v�ʒu�C���{��
        float differenceX;                 //�^�b�v�ʒu�C����X
        float differenceY;                 //�^�b�v�ʒu�C����Y
        float canvasHigh;                  //Canvas�̍���(1920.0f)
        float canvasWidth;                 //Canvas�̕�(1080.0f)
        float maxX;                        //�n���X�^�[�Ǐ]���E�l
        float posY = -530.0f;              //�n���X�^�[Y���W
        float posZ = -1.0f;                //�n���X�^�[Z���W
        float fastTapPosY;                 //�ŏ��ɐG�����ʒu
        bool throwOperation = false;       //�����铮��
        float throwTriggerTapPos = -100.0f;//�����n�߂�ʒu
        float throwStopTapPos = -50.0f;    //������̂���߂�ʒu
        Vector3 lineStartPos = new Vector3(0.0f, 30.0f, 0.0f);//�������C���̃X�^�[�g�ʒu
        [System.NonSerialized]
        public bool gameStart = false;     //�Q�[���J�n�H

        void Start()
        {
            tra       = GetComponent<RectTransform>();
            ima       = GetComponent<Image>();
            canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
            line      = GetComponent<LineRenderer>();

            canvasHigh    = canvasTra.sizeDelta.y;
            canvasWidth   = canvasTra.sizeDelta.x;
            differenceX   = canvasWidth / 2;
            differenceY   = canvasHigh / 2;
            magnification = canvasWidth / Screen.width;
            maxX          = differenceX - 50.0f;
        }

        //========================================================================
        //�^�b�v����
        //========================================================================
        void FixedUpdate()
        {
            if (gameStart)
            {
                if (!blockMan.throwNow && !blockMan.blockDeleteNow)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        fastTapPosY = Input.mousePosition.y * magnification - differenceY;
                        tapStart = true;
                    }
                    if (tapStart)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        //���W�̏C��
                        Vector3 targetPos = new Vector3(mousePos.x * magnification - differenceX, (mousePos.y * magnification - differenceY) - posY, posZ);
                        if (targetPos.y < throwTriggerTapPos || (throwOperation && targetPos.y < throwStopTapPos))
                        {
                            Vector3[] linePos = LineCalculation(targetPos);
                            DrawLine(linePos);
                            if (throwOperation)
                            {
                                if (Input.GetMouseButtonUp(0))
                                {
                                    throwOperation = false;
                                    tapStart = false;
                                    line.positionCount = 0;
                                    //�u���b�N�𓊂���
                                    StartCoroutine(blockMan.BlockThrow(linePos));
                                }
                            }
                            else throwOperation = true;
                        }
                        else
                        {
                            /*/�ړ�����
                            targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
                            tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, new Vector3(targetPos.x, posY), 1.0f);
                            */
                            if (throwOperation)
                            {
                                //������̂���߂�
                                line.positionCount = 0;
                                throwOperation = false;
                            }
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
            float linePosX  = 0.0f;
            float linePosY  = 0.0f;
            float hamPosX   = tra.anchoredPosition.x;
            bool rightThrow = mousePos.x < hamPosX;                                //�E�ɓ����n�߂�?
            float maxY      = canvasHigh / 2 - posY - blockMan.blockDiameter / 2;  //Y�ő�l
            float[] maxX    = new float[2];                                        //X�ő�l
            maxX[0]         = (rightThrow) ? differenceX - hamPosX : -differenceX - hamPosX;
            maxX[1]         = (rightThrow) ? -differenceX - hamPosX : differenceX - hamPosX;
            linePosY        = (maxX[0] / -(mousePos.x - hamPosX)) * -mousePos.y;   //Y���W�Z�o

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

            if (linePosY < maxY)
            {
                //---------------------------------------------
                //���˗L
                //---------------------------------------------
                linePosX = maxX[0];
                List<Vector3> linePos = new List<Vector3>();
                linePos.Add(lineStartPos);
                linePos.Add(new Vector3(linePosX, linePosY, 0.0f));

                //---------------------------------------------
                //���ˌv�Z
                //---------------------------------------------
                bool frastReflection = true;
                while (true)
                {
                    int frontLineIndex = linePos.Count - 1;
                    float quotient = ((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y);
                    float multiplier = canvasWidth / quotient;
                    float nextLinePosY = Mathf.Abs(multiplier) + linePos[frontLineIndex].y;
                    int maxXIndex = ((rightThrow && linePos[frontLineIndex].x < 0) || (!rightThrow && linePos[frontLineIndex].x > 0)) ? 0 : 1;
                    float nextLinePosX = maxX[maxXIndex];
                    if (nextLinePosY < maxY)
                    {
                        linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                    }
                    else
                    {
                        float multiplierY = (maxY - linePos[frontLineIndex].y) * (((frastReflection) ? linePos[frontLineIndex].x : linePos[frontLineIndex].x * 2.0f) / (linePos[frontLineIndex].y - linePos[frontLineIndex - 1].y));
                        nextLinePosY = maxY;
                        nextLinePosX = linePos[frontLineIndex].x - multiplierY;
                        linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
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
                float multiplierY = maxY / -mousePos.y;
                linePosY = maxY;
                linePosX = multiplierY * -(mousePos.x - hamPosX);

                //---------------------------------------------
                //�������C���o��
                //---------------------------------------------
                return new Vector3[] { lineStartPos, new Vector3(linePosX, linePosY, 0.0f) };
            }
        }

        //========================================================================
        //��������
        //========================================================================
        //linePos; �N�����C���̒��_���W
        //========================================================================
        void DrawLine(Vector3[] linePos)
        {
            line.positionCount = linePos.Length;
            line.startWidth = 20.0f;
            line.endWidth = 20.0f;
            line.SetPositions(linePos);
        }
    }
}