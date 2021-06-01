using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    private UniNeoPixel NeoPixel;
    // Start is called before the first frame update
    void Start()
    {
        NeoPixel = GameObject.Find("UniNeoPixel").GetComponent<UniNeoPixel>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //NeoPixel.lcdDisplay("Hello World!");
    }
}
