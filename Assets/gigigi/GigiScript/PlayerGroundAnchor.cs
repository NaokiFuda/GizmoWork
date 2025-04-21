
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerGroundAnchor : UdonSharpBehaviour
{
    private bool m_isActive = false;
    [SerializeField] private bool defaultOn = false;
    /// <summary>
    /// プレイヤー
    /// </summary>
    private VRCPlayerApi m_owner;

    void Start()
    {
        if (defaultOn)
        {
            Activate();
        }

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
        // GetTrackingData(Chest)でカメラ位置が取得できるみたい
        transform.position = m_owner.GetPosition();
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
