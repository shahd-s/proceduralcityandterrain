using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCams : MonoBehaviour
{
    public GameObject FPS, other;
    bool meow = false;
    // Start is called before the first frame update
    void Start()
    {
        meow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (meow)
        {
            FPS.SetActive(false);
            other.SetActive(true);
        }
        else
        { FPS.SetActive(true);
            other.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            meow = !meow;
        }
    }
}
