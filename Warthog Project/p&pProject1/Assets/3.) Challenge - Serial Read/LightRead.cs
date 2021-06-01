using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRead : MonoBehaviour
{
    [Range(0.0f, 100.0f)]
    public float rangeValue;

    Light light;

    void Start()
    {
        light = this.GetComponent<Light>();
        BasicSerialThread.instance.Init("COM5", 9600);
    }

    void Update()
    {
        light.range = BasicSerialThread.instance.currentValue / 1023.0f * 30;
    }
}
