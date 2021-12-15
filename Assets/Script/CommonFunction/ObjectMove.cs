using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            bool leftMove      = true;   //���ɉ�]�H
            bool rotReturn     = false;  //�p�x�߂����H
            bool shakeStop     = false;  //��~�H
            int loopTimes      = 0;      //�����
            int cycleTimes     = 0;      //�T�C�N����
            float playTime     = 0.0f;   //�h�ꓮ��Đ�����
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
        //acceleRate; ������
        //targetPos;  �ڕW���W
        //========================================================================
        public static IEnumerator MoveMovement(RectTransform tra, float moveSpeed, float acceleRate, Vector2 targetPos)
        {
            float offset   = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 nowPos = tra.anchoredPosition; //���݂̍��W
            bool sideways  = Mathf.Abs(targetPos.x - nowPos.x) >= Mathf.Abs(targetPos.y - nowPos.y); //X�����ɓ���H
            while (true)
            {
                yield return new WaitForFixedUpdate();
                moveSpeed *= acceleRate;
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, targetPos, moveSpeed);
                nowPos = tra.anchoredPosition;

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


        //========================================================================
        //�ړ�����(MoveMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //moveSpeed;  ���쑬�x
        //acceleRate; ������
        //startPos;   �J�n���W
        //targetPos;  �ڕW���W
        //return;     ���v����
        //========================================================================
        public static float GetMoveTime(float moveSpeed, float acceleRate, Vector2 startPos, Vector2 targetPos)
        {
            float moveTime     = 0.0f;   //�ړ�����
            float oneFrameTime = 0.02f;  //1�t���[���̎���
            float offset       = 0.5f;   //��~�ꏊ�̃I�t�Z�b�g
            float distanceX    = Mathf.Abs(targetPos.x - startPos.x) - offset;               //�ړ�����X
            float distanceY    = Mathf.Abs(targetPos.y - startPos.y) - offset;               //�ړ�����Y
            float moveDistance = Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY);  //���ړ�����

            //�v�Z
            while (true)
            {
                moveSpeed    *= acceleRate;
                moveDistance -= moveSpeed;
                moveTime     += oneFrameTime;
                if (moveDistance <= 0) break;
            }
            return moveTime;
        }


        //========================================================================
        //���E�h�ꓮ��
        //========================================================================
        //tra;        ����I�u�W�F�N�g��RectTransform
        //shakeSpeed; ���쑬�x
        //offsetX;    �ڕW���W�I�t�Z�b�gX
        //offsetY;    �ڕW���W�I�t�Z�b�gY
        //shakeTimes; �ړ���
        //delayTime;  �ړ��Ԃ̑ҋ@����
        //========================================================================
        public static IEnumerator SlideShakeMovement(RectTransform tra, float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float offset       = 0.5f;                 //��~�ꏊ�̃I�t�Z�b�g
            Vector2 defaultPos = tra.anchoredPosition; //�������W�擾
            bool sideways      = Mathf.Abs(offsetX) >= Mathf.Abs(offsetY); //X�����ɓ���H

            //��������
            for (int moveCount = 0; moveCount < shakeTimes; moveCount++)
            {
                int vector = (moveCount % 2 == 0) ? 1 : -1;
                Vector2 tarPos = new Vector2(defaultPos.x + offsetX * vector, defaultPos.y + offsetY * vector);
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, tarPos, shakeSpeed);
                    Vector2 nowPos = tra.anchoredPosition;

                    //---------------------------------------------
                    //���̈ړ���
                    //---------------------------------------------
                    if ((sideways && tarPos.x - offset <= nowPos.x && nowPos.x <= tarPos.x + offset) ||
                        (!sideways && tarPos.y - offset <= nowPos.y && nowPos.y <= tarPos.y + offset))
                    {
                        tra.anchoredPosition = tarPos;
                        yield return new WaitForSeconds(delayTime);
                        break;
                    }
                }
            }

            //���̍��W�ɖ߂�
            while (true)
            {
                yield return new WaitForFixedUpdate();
                tra.anchoredPosition = Vector2.MoveTowards(tra.anchoredPosition, defaultPos, shakeSpeed);
                Vector2 nowPos = tra.anchoredPosition;

                //---------------------------------------------
                //�ړ��I��
                //---------------------------------------------
                if ((sideways && defaultPos.x - offset <= nowPos.x && nowPos.x <= defaultPos.x + offset) ||
                    (!sideways && defaultPos.y - offset <= nowPos.y && nowPos.y <= defaultPos.y + offset))
                {
                    tra.anchoredPosition = defaultPos;
                    break;
                }
            }
        }


        //========================================================================
        //���E�h�ꓮ��(SlideShakeMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //shakeSpeed; ���쑬�x
        //offsetX;    �ڕW���W�I�t�Z�b�gX
        //offsetY;    �ڕW���W�I�t�Z�b�gY
        //shakeTimes; �ړ���
        //delayTime;  �ړ��Ԃ̑ҋ@����
        //========================================================================
        public static float GetSlideShakeTime(float shakeSpeed, float offsetX, float offsetY, int shakeTimes, float delayTime)
        {
            float moveTime     = 0.0f;   //�ړ�����
            float oneFrameTime = 0.02f;  //1�t���[���̎���
            float moveDistance = Mathf.Sqrt((offsetX * offsetX) + (offsetY * offsetY));  //���ړ�����

            //�v�Z
            if (shakeTimes != 0)
            {
                moveTime = moveDistance * shakeTimes * 2 / shakeSpeed * oneFrameTime + delayTime * shakeTimes;
            }

            return moveTime;
        }


        //========================================================================
        //��]����
        //========================================================================
        //traArray;    ����I�u�W�F�N�g��RectTransform[]
        //rotSpeed;    �g�k���x
        //stopRot;     ��]��̊p�x(��Ίp)
        //========================================================================
        public static IEnumerator RotateMovement(RectTransform[] traArray, Vector3 rotSpeed, Vector3 stopRot)
        {
            //�ł��������삷�鎲����
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //��]
            float tolerance = 5.0f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                foreach (RectTransform tra in traArray)
                { tra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z); }
                Vector3 nowRot   = traArray[0].localEulerAngles;
                float refRot     = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot     = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot     = nowRot.z;
                        refStopRot = stopRot.z;
                        break;
                }
                if (refStopRot - tolerance <= refRot && refRot <= refStopRot + tolerance) break;
            }

            //�ŏI�p�x�ɍ��킹��
            foreach (RectTransform tra in traArray)
            { tra.localRotation = Quaternion.Euler(stopRot.x, stopRot.y, stopRot.z); }
        }


        //========================================================================
        //��]����(RotateMovement)�ɗv���鎞�Ԍv�Z
        //========================================================================
        //obj;         ����I�u�W�F�N�g
        //tra;         ����I�u�W�F�N�g��RectTransform
        //rotSpeed;    �g�k���x
        //stopRot;     ��]��̊p�x(��Ίp)
        //========================================================================
        public static float GetRotateMoveTime(GameObject obj, RectTransform tra, Vector3 rotSpeed, Vector3 stopRot)
        {
            //�I�u�W�F�N�g�N���[���쐻
            GameObject clone       = GameObject.Instantiate(obj) as GameObject;
            RectTransform cloneTra = clone.GetComponent<RectTransform>();
            Transform parentTra    = tra.parent.gameObject.transform;
            cloneTra.SetParent(parentTra, false);
            cloneTra.localRotation = tra.localRotation;

            //�ł��������삷�鎲����
            int axis = 0;
            if (rotSpeed.x < rotSpeed.y)
                axis = (rotSpeed.y > rotSpeed.z) ? 1 : 2;
            else if (rotSpeed.x < rotSpeed.z)
                axis = 2;

            //��]
            float oneFrameTime = 0.02f;
            float moveTime     = 0.0f;
            float tolerance    = 5.0f;
            while (true)
            {
                moveTime += oneFrameTime;
                cloneTra.Rotate(rotSpeed.x, rotSpeed.y, rotSpeed.z);
                Vector3 nowRot = cloneTra.localEulerAngles;
                float refRot = nowRot.x;
                float refStopRot = stopRot.x;
                switch (axis)
                {
                    case 1:
                        refRot = nowRot.y;
                        refStopRot = stopRot.y;
                        break;
                    case 2:
                        refRot = nowRot.z;
                        refStopRot = stopRot.z;
                        break;
                }
                if (refStopRot - tolerance <= refRot && refRot <= refStopRot + tolerance) break;
            }

            Destroy(clone);  //�N���[���폜
            return moveTime; //���Ԃ�Ԃ�
        }


        //========================================================================
        //�g��k������
        //========================================================================
        //tra;          ����I�u�W�F�N�g��RectTransform
        //scalingSpeed; �g�k���x
        //changeScale;  �ύX���̊g�嗦
        //endScale;     �I�����̊g�嗦
        //scalingTimes; �g�k��
        //========================================================================
        public static IEnumerator ScaleChange(RectTransform tra, float scalingSpeed, float changeScale, float endScale, int scalingTimes)
        {
            Vector3 nowScale = tra.localScale;    //���݂̊g�嗦
            bool scaleUp     = scalingSpeed > 0;  //�g��H
            bool scaleChange = true;              //�ύX���쒆�H
            bool end         = false;             //�ύX����I���H

            for (int loopTimes = 0; loopTimes < scalingTimes; loopTimes++)
            {
                while (true)
                {
                    yield return new WaitForFixedUpdate();
                    if (scaleChange)
                    {
                        //---------------------------------------------
                        //�ύX����
                        //---------------------------------------------
                        nowScale = Vector3.one * (nowScale.x + scalingSpeed);
                        if ((scaleUp && nowScale.x >= changeScale) || (!scaleUp && nowScale.x <= changeScale))
                            scaleChange = false;
                    }
                    else
                    {
                        //---------------------------------------------
                        //�I������
                        //---------------------------------------------
                        nowScale = Vector3.one * (nowScale.x - scalingSpeed);
                        if ((scaleUp && nowScale.x <= endScale) || (!scaleUp && nowScale.x >= endScale))
                            end = true;
                    }
                    tra.localScale = nowScale;
                    if (end) break;
                }

                //�ϐ����Z�b�g
                tra.localScale = Vector3.one * endScale;
                scaleChange = true;
                end = false;
            }
        }


        //========================================================================
        //�F�ύX(2�F�_��)����  ��Image or Text �̎g�p�Ȃ����� null ���w�肷��
        //========================================================================
        //ima;          �ύX�Ώ�Image
        //tex;          �ύX�Ώ�Text
        //changeSpeed;  �ύX���x
        //colArray;     �ύX�F�̔z��
        //compArray;    ��r�ԍ��w��z��(0:R 1:G 2:B 3:A)
        //chengeCount;  ���[�v��(�z��1����1�J�E���g�A-1�w��Ŗ����Đ�)
        //========================================================================
        //changeEnd;    �_�Œ�~
        //========================================================================
        public static bool changeEnd = false;
        public static IEnumerator PaletteChange(Image ima, Text tex, float changeSpeed, Color[] colArray, int[] compArray, int chengeCount)
        {
            float oneFrameTime = 0.02f;            //1�t���[������
            int loopTimes      = 0;                //�J��Ԃ���
            int colCount       = colArray.Length;  //�ύX�F�̐�

            int nowIndex  = 0;    //���݂̐F
            int nextIndex = 1;    //���̐F
            float nextCompCol = colArray[nextIndex][compArray[nowIndex]];   //��r�F�w��
            float judgeRange  = 5.0f / 255.0f;                              //����͈�

            if (tex == null)
            {
                //-------------------------
                //Image
                //-------------------------
                while (!changeEnd)
                {
                    ima.color = Color.Lerp(ima.color, colArray[nextIndex], changeSpeed);
                    float nowCompCol = ima.color[compArray[nowIndex]];
                    if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                    {
                        nowIndex = nextIndex;
                        nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                        nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                        loopTimes++;
                    }
                    if (chengeCount >= 0 && loopTimes >= chengeCount) break;
                    yield return new WaitForSecondsRealtime(oneFrameTime);
                }
            }
            else
            {
                //-------------------------
                //Text
                //-------------------------
                while (!changeEnd)
                {
                    tex.color = Color.Lerp(tex.color, colArray[nextIndex], changeSpeed);
                    float nowCompCol = tex.color[compArray[nowIndex]];
                    if (nowCompCol + judgeRange >= nextCompCol && nextCompCol >= nowCompCol - judgeRange)
                    {
                        nowIndex = nextIndex;
                        nextIndex = (nextIndex + 1 >= colCount) ? 0 : nextIndex + 1;
                        nextCompCol = colArray[nextIndex][compArray[nowIndex]];
                        loopTimes++;
                    }
                    if (chengeCount >= 0 && loopTimes >= chengeCount) break;
                    yield return new WaitForSecondsRealtime(oneFrameTime);
                }
            }
            changeEnd = false;
        }
    }
}