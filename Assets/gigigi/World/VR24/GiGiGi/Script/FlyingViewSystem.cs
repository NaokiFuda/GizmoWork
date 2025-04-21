using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingViewSystem : MonoBehaviour
{
    [SerializeField] private float x_Axis= 0.1f;
    [SerializeField] private float z_Axis = 0.1f;
    [SerializeField] private float y_Axis= 0.05f;
    [SerializeField] private float mouseSenciblity = 15f;
    private Vector3 _viewPonit = new Vector3(0, 0, 0);
    private float _mouseX_Axis = 0;
    private float _mouseY_Axis = 0;

    void Start()
    {
        _mouseX_Axis = Input.GetAxis("Mouse X");
        _mouseY_Axis = Input.GetAxis("Mouse Y");
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _viewPonit.z = z_Axis;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _viewPonit.z = z_Axis * -1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _viewPonit.x = x_Axis;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _viewPonit.x = x_Axis * -1.0f;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            _viewPonit.y = y_Axis;
        }
        transform.Translate(_viewPonit);
        _viewPonit = new Vector3(0, 0, 0);

        if (_mouseX_Axis - Input.GetAxis("Mouse X") != 0)
        {
            transform.Rotate(0.0f, (Input.GetAxis("Mouse X")-_mouseX_Axis) * mouseSenciblity, 0.0f, Space.World);
        }
        if (_mouseY_Axis - Input.GetAxis("Mouse Y") != 0)
        {
            transform.rotation = transform.rotation * new Quaternion((_mouseY_Axis-Input.GetAxis("Mouse Y")) / mouseSenciblity, 0, 0, 1.0f - (_mouseY_Axis - Input.GetAxis("Mouse Y")) / mouseSenciblity);

        }
    }
}
