
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PicObj : UdonSharpBehaviour
{

	private bool m_isActive = false;
	private VRCPlayerApi m_owner;

	bool first = false;

	private Vector3 lateOne;
	private Vector3 offset;
	private Vector3 offsetL;
	private Vector3 offsetR;
	private Vector3 headpos;
	private Vector3 Lpos;
	private Vector3 Rpos;
	private Quaternion lateL;
	private Quaternion lateR;
	private Quaternion lateRot;

	private float L_length = 0;
	private float R_length = 0;

	Animator animator;


	private void Update()
	{
		if (!m_isActive)
		{
			return;
		}

		var DorV = Networking.LocalPlayer.IsUserInVR();


		if (DorV == true)
		{
			Lpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position - transform.position;
			L_length = Lpos.sqrMagnitude;
			Rpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position - transform.position;
			R_length = Rpos.sqrMagnitude;

			if ((L_length - R_length) >= 0)
			{
				transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
				transform.rotation = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation * Quaternion.Inverse(lateR) * lateRot;

				if (first == true)
				{
					transform.GetChild(0).gameObject.transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position + offsetR;
					animator = this.gameObject.GetComponent<Animator>();
					animator.SetBool("Switch", true);

					first = false;
				}
			}
			if ((L_length - R_length) <= 0)
			{
				transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
				transform.rotation = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation * Quaternion.Inverse(lateL) * lateRot;

				if (first == true)
				{
					transform.GetChild(0).gameObject.transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position + offsetL;
					animator = this.gameObject.GetComponent<Animator>();
					animator.SetBool("Switch", true);

					first = false;

				}
			}
		}

		if (DorV == false)
		{
			// GetTrackingData(Head)でカメラ位置が取得できるみたい
			transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
			transform.rotation = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;


			if (first == true)
			{
				transform.GetChild(0).gameObject.transform.position = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + offset;

				animator = this.gameObject.GetComponent<Animator>();
				animator.SetBool("Switch", true);

				first = false;

			}

		}

	}


	public override void Interact()
	{

		if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}
		if (!Networking.IsOwner(Networking.LocalPlayer, transform.GetChild(0).gameObject.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, transform.GetChild(0).gameObject.gameObject);
		}

		m_owner = Networking.GetOwner(gameObject);
		m_isActive = true;

		lateOne = this.gameObject.transform.position;
		lateRot = this.gameObject.transform.rotation;
		headpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
		Lpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
		Rpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
		lateL = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;
		lateR = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
		offset = lateOne - headpos;
		offsetL = lateOne - Lpos;
		offsetR = lateOne - Rpos;
		first = true;
	}

	public void Activate()
	{

		if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
		}
		if (!Networking.IsOwner(Networking.LocalPlayer, transform.GetChild(0).gameObject.gameObject))
		{
			Networking.SetOwner(Networking.LocalPlayer, transform.GetChild(0).gameObject.gameObject);
		}

		m_owner = Networking.GetOwner(gameObject);
		m_isActive = true;

		lateOne = this.gameObject.transform.position;
		lateRot = this.gameObject.transform.rotation;
		headpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
		Lpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).position;
		Rpos = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
		lateL = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand).rotation;
		lateR = m_owner.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).rotation;
		offset = lateOne - headpos;
		offsetL = lateOne - Lpos;
		offsetR = lateOne - Rpos;
		first = true;
	}

	public void C_Switch()
	{
		transform.position = transform.GetChild(0).gameObject.transform.position;
		transform.GetChild(0).gameObject.transform.position = transform.position;
		m_isActive = false;
		animator.SetBool("Switch", false);
	}
}
