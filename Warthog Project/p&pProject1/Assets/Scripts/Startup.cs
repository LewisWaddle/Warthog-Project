using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public string port = "COM7";
    public int baudRate = 9600;

	void Start ()
    {
        PlatformController.singleton.Init(port, baudRate);
        //BasicSerialThread.instance.Init(port, baudRate);
        //SerialThread.instance.SetComParameters(port, baudRate);
	}
}
