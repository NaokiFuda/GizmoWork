
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GiGiInstantiate : UdonSharpBehaviour
{
    [SerializeField] private GameObject prefab;

    private Transform cashedTransform;

    private void Start()
    {
        cashedTransform = GetComponent<Transform>();
    }

    public void Activate()
    {
        var go = Instantiate(prefab);
        go.GetComponent<Transform>().position = cashedTransform.position;
    }
}
