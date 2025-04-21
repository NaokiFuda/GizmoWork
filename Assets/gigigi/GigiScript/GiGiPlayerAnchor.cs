
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

/// <summary>
/// �v���C���[�̓��̏�ɂ��Ă�����
/// </summary>
public class GiGiPlayerAnchor : UdonSharpBehaviour
{
    /// <summary>
    /// �L�����ǂ���
    /// </summary>
    private bool m_isActive = false;

    /// <summary>
    /// �v���C���[
    /// </summary>
    private VRCPlayerApi m_owner;

    void Start()
    {

    }

    /// <summary>
    /// ���t���[���Ă΂��
    /// </summary>
    private void Update()
    {
        if (!m_isActive)
        {
            return;
        }
        // GetTrackingData(Head)�ŃJ�����ʒu���擾�ł���݂���
        transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + new Vector3(0, 1.0f, 0);
        transform.rotation = m_owner.GetRotation() * Quaternion.AngleAxis(180, Vector3.up);
    }

    /// <summary>
    /// �L��������
    /// </summary>
    public void Activate()
    {
        // ���̎��_�̃I�[�i�[��
        m_owner = Networking.GetOwner(gameObject);
        m_isActive = !m_isActive;
    }
}