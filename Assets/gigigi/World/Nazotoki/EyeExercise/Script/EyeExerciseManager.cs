

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EyeExerciseManager : UdonSharpBehaviour
{
    [SerializeField] float[] randomSpeeds;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] public Transform popArea;
    GameObject[] _targets = new GameObject[3];
    float[] setedSpeed;
    
    void Start()
    {
        setedSpeed = new float[_targets.Length];
        for (int i = 0; i<_targets.Length;i++)
        {
            _targets[i] = Instantiate(targetPrefab, popArea.position + popArea.right * i * 2, transform.rotation);
            setedSpeed[i] = randomSpeeds[Random.Range(0, randomSpeeds.Length)];
        }
    }

    private void Update()
    {
        for (int i = 0; i < _targets.Length; i ++)
            MoveTarget(_targets[i].transform, i );
    }

    void MoveTarget(in Transform target, in int i)
    {
        target.position += Vector3.forward * setedSpeed[i];
    }

    public void KnockBackTarget(in Transform target)
    {
        target.position += Vector3.forward * -10;

        for (int i =0; i < _targets.Length;i++)
            if (_targets[i] == target)
                setedSpeed[i] = randomSpeeds[Random.Range(0, randomSpeeds.Length)];
    }
   
}