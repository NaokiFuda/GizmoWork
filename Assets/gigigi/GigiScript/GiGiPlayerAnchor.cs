
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// プレイヤーの頭の上についてくるやつ
/// </summary>
public class GiGiPlayerAnchor : UdonSharpBehaviour
{
    /// <summary>
    /// 有効かどうか
    /// </summary>
    private bool m_isActive = false;

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
        transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, 1.0f, 0);
        transform.rotation = m_owner.GetRotation() * Quaternion.AngleAxis(180, Vector3.up);
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