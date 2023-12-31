using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate;
    GameObject player;

    bool triggered = false;
    Collider playerCollider;

    void Start()
    {
        objectToActivate.SetActive(false);
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        if (triggered && !playerCollider)
        {
            objectToActivate.SetActive(false);
            GetComponent<Renderer>().material.color = new Color32(100, 100, 100, 255);
            triggered = false;
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            player.gameObject.GetComponent<PlayerController>().audioSource.PlayOneShot(player.gameObject.GetComponent<PlayerController>().buttonSound);
            objectToActivate.SetActive(true);
            GetComponent<Renderer>().material.color = new Color32(0, 200, 255, 255);
            triggered = true;
            playerCollider = col;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            objectToActivate.SetActive(false);
            GetComponent<Renderer>().material.color = new Color32(100, 100, 100, 255);
            triggered = false;
        }
    }
}
