
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProjectiveCore : UdonSharpBehaviour
{
    [SerializeField]
    private Material _material;
    [SerializeField]
    private Mesh _mesh;
    private GameObject _obj;
    [Tooltip("複製物の表示する高さ）")]
    [SerializeField] private float _yAxis = 1.0f;
    [Tooltip("複製物の拡大率）")]
    [SerializeField] private float _scale = 2.0f;

    private Quaternion _displayRot;
    private Vector3 _displayPos;

    private Matrix4x4[] _meshtransform;
    private bool _setObj;

    void Start()
    {
        _displayPos = this.transform.position + new Vector3(0, _yAxis, 0);
    }
    void Update()
    {
        if (_obj != null)
        {
            VRCGraphics.DrawMeshInstanced(_mesh, 0, _material, _meshtransform);
            _setObj = false;
            return;
        }
        if (_setObj)
        {
            _setObj = false;
            _displayRot = _obj.transform.rotation;
            _mesh = _obj.GetComponent<MeshFilter>().mesh;
            var rotate = this.transform.rotation * _displayRot;
            _meshtransform = new Matrix4x4[1];
            for (int i=0; i< _meshtransform.Length; i++)
            {
                _meshtransform[i] = Matrix4x4.TRS(_displayPos, rotate, Vector3.one * _scale);
            }
        }
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 13 && other.gameObject.name[0] == 'C')
        {
            _setObj = true;
            _obj = other.gameObject;
        }
        
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _obj)
        {
            _obj = null;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == null && _obj != null)
        {
            _obj = null;
        }
    }
}
