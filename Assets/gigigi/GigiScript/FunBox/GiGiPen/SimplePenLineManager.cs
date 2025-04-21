
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SimplePenLineManager : UdonSharpBehaviour
{
    [SerializeField]GameObject[] lines = new GameObject[500];

    public override void OnPickupUseDown()
    {
        if (lines[lines.Length-1] != null) 
        {
            for (int i = 0; i< lines.Length; i++)
            {
                if (i < lines.Length - 2)
                    CreateLine(lines[i], i + 1);
                    
                else
                    Destroy(lines[i]);
                
            }
        }
        else
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line == null) { CreateLine(line, i-1);  break; }
            }
        }

    }
    void CreateLine(GameObject line , int i)
    {
        line = Instantiate(lines[i], lines[i].transform.parent);
    }
}
