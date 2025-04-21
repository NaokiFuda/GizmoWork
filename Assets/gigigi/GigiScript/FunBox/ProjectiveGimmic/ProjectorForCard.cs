
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProjectorForCard : UdonSharpBehaviour
{
    [Tooltip("複製物の表示する高さ）")]
    [SerializeField] private float _yAxis = 1.0f;
    [Tooltip("複製物の拡大率）")]
    [SerializeField] private float _scale = 2.0f;
    private GameObject _obj;
    private GameObject _monitorObj;
    private bool _setObj;
    private Sprite getSprite;
    private SpriteRenderer monitor;
    private Sprite _defMonitorSprite;
    private Vector3 _defMonitorScale;
    private Vector3 _defMonitorPos;
    private bool _isSprite;

    void Start()
    {
        _monitorObj = transform.GetChild(0).gameObject;
        _defMonitorScale = _monitorObj.transform.localScale;
        monitor = _monitorObj.GetComponent<SpriteRenderer>();
        _defMonitorSprite = monitor.sprite;
        _defMonitorPos = _monitorObj.transform.localPosition;
    }
    void Update()
    {
        if (_setObj)
        {
            _setObj = false;
            if (_obj.GetComponent<SpriteRenderer>())
            {
                getSprite = _obj.transform.GetComponent<SpriteRenderer>().sprite;
                _isSprite = true;
            }
            else
            {
                for (int i = 0; i < _obj.transform.childCount; i++)
                {
                    if (_obj.transform.GetChild(i).GetComponent<SpriteRenderer>())
                    {
                        getSprite = _obj.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                        _isSprite = true;
                        break;
                    }
                }

            }
        }
        if (_isSprite)
        {
            _isSprite = false;
            monitor.sprite = getSprite;
            _monitorObj.transform.localScale = _defMonitorScale * _scale;
            _monitorObj.transform.localPosition = Vector3.up * _yAxis;
        }
        if(_obj == null && _monitorObj.transform.localScale != _defMonitorScale)
        {
            monitor.sprite = _defMonitorSprite;
            _monitorObj.transform.localScale = _defMonitorScale;
            _monitorObj.transform.localPosition = _defMonitorPos;
        }

        if(Networking.LocalPlayer != null)
        {
            _monitorObj.transform.LookAt(Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position);
            _monitorObj.transform.localRotation = new Quaternion(0, _monitorObj.transform.localRotation.y, 0, _monitorObj.transform.localRotation.w) * new Quaternion(0, 1f, 0, 0);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13 && other.gameObject.name[0] == 'C')
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
}