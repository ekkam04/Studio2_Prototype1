using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionController : MonoBehaviour
{
    public float timeSpeed = 1.5f;

    public KeyCode abilityTrigger;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(abilityTrigger))
        {
            SlowMotion();
        }
    }

    void SlowMotion()
    {
        timeSpeed = 0;
    }

}
