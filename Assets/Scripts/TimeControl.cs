using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    public float time = 1;

    void Update()
    {
        Time.timeScale = time;
    }
}
