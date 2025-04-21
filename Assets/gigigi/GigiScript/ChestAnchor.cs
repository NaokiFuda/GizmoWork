
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ChestAnchor : UdonSharpBehaviour
{
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
        // GetTrackingData(Chest)でカメラ位置が取得できるみたい
        transform.position = m_owner.GetBonePosition(UnityEngine.HumanBodyBones.Chest);
        transform.rotation = m_owner.GetBoneRotation(UnityEngine.HumanBodyBones.Chest); 
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
