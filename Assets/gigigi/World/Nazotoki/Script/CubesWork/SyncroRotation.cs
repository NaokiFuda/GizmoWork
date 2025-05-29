
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncroRotation : UdonSharpBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 aimRotation;
    [SerializeField] Vector3 rotationAxis;
    [SerializeField] Vector3 aimAxis;
    [SerializeField] Vector3 effectedAxis;
    Quaternion _defRot;
    Vector3 _defPos;
    [SerializeField] float sencibility = 1.5f;

    private void Start()
    {
        _defRot = transform.localRotation;
        _defPos = transform.localPosition;
    }
    private void Update()
    {
        float xRot = 0;
        float yRot = 0;
        float zRot = 0;
        float wRot = 1;

        float xPos = _defPos.x;
        float zPos = _defPos.z;
        float yPos = _defPos.y;

        aimRotation = aimRotation.normalized;
        Quaternion targetRot = new Quaternion(aimRotation.x * target.localRotation.x, aimRotation.y * target.localRotation.y, aimRotation.z * target.localRotation.z , target.localRotation.w);
        Vector3 targetPos = new Vector3(target.localPosition.x * aimAxis.x, target.localPosition.y * aimAxis.y, target.localPosition.z * aimAxis.z);
        if (rotationAxis.y != 0)
        {
            yRot = targetRot.x + targetRot.y+ targetRot.z;
        }
        if (rotationAxis.x != 0)
        {
            xRot = targetRot.x + targetRot.y + targetRot.z;
        }
        if (rotationAxis.z != 0)
        {
            zRot = targetRot.x + targetRot.y + targetRot.z;
        }
        wRot = targetRot.w;

        if (effectedAxis.y != 0)
        {
            yPos += (targetPos.x+ targetPos.y+ targetPos.z) * sencibility;
        }
        if (effectedAxis.x != 0)
        {
            xPos += (targetPos.x + targetPos.y + targetPos.z) * sencibility;
        }
        if (effectedAxis.z != 0)
        {
            zPos += (targetPos.x + targetPos.y + targetPos.z) * sencibility;
        }
        Quaternion spinRot = new Quaternion(xRot, yRot, zRot, wRot);
        transform.localRotation = spinRot * _defRot;
        transform.localPosition = new Vector3(xPos, yPos, zPos) ;
    }
}
