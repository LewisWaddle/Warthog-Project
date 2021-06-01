using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class UnitySerial : MonoBehaviour
{
    SerialPort serialPort;
    public Light light;

    void Start()
    {
        serialPort = new SerialPort();
        serialPort.BaudRate = 9600;
        serialPort.PortName = "COM8";
        //serialPort.ReadTimeout = 5;

        try
        {
            serialPort.Open();
            print("portOpen");
        }
        catch (System.Exception ex)
        {
            print(ex.Message);
        }
    }

    void Update()
    {
        try
        {
            string msg = serialPort.ReadLine();
            print(msg);
            //float value;
            //bool success = float.TryParse(msg, out value);
            //if (success)
            //{
            //    light.range = (value / 1023.0f) * 50;
            //}
        }
        catch (System.Exception ex)
        {

        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
