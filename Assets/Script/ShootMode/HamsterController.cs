using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterController : MonoBehaviour
{
    RectTransform tra;                 //RectTransform
    RectTransform canvasTra;           //Canvas��RectTransform
    LineRenderer line;                 //LineRenderer

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

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();
        canvasTra = GameObject.FindWithTag("CanvasMain").GetComponent<RectTransform>();
        line = GetComponent<LineRenderer>();
        canvasHigh = canvasTra.sizeDelta.y;
        canvasWidth = canvasTra.sizeDelta.x;
        differenceX = canvasWidth / 2;
        differenceY = canvasHigh / 2;
        magnification = canvasWidth / Screen.width;
        maxX = differenceX - 50.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
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
                if (!throwOperation) throwOperation = true;
            }
            else
            {
                //�ړ�����
                targetPos.x = Mathf.Clamp(targetPos.x, -maxX, maxX);
                tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, new Vector3(targetPos.x, posY), 1.0f);

                if (throwOperation)
                {
                    //������̂���߂�
                    line.positionCount = 0;
                    throwOperation = false;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            tapStart = false;
            throwOperation = false;
            line.positionCount = 0;
        }
    }

    //�������C�����W�v�Z
    Vector3[] LineCalculation(Vector3 mousePos)
    {
        float linePosX = 0.0f;
        float linePosY = 0.0f;
        float hamPosX = tra.anchoredPosition.x;
        float maxY = canvasHigh - (blockMan.blockPosY * (blockMan.nowLineNum - 1)) + posY;     //Y�ő�l
        float maxX = (mousePos.x < hamPosX) ? differenceX - hamPosX : -differenceX - hamPosX;  //X�ő�l
        float multiplierX = maxX / -(mousePos.x - hamPosX);                                    //�搔         
        linePosY = multiplierX * -mousePos.y;                                                  //Y���W�Z�o

        //���˗L
        if (linePosY <= maxY)
        {
            linePosX = maxX;
            List<Vector3> linePos = new List<Vector3>();
            linePos.Add(lineStartPos);
            linePos.Add(new Vector3(linePosX, linePosY, 0.0f));

            //���ˌv�Z
            while (true)
            {
                float frontLinePosX = linePos[linePos.Count - 1].x;
                float frontLinePosY = linePos[linePos.Count - 1].y;
                float quotient = frontLinePosX / frontLinePosY;
                float multiplier = canvasWidth / quotient;
                float nextLinePosY = Mathf.Abs(multiplier) + frontLinePosY;
                float nextLinePosX = (frontLinePosX >= 0) ? -(canvasWidth - Mathf.Abs(maxX)) : canvasWidth - Mathf.Abs(maxX);
                if (nextLinePosY < maxY)
                {
                    linePos.Add(new Vector3(nextLinePosX, nextLinePosY, 0.0f));
                }
                else
                {
                    break;
                }
            }

            //�������C���o��(�z��ɕϊ�)
            return linePos.ToArray();
        }
        //���˖�
        else
        {
            float multiplierY = maxY / -mousePos.y;
            linePosY = maxY;
            linePosX = multiplierY * -(mousePos.x - hamPosX);

            //�������C���o��
            return new Vector3[] { lineStartPos, new Vector3(linePosX, linePosY, 0.0f) };
        }
    }

    //��������
    void DrawLine(Vector3[] linePos)
    {
        line.positionCount = linePos.Length;
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.SetPositions(linePos);
    }
}
