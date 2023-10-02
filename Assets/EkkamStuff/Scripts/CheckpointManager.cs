using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    bool triggered = false;

    void Update()
    {
        if (triggered)
        {
            transform.parent.GetComponent<Renderer>().material.color = Color.Lerp(Color.green, Color.white, Mathf.PingPong(Time.time, 1));
        }
        else
        {
            transform.parent.GetComponent<Renderer>().material.color = Color.green;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (col.gameObject.GetComponent<PlayerController>() != null) col.gameObject.GetComponent<PlayerController>().audioSource.PlayOneShot(col.gameObject.GetComponent<PlayerController>().checkpointSound);
            col.gameObject.GetComponent<PlayerController>().SetCheckpoint(transform.position);
            triggered = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            triggered = false;
        }
    }
}
