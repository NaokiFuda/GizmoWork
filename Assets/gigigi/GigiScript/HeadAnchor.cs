
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeadAnchor : UdonSharpBehaviour
{
    /// <summary>
    /// 有効かどうか
    /// </summary>
    private bool m_isActive = false;
    [SerializeField] float AnchorUp = 0.6f;
    [SerializeField] bool rotateTopBottom = false;

    /// <summary>
    /// プレイヤー
    /// </summary>
    private VRCPlayerApi m_owner;

    void Start()
    {

    }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    private void Update()
    {
        if (!m_isActive)
        {
            return;
        }
        // GetTrackingData(Head)でカメラ位置が取得できるみたい
        transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, AnchorUp, 0);
        if (rotateTopBottom)
        {
            transform.rotation = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        }
        else
        {
            transform.rotation = m_owner.GetRotation() * Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    /// <summary>
    /// 有効化する
    /// </summary>
    public void Activate()
    {
        // その時点のオーナーに
        m_owner = Networking.GetOwner(gameObject);
        m_isActive = !m_isActive;
    }
}