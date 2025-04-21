
using UdonSharp;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common;

public class TextSwitch : UdonSharpBehaviour
{
    Animator animator ;

    private void Start()
    {
        animator = transform.parent.GetComponent<Animator>();
    }
    public override void InputDrop(bool value, UdonInputEventArgs args) 
    {
        if (!animator.GetBool("Switch")){ return; }
        else 
        {
            UdonBehaviour TextMove = (UdonBehaviour)transform.parent.GetComponent(typeof(UdonBehaviour));
            TextMove.SendCustomEvent("C_Switch");
        }
        
    }

    public override void Interact()
	{
			UdonBehaviour TextMove = (UdonBehaviour)transform.parent.GetComponent(typeof(UdonBehaviour));
			TextMove.SendCustomEvent("C_Switch");
	}
}
