
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DrawMeshInstance : UdonSharpBehaviour
{
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;
    [SerializeField]
    private int MeshNum=0;
    [SerializeField]
    private float MeshXMoveOn = 1.0f;
    [SerializeField]
    private float MeshYMoveOn = 0;
    [SerializeField]
    private float MeshZMoveOn = 0;
    [SerializeField]
    private int UpperRandamizeRotationRange = 0;
    [SerializeField]
    private int BottomRandamizeRotationRange = 0;
    [SerializeField]
    private Vector3 StartPos = new Vector3(0f, 0f, 0f);
    [SerializeField]
    private Quaternion StartRot = Quaternion.Euler(new Vector3(0f, 0f, 0f));

    private Matrix4x4[] _Meshtransforms;

    private void Start()
    {
        if (StartPos == new Vector3(0, 0, 0)) { StartPos = this.transform.position + new Vector3(MeshXMoveOn, MeshYMoveOn, MeshZMoveOn);}
        if (UpperRandamizeRotationRange < BottomRandamizeRotationRange){
            BottomRandamizeRotationRange = UpperRandamizeRotationRange;
        }
        _Meshtransforms = new Matrix4x4[MeshNum];
        for (var i = 0; i < _Meshtransforms.Length; i++)
        {
            var position = StartPos + new Vector3(i * MeshXMoveOn, i * MeshYMoveOn, i * MeshZMoveOn); 
            var rotate = this.transform.rotation * StartRot * Quaternion.Euler(new Vector3(0f, Random.Range(BottomRandamizeRotationRange, UpperRandamizeRotationRange), 0f));
            _Meshtransforms[i] = Matrix4x4.TRS(position, rotate, Vector3.one);
        }
    }

    private void Update()
    {
        VRCGraphics.DrawMeshInstanced(_mesh, 0, _material, _Meshtransforms);
    }
}
