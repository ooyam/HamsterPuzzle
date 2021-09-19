using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoveFunction
{
    public class ObjectMove : MonoBehaviour
    {
        //========================================================================
        //�h��铮��
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //moveSpeed;  ���쑬�x
        //maxRot;     �h��p�x
        //moveCount;  1�T�C�N�������(�J�E���g���Ȃ��ꍇ��-1�w��)
        //stopTime;   ��~����
        //breakCount; �I���T�C�N����(�������[�v�̏ꍇ��-1�w��)
        //endTime;    �h��I������(���ԂŎ~�߂Ȃ��ꍇ��-1�w��)
        //========================================================================
        public static IEnumerator ShakeMovement(RectTransform tra, float moveSpeed, float maxRot, int moveCount, float stopTime, int breakCount, float endTime)
        {
            bool leftMove = true;        //���ɉ�]�H
            bool rotReturn = false;      //�p�x�߂����H
            bool shakeStop = false;      //��~�H
            int loopTimes = 0;           //�����
            int cycleTimes = 0;          //�T�C�N����
            float playTime = 0.0f;       //�h�ꓮ��Đ�����
            float oneFrameTime = 0.02f;  //1�t���[���̎���
            while (true)
            {
                yield return new WaitForFixedUpdate();
                float rotZ = tra.localRotation.eulerAngles.z;
                rotZ = (rotZ >= 180.0f) ? rotZ - 360.0f : rotZ;
                if (!rotReturn)
                {
                    if (leftMove)
                    {
                        //---------------------------------------------
                        //���ɉ�]
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, moveSpeed);
                        if (rotZ > maxRot) leftMove = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //�E�ɉ�]
                        //---------------------------------------------
                        tra.Rotate(0.0f, 0.0f, -moveSpeed);
                        if (rotZ < -maxRot)
                        {
                            loopTimes++;                            
                            if (0 < moveCount && moveCount <= loopTimes) rotReturn = true;  //�I���T�C�N�����𒴂������ɗh����~�߂�
                            else leftMove = true;
                        }
                    }
                }
                else
                {
                    //---------------------------------------------
                    //�p�x��0�ɖ߂�
                    //---------------------------------------------
                    tra.Rotate(0.0f, 0.0f, moveSpeed);
                    if (-0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        shakeStop = true;
                    }

                    //---------------------------------------------
                    //�h����~�߂�
                    //---------------------------------------------
                    if (shakeStop)
                    {
                        cycleTimes++;
                        if (0 < breakCount && breakCount <= cycleTimes) break;  //�h��I��
                        else yield return new WaitForSeconds(stopTime);         //�ꎞ��~
                        loopTimes = 0;
                        rotReturn = false;
                        shakeStop = false;
                    }
                }

                //---------------------------------------------
                //���ԂŒ�~����ꍇ�̏���
                //---------------------------------------------
                if (0 < endTime)
                {
                    playTime += oneFrameTime;
                    if (playTime >= endTime && -0.5f <= rotZ && rotZ >= 0.5f)
                    {
                        tra.rotation = Quaternion.Euler(0, 0, 0);
                        break;  //�h��I��
                    }
                }
            }
        }


        //========================================================================
        //�ړ�����
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //moveSpeed;  ���쑬�x
        //targetPos;  �ڕW���W
        //sideways;   X�����ɓ���H
        //========================================================================
        public static IEnumerator MoveMovement(RectTransform tra, float moveSpeed, Vector2 targetPos, bool sideways)
        {
            float offset = 0.5f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, targetPos, moveSpeed);
                Vector2 nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && targetPos.x - offset <= nowPos.x && nowPos.x <= targetPos.x + offset) ||
                    (!sideways && targetPos.y - offset <= nowPos.y && nowPos.y <= targetPos.y + offset))
                {
                    tra.anchoredPosition = targetPos;
                    break;
                }
            }
        }
    }
}