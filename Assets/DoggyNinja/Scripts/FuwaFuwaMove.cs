using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuwaFuwaMove : MonoBehaviour
{
    [SerializeField] Transform rootBone;
    Transform[] bonesTransform;
    float[] bonesLength;
    Vector3 _lastPos;
    Vector3 _lastDir;
    Vector3[] _defDir;
    float[] _lastForce;
    float[] _deltaForce;
    float maxLength;

    Quaternion[] _defRot;
    [SerializeField] private bool suppressX = false, suppressY = false, suppressZ = false;
    [SerializeField][Range(0, 1)] private float[] suppressionStrengths = new float[3] { 0, 0, 0 };

    int grabIndex = -1;
    [SerializeField] Transform[] grabPoint;
    Transform[] _grabedParent;



    [SerializeField] AnimationCurve hardness = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0.1f, timeEnd: 1f, valueEnd: 0.1f);
    [SerializeField] AnimationCurve rebounceness = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 0.5f, timeEnd: 1f, valueEnd: 0.5f);
    [SerializeField] AnimationCurve angleLimitness = AnimationCurve.EaseInOut(timeStart: 0f, valueStart: 1f, timeEnd: 1f, valueEnd: 1f);
    [SerializeField] float angleLimit = 90f;



    [SerializeField] float glabDistance = 0.3f;



    void Start()
    {
        if (rootBone == null) { rootBone = transform; }
        bonesTransform = rootBone.transform.GetComponentsInChildren<Transform>();
        bonesLength = new float[bonesTransform.Length];
        _lastForce = new float[bonesTransform.Length];
        _defDir = new Vector3[bonesTransform.Length];
        _defRot = new Quaternion[bonesTransform.Length];
        _deltaForce = new float[bonesTransform.Length];

        for (int i = 0; i < bonesTransform.Length; i++)
        {
            var t = bonesTransform[i];
            if (i != 0)
            {
                var k = 1;
                while (bonesTransform[i - k] != t.parent) k++;
                bonesLength[i] = bonesLength[i - k] + Vector3.Distance(t.parent.position, t.position);
                if (bonesLength[i] > maxLength) maxLength = bonesLength[i];
            }
            else
            {
                bonesLength[i] = 0;
                _lastPos = rootBone.position;
                _lastDir = rootBone.up;
            }
            _defDir[i] = t.up;
            _defRot[i] = t.localRotation;
        }
    }


    void FixedUpdate()
    {
        Vector3 forceDir = CaluculateSwingDirection();
        float force = forceDir.magnitude;
        if (grabIndex >= 0)
        {
            FuwaFuwa(grabIndex, forceDir, force);
        }
        else
        {
            FuwaFuwa(0, forceDir, force);
        }

        _lastPos = rootBone.position;
        _lastDir = rootBone.up;
    }

    void FuwaFuwa(in int rootIndex, in Vector3 forceDir, in float force)
    {
        for (int i = rootIndex; i < bonesTransform.Length; i++)
        {
            var t = bonesTransform[i];

            if (i == 0) continue;
            _lastForce[i] = force / bonesLength[i];

            Vector4 forceDir4 = ApplyDirectionalSuppression(forceDir.normalized * _lastForce[i], i);
            forceDir4.w = i;

            DoFuwa(forceDir4);

            _deltaForce[i] = Vector3.Distance(t.up, _defDir[i]);
            ReFuwa(i);

        }
    }



    void DoFuwa(in Vector4 swingDir4)
    {
        var i = (int)swingDir4.w;
        var t = bonesTransform[i];
        t.localRotation = CalculateRotate(swingDir4, i);
    }
    void ReFuwa(in int i)
    {
        Transform t = bonesTransform[i];
        float rebounceStrength = Mathf.Clamp(rebounceness.Evaluate(bonesLength[i] / maxLength), 0, 1);

        Quaternion returnRot = Quaternion.RotateTowards(t.localRotation, _defRot[i], _deltaForce[i] * rebounceStrength * 10);

        t.localRotation = returnRot;

    }


    Vector3 CaluculateSwingDirection()
    {
        Vector3 swingDir = (rootBone.position - _lastPos);
        swingDir += rootBone.up - _lastDir;

        return -swingDir;
    }

    private Vector3 ApplyDirectionalSuppression(Vector3 originalVector, int i)
    {
        Vector3 result = originalVector;


        for (int j = 0; j < suppressionStrengths.Length; j++)
        {
            Vector3 normalizedDir = Vector3.zero;
            if (j == 0 && suppressZ) normalizedDir = bonesTransform[i].forward;
            if (j == 1 && suppressX) normalizedDir = bonesTransform[i].right;
            if (j == 2 && suppressY) normalizedDir = bonesTransform[i].up;
            float dotProduct = Vector3.Dot(result, normalizedDir);
            Vector3 componentToSuppress = normalizedDir * dotProduct;

            // �}����K�p
            result -= componentToSuppress * suppressionStrengths[j];
        }

        // �Đ��K��
        if (result.sqrMagnitude > 0.0001f)
        {
            result.Normalize();
        }

        return result;
    }

    Quaternion CalculateRotate(in Vector3 swingDir, in int i)
    {
        Transform t = bonesTransform[i];

        // ���[���h���W�n�Ŏ��Ɗp�x���v�Z
        Vector3 worldAxis = Vector3.Cross(t.up, t.up + swingDir).normalized;
        float angle = Vector3.SignedAngle(t.up, t.up + swingDir, worldAxis);

        // ���������ȏꍇ�i���s�x�N�g���j�͌��݂̉�]��Ԃ�
        if (worldAxis.sqrMagnitude < 0.001f)
        {
            return t.localRotation;
        }

        float hardnessStrength = 1 - Mathf.Clamp(hardness.Evaluate(bonesLength[i] / maxLength), 0, 1);
        float al = angleLimit * Mathf.Clamp(angleLimitness.Evaluate(bonesLength[i] / maxLength), 0, 1);
        angle = Mathf.Clamp(angle, -al, al);

        Quaternion fixedTwist = _defRot[i] * Quaternion.Inverse(t.localRotation);
        fixedTwist.x = 0;
        fixedTwist.z = 0;
        fixedTwist = fixedTwist.normalized;
        Quaternion fixedCurrentRot = fixedTwist * t.localRotation;

        Vector3 localAxis = t.InverseTransformDirection(worldAxis);

        Quaternion localTargetRotation = Quaternion.AngleAxis(angle, localAxis);
        Quaternion targetRot = localTargetRotation * fixedCurrentRot;

        return Quaternion.Slerp(fixedCurrentRot, targetRot, hardnessStrength);
    }



}
