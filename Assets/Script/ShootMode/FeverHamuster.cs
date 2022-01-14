using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;


public class FeverHamuster : MonoBehaviour
{
    BoxCollider2D coll;         //�t�B�[�o�[�n���X�^�[BoxCollider2D
    RectTransform tra;          //�t�B�[�o�[�n���X�^�[RectTransform
    GameObject feverHamObj;     //�t�B�[�o�[�n���X�^�[RectTransform
    RectTransform startObjTra;  //�t�B�[�o�[�J�n�I�u�W�F�N�gRectTransform(�t�B�[�o�[�J�n�I�u�W�F�N�g)

    float magnification;     //�^�b�v���쎞�̏C���l
    float differenceX;       //�^�b�v���쎞�̏C���l
    float differenceY;       //�^�b�v���쎞�̏C���l
    Vector2 defaultPos = new Vector2(0.0f, -530.0f);

    string objName;          //���g�̃^�O
    string[] blockTag;       //�u���b�N�^�O���X�g
    BlockManager blockMan;   //BlockManager
    GameObject mainHamObj;   //���C���n���X�^�[�I�u�W�F�N�g
    Camera mainCamera;       //���C���J����
    bool hamsterMove;        //�ړ��J�n�t���O

    //========================================================================
    //�t�B�[�o�[�J�n(�S����)
    //========================================================================
    public IEnumerator FeverStart()
    {
        mainCamera  = Camera.main;
        objName     = this.gameObject.name;
        tra         = GetComponent<RectTransform>();
        coll        = GetComponent<BoxCollider2D>();
        feverHamObj = tra.GetChild(0).gameObject;
        startObjTra = tra.GetChild(1).GetComponent<RectTransform>();
        tra.anchoredPosition = defaultPos;
        feverHamObj.SetActive(false);

        magnification = CANVAS_WIDTH / Screen.width;
        differenceX   = CANVAS_WIDTH / 2.0f;
        differenceY   = CANVAS_HEIGHT / 2.0f;

        //�u���b�N�^�O�擾
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        blockMan   = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();
        mainHamObj = GameObject.FindWithTag("Hamster");

        //�t�B�[�o�[�J�n�I�u�W�F�N�g��ʒ����܂ňړ�
        float moveSpeed   = 15.0f;
        float acceleRate  = 1.0f;
        Vector2 targetPos = new Vector2(0.0f, 70.0f);
        float mvoeTime    = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //����
        moveSpeed  = 15.0f;
        acceleRate = 0.9f;
        targetPos  = new Vector2(0.0f, 175.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //���C���n���X�^�[��\��
        mainHamObj.SetActive(false);
        feverHamObj.SetActive(true);

        //��U������(����)
        moveSpeed  = 1.5f;
        acceleRate = 1.05f;
        targetPos  = new Vector2(0.0f, 125.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //��U������(����)
        moveSpeed  = 3.0f;
        acceleRate = 0.98f;
        targetPos  = new Vector2(0.0f, 75.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //�㏸�J�n(����)
        moveSpeed  = 1.0f;
        acceleRate = 1.3f;
        targetPos  = new Vector2(0.0f, 100.0f);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);

        //��ʊO�܂ŏ㏸
        moveSpeed  = 15.0f;
        acceleRate = 1.01f;
        targetPos  = new Vector2(0.0f, CANVAS_HEIGHT);
        mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
        StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
        yield return new WaitForSeconds(mvoeTime);
        Destroy(startObjTra.gameObject);

        //�n���X�^�[�ړ��\
        hamsterMove = true;
        StartCoroutine(HamsterMove());

        //�u���b�N�����_�������J�n
        StartCoroutine(blockMan.FeverStrat(this));
    }

    //========================================================================
    //�n���X�^�[���̈ʒu�ɖ߂�
    //========================================================================
    public IEnumerator ReturnFirstPosition()
    {
        hamsterMove      = false;
        float moveSpeed  = 12.0f;
        float acceleRate = 1.0f;
        float mvoeTime   = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
        StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
        yield return new WaitForSeconds(mvoeTime);

        //���C���n���X�^�[�\��
        mainHamObj.SetActive(true);
        Destroy(this.gameObject);
    }

    //========================================================================
    //�n���X�^�[�ړ�
    //========================================================================
    IEnumerator HamsterMove()
    {
        bool push  = false;
        float maxX = CANVAS_WIDTH / 2.0f;
        while (hamsterMove)
        {
            yield return new WaitForFixedUpdate();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit2d = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction);
                if (hit2d && hit2d.transform.gameObject.name == objName)
                {
                    push = true;
                }
            }
            if (push)
            {
                Vector3 mousePos = Input.mousePosition;
                //���W�̏C��
                mousePos.x = mousePos.x * magnification - differenceX;
                mousePos.y = mousePos.y * magnification - differenceY;
                mousePos.z = 0.0f;
                // X, Y���W�͈̔͂𐧌�����
                mousePos.x = Mathf.Clamp(mousePos.x, -maxX, maxX);
                mousePos.y = Mathf.Clamp(mousePos.y, defaultPos.y, defaultPos.y);
                tra.anchoredPosition = Vector3.Lerp(tra.anchoredPosition, mousePos, 1.0f);
                if (Input.GetMouseButtonUp(0))
                {
                    push = false;
                }
            }
        }
    }
}
