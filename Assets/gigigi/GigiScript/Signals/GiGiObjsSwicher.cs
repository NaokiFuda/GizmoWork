
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GiGiObjsSwicher : UdonSharpBehaviour
{
	[SerializeField] GameObject[] Signals;
	public bool IsLocal = false;
	private bool first = true;
	public bool KillOwn = false;
	public bool Killparent = false;
	public bool OnlyOnce = false;
	public bool OnlyActive = false;
	public bool OnlyDeActive = false;


	public override void Interact()
	{
		if (IsLocal == false)
		{
			SendCustomNetworkEvent(NetworkEventTarget.All, "Activate");
		}
		if (IsLocal == true)
		{
			SendCustomEvent("Activate");
		}
	}

	public void Activate()
	{
		if (KillOwn == true)
		{
			this.gameObject.SetActive(false);
		}
		if (Killparent == true)
		{
			transform.parent.gameObject.SetActive(false);
		}

        if(Signals.Length == 0)
        {
            if (OnlyOnce && !first) return;

            if (!OnlyActive)
            {
                this.gameObject.SetActive(!this.gameObject.activeSelf);
                if (OnlyOnce) first = false;
                return;
            }

            if (OnlyDeActive) return;

            this.gameObject.SetActive(true);
        }
        else
        {
            foreach (GameObject signal in Signals)
            {
                if (OnlyOnce && !first) return;

                if (!Networking.IsOwner(Networking.LocalPlayer, signal.gameObject)) Networking.SetOwner(Networking.LocalPlayer, signal.gameObject);
                if (OnlyActive) signal.gameObject.SetActive(true);
                else if (OnlyDeActive) signal.gameObject.SetActive(false);
                else signal.gameObject.SetActive(!signal.gameObject.activeSelf);
                
                if (OnlyOnce) first = false;
            }
        }
	}

}
