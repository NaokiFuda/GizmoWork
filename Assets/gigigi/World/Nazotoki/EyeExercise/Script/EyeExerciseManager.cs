

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class EyeExerciseManager : UdonSharpBehaviour
{
    [SerializeField] float[] randomSpeeds;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] public Transform popArea;
    GameObject[] _targets = new GameObject[3];
    float[] setedSpeed;
    bool[] _targetedTargetIndexs;
    bool[] _knockedTargetIndexs;
    Vector3[] _directionList;
    Vector3[] _targetDir;
    Vector3[] _lasthandPos = new Vector3[2];
    bool[] inputHandIsLeft;

    void Start()
    {
        setedSpeed = new float[_targets.Length];
        _knockedTargetIndexs = new bool[_targets.Length];
        _targetedTargetIndexs = new bool[_targets.Length];
        _directionList = new Vector3[4] { targetPrefab.transform.GetChild(0).up, targetPrefab.transform.GetChild(0).right, -targetPrefab.transform.GetChild(0).up, -targetPrefab.transform.GetChild(0).right };
        _targetDir = new Vector3[_targets.Length];
        inputHandIsLeft = new bool[_targets.Length];

        for (int i = 0; i<_targets.Length;i++)
        {
            _targets[i] = Instantiate(targetPrefab, popArea.position + popArea.right * i * 2, transform.rotation);
            InitilizeTarget(i);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _targets.Length; i ++)
        {
            if (_targetedTargetIndexs[i])
            {
                if (!inputHandIsLeft[i])
                {
                    Vector3 dir = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position - _lasthandPos[0];
                    if(dir.sqrMagnitude > 0.1f )
                    {
                        dir = new Vector3(dir.x * Mathf.Abs(_targetDir[i].x), dir.y * Mathf.Abs(_targetDir[i].y), dir.z * Mathf.Abs(_targetDir[i].z));
                        if (Vector3.Dot(dir, _targetDir[i]) > 0.7f) { _knockedTargetIndexs[i] = true; }
                        Debug.Log(_targets[i].transform.GetChild(0).right +" " + dir);
                    }
                    _lasthandPos[0] = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
                }
                else
                {
                    Vector3 dir = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position - _lasthandPos[1];
                    if (dir.sqrMagnitude > 0.1f)
                    {
                        dir = new Vector3(dir.x * _targetDir[i].x, dir.y * _targetDir[i].y, dir.z * _targetDir[i].z);
                        if (Vector3.Dot(dir, _targetDir[i]) > 0.7f) { _knockedTargetIndexs[i] = true; }
                    }

                    _lasthandPos[1] = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
                }
            }
            if (_knockedTargetIndexs[i] &&_targets[i].transform.position.z - popArea.position.z < 0)
            {
                InitilizeTarget(i);
            }

            MoveTarget(_targets[i].transform, i);
        }
            
       

    }
    void InitilizeTarget(in int i)
    {
        _knockedTargetIndexs[i] = false;

        setedSpeed[i] = randomSpeeds[Random.Range(0, randomSpeeds.Length)];
        _targetDir[i] = _directionList[Random.Range(0, 3)];
        _targets[i].transform.GetChild(0).rotation = Quaternion.FromToRotation(_targets[i].transform.GetChild(0).right, _targetDir[i]) * _targets[i].transform.GetChild(0).rotation;
    }

    void MoveTarget(in Transform target, in int i)
    {
        float speed = setedSpeed[i];
        if (_knockedTargetIndexs[i]) speed = -randomSpeeds[randomSpeeds.Length-1];

        target.position += Vector3.forward * speed;
    }

    public void KnockBackTarget(in Transform target, in UdonInputEventArgs args)
    {
        if(args.boolValue)
        {
            for (int i = 0; i < _targets.Length; i++)
                if (_targets[i].transform == target)
                {
                    _targetedTargetIndexs[i] = true;
                    if (args.handType == HandType.RIGHT) inputHandIsLeft[i] = false;
                    else inputHandIsLeft[i] = true;
                }
        }
        else
            for (int i = 0; i < _targets.Length; i++)
                if (_targets[i].transform == target)
                    _targetedTargetIndexs[i] = false;

    }
    public void UnLockTarget(UdonInputEventArgs args)
    {
        if(!args.boolValue)
        for (int i = 0; i < _targets.Length; i++)
            if (_targetedTargetIndexs[i])
            {
                _targetedTargetIndexs[i]=false;
            }
    }
}