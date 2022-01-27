using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ShootMode;
using ShootMode_Tutorial;
using SoundFunction;
using static ShootMode.ShootModeDefine;
using static MoveFunction.ObjectMove;

namespace ShootMode_Tutorial
{
    public class FeverHamuster_Tutorial : MonoBehaviour
    {
        [Header("�n���X�^�[�X�v���C�g")]
        [SerializeField]
        Sprite hamsterDefSpr;

        BoxCollider2D coll;         //�t�B�[�o�[�n���X�^�[BoxCollider2D
        RectTransform tra;          //�t�B�[�o�[�n���X�^�[RectTransform
        GameObject feverHamObj;     //�t�B�[�o�[�n���X�^�[RectTransform
        RectTransform startObjTra;  //�t�B�[�o�[�J�n�I�u�W�F�N�gRectTransform(�t�B�[�o�[�J�n�I�u�W�F�N�g)

        float magnification;     //�^�b�v���쎞�̏C���l
        float differenceX;       //�^�b�v���쎞�̏C���l
        float differenceY;       //�^�b�v���쎞�̏C���l
        Vector2 defaultPos = new Vector2(0.0f, -530.0f);

        string objName;                   //���g�̃^�O
        string[] blockTag;                //�u���b�N�^�O���X�g
        BlockManager_Tutorial blockMan;   //BlockManager
        ScoreManager_Tutorial scoreMan;   //ScoreManager
        SoundManager soundMan;            //SoundManager
        GameObject mainHamObj;            //���C���n���X�^�[Box�I�u�W�F�N�g
        TutorialManager tutorialMan;      //�`���[�g���A���}�l�[�W���[
        Camera mainCamera;                //���C���J����
        bool hamsterMove;                 //�ړ��J�n�t���O

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
            blockMan    = GameObject.FindWithTag("BlockManager").GetComponent<BlockManager_Tutorial>();
            scoreMan    = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager_Tutorial>();
            soundMan    = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
            mainHamObj  = GameObject.FindWithTag("HamsterBox");
            tutorialMan = GameObject.Find("Tutorial").GetComponent<TutorialManager>();

            //SE
            soundMan.FeverStartSE_Shoot();

            //BGM
            StartCoroutine(soundMan.FeverBGM_Shoot());

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
            targetPos  = new Vector2(0.0f, 150.0f);
            mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
            StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime);

            //���C���n���X�^�[��\��
            mainHamObj.SetActive(false);
            feverHamObj.SetActive(true);

            //��U������(����)
            moveSpeed  = 1.5f;
            acceleRate = 1.05f;
            targetPos  = new Vector2(0.0f, 100.0f);
            mvoeTime   = GetMoveTime(startObjTra, moveSpeed, acceleRate, targetPos);
            StartCoroutine(MoveMovement(startObjTra, moveSpeed, acceleRate, targetPos));
            yield return new WaitForSeconds(mvoeTime);

            //��U������(����)
            moveSpeed  = 3.0f;
            acceleRate = 0.98f;
            targetPos  = new Vector2(0.0f, 50.0f);
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

            //���̐�����
            tutorialMan.NextDescriptionStart();

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
            //���̈ʒu�ɖ߂�
            hamsterMove      = false;
            float moveSpeed  = 5.0f;
            float acceleRate = 1.0f;
            float mvoeTime   = GetMoveTime(tra, moveSpeed, acceleRate, defaultPos);
            tra.GetChild(0).GetComponent<Image>().sprite = hamsterDefSpr;
            if (tra.anchoredPosition.x > defaultPos.x) tra.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            StartCoroutine(MoveMovement(tra, moveSpeed, acceleRate, defaultPos));
            yield return new WaitForSeconds(mvoeTime);

            //���C���n���X�^�[�\��
            mainHamObj.SetActive(true);
            FEVER_START = false;

            //���̐�����
            tutorialMan.NextDescriptionStart();
            Destroy(this.gameObject);
        }

        //========================================================================
        //�n���X�^�[�ړ�
        //========================================================================
        IEnumerator HamsterMove()
        {
            float maxX = PLAY_SCREEN_WIDTH / 2.0f;
            bool firstTap = true;
            while (hamsterMove)
            {
                yield return new WaitForFixedUpdate();
                if (firstTap && Input.GetMouseButtonDown(0))
                {
                    //���\��
                    tutorialMan.HandHide();
                    firstTap = false;
                }
                if (Input.GetMouseButton(0))
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
                }
            }
        }

        //========================================================================
        //�ڐG����
        //========================================================================
        void OnTriggerEnter2D(Collider2D col)
        {
            //���n����
            if (FEVER_START)
            {
                GameObject connectObj = col.gameObject;
                string connectObjTag  = connectObj.tag;
                int tagIndex = Array.IndexOf(blockTag, connectObjTag);
                if (0 <= tagIndex)
                {
                    //SE
                    soundMan.FeverHarvestSE_Shoot();
                    blockMan.fallCompleteCount++;
                    scoreMan.HarvestVegetable(connectObjTag);
                    Destroy(connectObj);
                }
            }
        }
    }
}