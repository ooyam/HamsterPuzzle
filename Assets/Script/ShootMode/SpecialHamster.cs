using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;

public class SpecialHamster : MonoBehaviour
{
    [Header("�u���b�N�{�b�N�X")]
    [SerializeField]
    RectTransform blockBoxTra;

    [Header("�n���X�^�[�X�v���C�g")]
    [SerializeField]
    Sprite[] hamsterSprite;   //0:�ʏ� 1:������� 2:������

    Image ima;               //�X�y�V�����n���X�^�[Image
    Image filterIma;         //�X�y�V�����n���X�^�[�t�B���^�[Image
    BoxCollider2D coll;      //�X�y�V�����n���X�^�[BoxCollider2D
    RectTransform tra;       //�X�y�V�����n���X�^�[RectTransform
    RectTransform parentTra; //�X�y�V�����n���X�^�[�̐e�I�u�W�F�N�g��RectTransform

    [System.NonSerialized]
    public float lowestLinePosY;  //���݂̍Œ�u���b�N�s��Y���W
    [System.NonSerialized]
    bool specialAvailable;        //�X�y�V�����g�p�\��ԁH
    [System.NonSerialized]
    public bool specialHavestNow; //���n���H

    string[] blockTag;        //�u���b�N�^�O���X�g
    BlockManager blockMan;    //BlockManager

    void Start()
    {
        ima  = GetComponent<Image>();
        tra  = GetComponent<RectTransform>();
        coll = GetComponent<BoxCollider2D>();
        filterIma = tra.GetChild(0).GetComponent<Image>();
        parentTra = tra.parent.GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(() => StartCoroutine(OneLineHarvest()));

        //�u���b�N�^�O�擾
        System.Array vegetableType = Enum.GetValues(typeof(VegetableType));
        blockTag = new string[vegetableType.Length];
        foreach (VegetableType value in vegetableType)
        { blockTag[(int)value] = Enum.GetName(typeof(VegetableType), value); }
        blockMan = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager>();

        StartCoroutine(EraseTenBlocks());//�e�X�g
    }

    //========================================================================
    //�X�y�V�����g�p�\���
    //========================================================================
    public IEnumerator EraseTenBlocks()
    {
        if (!specialAvailable)
        {
            //�g�k�ݒ�
            float scalingSpeed = 0.005f;  //�g�嗦�ύX���x
            float changeScale  = 1.05f;   //�ύX���̊g�嗦
            float endScale     = 1.0f;    //�I�����̊g�嗦

            //�_�Őݒ�
            float colouringSpeed = 0.1f;    //�_�ő��x
            Color filterOn       = new Color(1.0f, 1.0f, 1.0f, 192.0f / 255.0f);   //�t�B���^�[��
            Color filterOff      = new Color(1.0f, 1.0f, 1.0f, 0.0f);              //�t�B���^�[����

            //����J�n
            specialAvailable = true;
            float nowScale   = tra.localScale.x;
            bool change      = true;
            while (specialAvailable)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    if (change)
                    {
                        //�g��
                        if (nowScale >= changeScale) change = false;
                        tra.localScale = Vector3.MoveTowards(tra.localScale, Vector3.one * changeScale, scalingSpeed);

                        //�t�B���^�[��
                        filterIma.color = Color.Lerp(filterIma.color, filterOn, colouringSpeed);
                    }
                    else
                    {
                        //�k��
                        if (nowScale <= endScale) break;
                        tra.localScale = Vector3.MoveTowards(tra.localScale, Vector3.one * endScale, scalingSpeed);

                        //�t�B���^�[����
                        filterIma.color = Color.Lerp(filterIma.color, filterOff, colouringSpeed);
                    }

                    //���݂̊g�嗦�X�V
                    nowScale = tra.localScale.x;
                }

                //���Z�b�g
                tra.localScale  = Vector3.one * endScale;
                filterIma.color = filterOff;
                change = true;
            }

            //���n�J�n
            specialHavestNow = true;
        }
    }

    //========================================================================
    //1�s���n
    //========================================================================
    IEnumerator OneLineHarvest()
    {
        if (specialAvailable)
        {
            //�ꕔ�̓��쒆�͏I������܂őҋ@
            yield return new WaitWhile(() => blockMan.throwNow == true);         //����
            yield return new WaitWhile(() => blockMan.blockGenerateNow == true); //�u���b�N����
            yield return new WaitWhile(() => blockMan.blockDeleteNow == true);   //�u���b�N�폜
            yield return new WaitWhile(() => blockMan.blockChangeNow == true);   //�����u���b�N�؂�ւ�

            //���n�J�n
            SPECIAL_HARVEST  = true;
            specialAvailable = false;
            yield return new WaitUntil(() => specialHavestNow == true);

            //�����W�ECollider�Escale�擾
            Vector2 defaultPos      = tra.anchoredPosition;
            Vector2 defaultCollSize = coll.size;
            Vector2 defaultCollOff  = coll.offset;
            Vector3 defaultScale    = tra.localScale;

            //�e����x������
            float delayTime = 0.5f;

            //�n���X�^�[Sprite�ύX
            tra.rotation   = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            ima.sprite     = hamsterSprite[1];
            tra.localScale = new Vector3(0.9f, 1.0f, 1.0f);
            yield return new WaitForSeconds(delayTime);
            ima.sprite     = hamsterSprite[2];
            tra.localScale = new Vector3(1.0f, 0.8f, 1.0f);

            //�E�[(��ʊO)�Ɉړ��ݒ�
            float moveSpeed   = 12.0f;
            float acceleRate  = 1.0f;
            float rightPosX   = CANVAS_WIDTH / 2.0f + 100.0f;
            Vector2 targetPos = new Vector2(rightPosX, defaultPos.y);
            float mvoeTime    = GetMoveTime(tra, moveSpeed, acceleRate, targetPos);

            //�h��ݒ�
            float rotSpeed = 2.0f;      //�h�ꑬ�x
            float maxRot   = 10.0f;     //�h��p�x
            int moveCount  = -1;        //1�T�C�N�������(�J�E���g���Ȃ��ꍇ�� - 1�w��)
            float stopTime = 0.0f;      //��~����
            int breakCount = -1;        //�I���T�C�N����(�������[�v�̏ꍇ�� - 1�w��)
            float endTime  = mvoeTime;  //�h��I������(���ԂŎ~�߂Ȃ��ꍇ�� - 1�w��)

            //�ړ��J�n
            StartCoroutine(ShakeMovement(tra, rotSpeed, maxRot, moveCount, stopTime, breakCount, endTime));
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime + delayTime);

            //���n���C���E�[�Ɉړ������]
            tra.SetParent(blockBoxTra, false);
            tra.anchoredPosition = new Vector2(rightPosX, lowestLinePosY);
            tra.rotation         = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            coll.size            = new Vector2(50.0f, 30.0f);
            coll.offset          = new Vector2(-30.0f, 0.0f);

            //���[�Ɉړ�
            float leftPosX = -rightPosX;
            targetPos      = new Vector2(leftPosX, lowestLinePosY);
            mvoeTime       = GetMoveTime(tra, moveSpeed, acceleRate, targetPos);
            endTime        = mvoeTime;

            //�ړ��J�n
            StartCoroutine(ShakeMovement(tra, rotSpeed, maxRot, moveCount, stopTime, breakCount, endTime));
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime + delayTime);

            //���̏ꏊ�̉E�[�Ɉړ�
            tra.SetParent(parentTra, false);
            tra.anchoredPosition = new Vector2(rightPosX, defaultPos.y);

            //���̏ꏊ�Ɉړ�
            mvoeTime = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
            endTime  = mvoeTime;

            //�ړ��J�n
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
            yield return new WaitForSeconds(mvoeTime);

            //�n���X�^�[Sprite�ύX
            ima.sprite = hamsterSprite[0];
            tra.anchoredPosition = defaultPos;
            coll.size            = defaultCollSize;
            coll.offset          = defaultCollOff;
            tra.localScale       = defaultScale;

            //�I������
            SPECIAL_HARVEST  = false;
            specialHavestNow = false;
        }
        else
        {
            //���nNG����(���E�h��)
            float shakeSpeed   = 20.0f;    //�ړ����x
            float shakeOffsetX = 20.0f;    //�ړ����WX
            float shakeOffsetY = 0.0f;     //�ړ����WY
            int   shakeTimes   = 4;        //�h���
            float delayTime    = 0.0f;     //�ړ��Ԃ̒x������
            StartCoroutine(SlideShakeMovement(tra, shakeSpeed, shakeOffsetX, shakeOffsetY, shakeTimes, delayTime));
        }
    }

    //========================================================================
    //�ڐG����
    //========================================================================
    void OnTriggerEnter2D(Collider2D col)
    {
        if (SPECIAL_HARVEST)
        {
            GameObject connectObj = col.gameObject;
            string connectObjTag = connectObj.tag;
            int tagIndex = Array.IndexOf(blockTag, connectObjTag);
            if (0 <= tagIndex) blockMan.BlockHarvest(connectObj);
        }
    }
}