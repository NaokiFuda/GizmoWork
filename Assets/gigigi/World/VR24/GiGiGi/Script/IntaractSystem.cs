using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntaractSystem : MonoBehaviour
{
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] GameObject main_camera;
    [SerializeField, Tooltip("ÉJÉÅÉâÇÃèâä˙à íu")] private Vector3 _cameraDefPos = new Vector3(0.0f, 0.5f, -1.3f);
    bool first = true;
    Vector3 respawnPoint;

    private void Start()
    {
        respawnPoint = player2.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attack")
        {
            if (first)
            {
                player1.GetComponent<PlayerControler>().enabled = false;
                player2.SetActive(true);
                main_camera.SetActive(false);
                
                first = false;
            }
            else
            {
                player2.transform.position = respawnPoint;
                main_camera.SetActive(true);
                player1.GetComponent<PlayerControler>().enabled = true;
                player2.SetActive(false);
                
                first = true;
            }
        }

    }
}
