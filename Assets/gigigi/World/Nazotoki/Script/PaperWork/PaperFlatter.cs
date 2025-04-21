using System.Runtime.CompilerServices;
using UdonSharp;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

public class PaperFlatter : UdonSharpBehaviour
{
    Rigidbody rb;
    VRC_Pickup pickUp;
    bool noGravity = true;
    Transform[] bones;
    float[] boneSqrDistances;
    Vector3[] lateRootPositions;
    public float timeScale = 1.0f;
    private Vector3[] velocities; 
    public float dampingFactor = 0.9f;
    [SerializeField] private FixedAxis[] suppressionDirections = new FixedAxis[3] {FixedAxis.x,FixedAxis.z, FixedAxis.y };
    [SerializeField][Range(0, 1)] private float[] suppressionStrengths = new float[3] {1,1,1};

    private Vector3[] forces; // 蓄積される力
    public float forceAccumulationRate = 2.0f; // 力の蓄積率
    public float forceReleaseThreshold = 0.1f; // 力を解放するしきい値
    public float bounceMultiplier = 1.5f;
    float[] rotationAmount;
    Vector3[] bounceAxis;
    float[] bounceLerp;
    float[] rebounceLerp;

    private Quaternion[] originalRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickUp = GetComponent<VRC_Pickup>();
        var boneParent = transform.Find("Armature");
        if (boneParent != null)
        {
            bones = boneParent.GetComponentsInChildren<Transform>();
            velocities = new Vector3[bones.Length];
            boneSqrDistances = new float[bones.Length];
            lateRootPositions = new Vector3[bones.Length];
            forces = new Vector3[bones.Length];
            rebounceAxis =new Vector3[bones.Length];
            rotationAmount = new float[bones.Length];
            bounceAxis = new Vector3[bones.Length];
            bouncingForce = new float[bones.Length];
            remainingRotation = new float[bones.Length];
            bounceLerp = new float[bones.Length];
            rebounceLerp = new float[bones.Length];
            forceReleased = new bool[bones.Length];
            switchDir = new bool[bones.Length];
            startBounce = new bool[bones.Length];
            originalRotation = new Quaternion[bones.Length];

            for (int i = 0; i < bones.Length; i++)
            {
                if (boneParent != bones[i].parent) boneParent = bones[i].parent;
                boneSqrDistances[i] = Vector3.SqrMagnitude(bones[i].position - boneParent.position);
                lateRootPositions[i] = bones[i].position;
                originalRotation[i] =  bones[i].localRotation;
            }
        }
    }

    public void DoFlatter()
    {
        if (noGravity) SetFollenOn();
        else SetFollenOff();
    }
    public void ResetPaper()
    {
        if (!noGravity) SetFollenOff();
        else SetFollenOn();
    }

    private void FixedUpdate()
    {
        if (bones.Length == 0 || noGravity || straighten) return;
        CalculateBounce();
    }
    
    void CalculateBounce()
    {
        float deltaTime = Time.fixedDeltaTime * timeScale;

        for (int i = 0; i < bones.Length; i++)
        {
            Vector3 swingVector = lateRootPositions[i]- bones[i].position;
            float physStr = Vector3.Magnitude(swingVector);
            float bounceStrength = bounceness.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);
            float flexFactor = flexibility.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);
            
            if (physStr > 0.0001f)
            {
                swingVector = swingVector.normalized;
                swingVector = ApplyDirectionalSuppression(swingVector, i);

                // 力を蓄積する（動きに応じて）
                forces[i] += swingVector * physStr * forceAccumulationRate;

                float forceMagnitude = forces[i].magnitude;

                float dotProduct = Vector3.Dot(forces[i].normalized, swingVector);

                Vector3 forwardDir = bones[i].up;
                if (forceMagnitude > forceReleaseThreshold && dotProduct < 0.7f)
                {
                    // 反発力を速度に変換（bouncenessによって強さを調整）
                    velocities[i] += forces[i].normalized * forceMagnitude * bounceStrength * bounceMultiplier;
                    forces[i] = Vector3.zero; // 力をリセット
                }

                bounceAxis[i] = Vector3.Cross(forwardDir, swingVector);

                if (bounceAxis[i].sqrMagnitude > 0.0001f)
                {
                    velocities[i] += swingVector * physStr;
                    float actualDamping = Mathf.Lerp(flexFactor, dampingFactor, 0.01f * deltaTime);
                    velocities[i] *= actualDamping;
                    //float gravityStr = glavityForce.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);
                    //velocities[i] += gravityStr * -Vector3.up;
                    float angularSpeed = Vector3.Angle(forwardDir, velocities[i]);
                    rotationAmount[i] = angularSpeed * flexFactor;

                    if (rotationAmount[i] > 0.01f)
                    {
                        float limitedAngle = limitAngle.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);
                        if (limitedAngle < rotationAmount[i]) rotationAmount[i] = limitedAngle;
                        bounceLerp[i] = 0;
                        startBounce[i] = true;
                    }
                }

                if (forces[i] == Vector3.zero)
                {
                    rebounceAxis[i] = bounceAxis[i];
                    bouncingForce[i] = rotationAmount[i] * -1;
                    remainingRotation[i] = rotationAmount[i] ;
                    rebounceLerp[i] = -rotationAmount[i];
                    forceReleased[i] = true;
                }
            }
            if (startBounce[i]) DoBounce(i, rotationAmount[i], bounceAxis[i] ,bounceStrength, flexFactor, deltaTime);
            if (forceReleased[i])
            {
                DoRebounce(i , bounceStrength, flexFactor, deltaTime);
            }
        }
        for(int i=0; i< lateRootPositions.Length; i++)
        {
            lateRootPositions[i] = bones[i].position;
        }
    }
    
    public AnimationCurve limitAngle = AnimationCurve.EaseInOut(
            timeStart: 0f,
            valueStart: 0f,
            timeEnd: 1f,
            valueEnd: 180f
        );

    public AnimationCurve flexibility = AnimationCurve.Constant(timeStart: 0f, timeEnd: 1f, value:1f);
    public AnimationCurve bounceness = AnimationCurve.Constant(timeStart: 0f, timeEnd: 1f, value: 0.9f);
    public AnimationCurve glavityForce = AnimationCurve.EaseInOut( timeStart: 0f, valueStart: 0f, timeEnd: 1f, valueEnd: 0f);
   
    bool[] startBounce;
    public void DoBounce(int i , float rotationAmount,Vector3 bounceAxis , float bounceStrength, float flexFactor, float deltaTime)
    {
        bounceLerp[i] = Mathf.Lerp( bounceLerp[i], rotationAmount, 10 * deltaTime );
         
        if (bounceLerp[i] < rotationAmount * 0.45f)
        {
            Quaternion velocityRotation = Quaternion.AngleAxis(bounceLerp[i], bounceAxis.normalized);
            bones[i].rotation = velocityRotation * bones[i].rotation;
        }
        else
        {
            startBounce[i] = false;
        }
    }

    bool[] forceReleased;
    float[] bouncingForce;// 初期の回転量
    float[] remainingRotation; // 残りの回転量を追跡
    bool[] switchDir; // 方向切り替えフラグ
    Vector3[] rebounceAxis;

    public void DoRebounce(int i, float bounceStrength, float flexFactor, float deltaTime)
    {
        if (Mathf.Abs(remainingRotation[i]) > 0.01f)
        {
            rebounceLerp[i] = Mathf.Lerp(rebounceLerp[i], bouncingForce[i],  10f * deltaTime);
            float actualDamping = Mathf.Lerp(flexFactor, dampingFactor, 10f* deltaTime);
            float limitedAngle = limitAngle.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);
            if (limitedAngle < Mathf.Abs(rebounceLerp[i])) rebounceLerp[i] = limitedAngle;

            if (Mathf.Abs(rebounceLerp[i]) > Mathf.Abs(bouncingForce[i])/4)
            {
                switchDir[i] = !switchDir[i];
                bouncingForce[i] *= -1f * actualDamping;
                rebounceLerp[i] *= -1f;
            }

            // 回転を適用
            Quaternion velocityRotation = Quaternion.AngleAxis(
                switchDir[i] ? -rebounceLerp[i] : rebounceLerp[i],
                rebounceAxis[i].normalized
            );
            bones[i].rotation = velocityRotation * bones[i].rotation;

            // 残りの回転量を減少
            remainingRotation[i] = Mathf.Abs(remainingRotation[i]) - Mathf.Abs(rebounceLerp[i]) * actualDamping;
        }
        else
        {
            // バウンス終了処理
            forceReleased[i] = false;
            remainingRotation[i] = 0;
        }
    }
    

    private Vector3 ApplyDirectionalSuppression(Vector3 originalVector, int i)
    {
        Vector3 result = originalVector;


        for (int j = 0; j < suppressionStrengths.Length; j++)
        {
            Vector3 normalizedDir = Vector3.zero;
            if (suppressionDirections[j] == FixedAxis.z) normalizedDir = bones[i].forward;
            if (suppressionDirections[j] == FixedAxis.x) normalizedDir = bones[i].right;
            if (suppressionDirections[j] == FixedAxis.y) normalizedDir = bones[i].up;
            float dotProduct = Vector3.Dot(result, normalizedDir);
            Vector3 componentToSuppress = normalizedDir * dotProduct;

            // 抑制を適用
            result -= componentToSuppress * suppressionStrengths[j];
        }

        // 再正規化
        if (result.sqrMagnitude > 0.0001f)
        {
            result.Normalize();
        }

        return result;
    }
    
    void SetFollenOn()
    {
        rb.useGravity = true;
        rb.isKinematic = false;
        noGravity = false;
    }
    void SetFollenOff()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        noGravity = true;
    }
    bool straighten;
    public void DoStraighten()
    {
        if(!straighten)straighten = true;

        for(int i=0; i< bones.Length; i++)
        {
            if(i != 0) bones[i].localRotation = originalRotation[i] ;
            if (i == bones.Length-1) straighten = false;
        } 
    }
    void ReleaseStraighten()
    {
        straighten = false;
    }

    float _threshold = 0.05f;
    void OnCollisionStay(Collision coliision)
    {
        if (coliision.transform.root == transform) return;
        for( int i=3; i< bones.Length; i++)
        {
            Transform bone = bones[i];
            var dot = Vector3.Dot(bone.up, coliision.contacts[0].normal);
            if (dot > -1f && dot < _threshold || dot < 1 && dot > _threshold)
            {
                var rotateAmount = Vector3.Angle(bone.forward, coliision.contacts[0].normal);
                float flexFactor = flexibility.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);

                if (dot <1f && dot > _threshold) rotateAmount *= -1f;

                bone.rotation = Quaternion.Slerp(bone.rotation, Quaternion.AngleAxis(rotateAmount, bone.right) * bone.rotation, flexFactor * Time.deltaTime);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root == transform) return;
        if(other.gameObject.layer == 9 )
        {
            for (int i = 3; i < bones.Length; i++)
            {
                Transform bone = bones[i];
                var boneToCol = bone.transform.position - other.ClosestPoint(bone.transform.position);
                var dot = Vector3.Dot(bone.up, boneToCol);
                if (dot > _threshold && dot < 0 || dot < _threshold && dot > 0)
                {
                    var rotateAmount = Vector3.Angle(bone.forward, boneToCol);
                    float flexFactor = flexibility.Evaluate(boneSqrDistances[i] / boneSqrDistances[boneSqrDistances.Length - 1]);

                    if (dot < 1f && dot > _threshold) rotateAmount *= -1f;

                    bone.rotation = Quaternion.Slerp(bone.rotation, Quaternion.AngleAxis(rotateAmount, bone.right) * bone.rotation, flexFactor * Time.deltaTime);
                }
            }
        }
    }
}
public enum FixedAxis
{
    x,  y, z
};