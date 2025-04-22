
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EnterSignal : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signals;
    [SerializeField] string signalName = "Activate";
    [SerializeField] string exitSignalName = "DeActivate";
    [SerializeField] string animParameterName = "active";
    [SerializeField] bool OnlyEnter = false;
	[SerializeField] bool OnlyExit = false;
	[SerializeField] bool TargetHasNoReciver = false;
	[SerializeField] bool ExitActivate = false;
    [SerializeField] bool ExitDeActivate = false;
    [SerializeField] bool OnlyOnce = false;
	[SerializeField] bool AllwaysActivateOn = false;
	[SerializeField] bool AllwaysActivateOFF = false;
	[SerializeField] bool Killself = false;
    bool Locker = false;

    UdonBehaviour[] _chashedClass;

	public override void OnPlayerTriggerEnter(VRCPlayerApi player)
	{
        if (!TargetHasNoReciver)
        {
            if (_chashedClass == null)
            {
                _chashedClass = new UdonBehaviour[Signals.Length];
                _chashAnimator = new Animator[Signals.Length];
            }
            for (int i = 0; i < _chashedClass.Length; i++)
            {
                _chashedClass[i] = Signals[i].GetComponent<UdonBehaviour>();
                _chashAnimator[i] = Signals[i].GetComponent<Animator>();
            }
        }

        if (Locker || OnlyExit || player != Networking.LocalPlayer) { return; }

        if (Signals.Length == 0 || TargetHasNoReciver)
		{
			Activate();
		}
        else
        {
            SendSignals();
        }

		if (OnlyOnce) { Locker=true; }
	}

    void SendSignals()
    {
        foreach (UdonBehaviour send in _chashedClass)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, send.gameObject)) Networking.SetOwner(Networking.LocalPlayer, send.gameObject);

            send.SendCustomEvent(signalName);
        }
        if (Killself) gameObject.SetActive(false);
    }

	public override void OnPlayerTriggerExit(VRCPlayerApi player)
	{
		if(OnlyEnter|| Locker|| player != Networking.LocalPlayer) return; 
		
		if(ExitActivate)
		{
			if (Signals.Length == 0 || TargetHasNoReciver)
			{
				Activate();
			}
            if (!TargetHasNoReciver)
            {
                SendSignals();
            }
        }

        if (!ExitDeActivate) return;

        if(!TargetHasNoReciver)
        {
            foreach (UdonBehaviour send in _chashedClass)
            {
                if (!Networking.IsOwner(Networking.LocalPlayer, send.gameObject)) Networking.SetOwner(Networking.LocalPlayer, send.gameObject);

                send.SendCustomEvent(exitSignalName);
            }
            if (Killself) gameObject.SetActive(false);
        }
		
		if (OnlyOnce) { Locker=true; }
	}

   void SetAnimator(Animator target ,bool value)
    {
        target.SetBool(animParameterName, value);
        if (Killself) gameObject.SetActive(false);
    }

    Animator[] _chashAnimator;
    public void Activate()
	{
        if (AllwaysActivateOn)
        {
            foreach(Animator anim in _chashAnimator)
            SetAnimator(anim, true);
		}
		else if (AllwaysActivateOFF)
		{
            foreach (Animator anim in _chashAnimator)
                SetAnimator(anim, false);
        }
		else
		{
            foreach (Animator anim in _chashAnimator)
                SetAnimator(anim, !anim.GetBool(animParameterName));
        }

	}
}
