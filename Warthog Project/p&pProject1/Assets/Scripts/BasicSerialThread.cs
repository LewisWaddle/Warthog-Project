using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;

public class BasicSerialThread : MonoBehaviour
{
    // Serial port settings
    private SerialPort serialPort;
    private string comPort = "COM1";
    private int baudRate = 9600;
    private bool isInitialized = false; // prevent re-initializing this object

    // Data variables - Define the variables that you need to read in from serial
    public int currentValue;

    // Thread variables
    private Thread thread;
    private bool threadIsLooping = false;
    public byte[] byteValues;
    public bool doSendSerial = false;

    //----------------------------------------------    
    // Singleton instance 
    //----------------------------------------------
    private static BasicSerialThread _instance; // this var holds the instance reference but is private (access via the Getter)

    public static BasicSerialThread instance // this Getter is public and encapsulates the "_instance" singleton var
    {
        get
        {
            // If a singleton instance is null we must create one, otherwise
            // we return a reference to the existing one, thus ensuring there is
            // always exactly one instance.

            if (_instance == null)
            {
                GameObject go = new GameObject("BasicSerialThread_Singleton");

                // Get a reference to the component, this will be our singleton instance 
                _instance = go.AddComponent<BasicSerialThread>();

                // Prevent this object from getting unloaded/destroyed when changing scenes
                DontDestroyOnLoad(go);
            }

            // Return the instance
            return _instance;
        }

    }

    public void Init(string com, int baud)
    {
        if (!isInitialized)
        {
            print("Initialize: " + this.name);
            comPort = com;
            baudRate = baud;
            threadIsLooping = true;
            byteValues = new byte[] { 0, 0, 0, 0, 0, 0 };

            OpenPort();

            StartThread();

            isInitialized = true;
        }
    }

    //----------------------------------------------
    // Serial Port Setup
    //----------------------------------------------
    void OpenPort()
    {
        if (serialPort == null)
        {
            serialPort = new SerialPort(@"\\.\" + comPort); // format to force Unity to recognize ports beyond COM9
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            //serialPort.ReadTimeout = 5;
        }

        try
        {
            serialPort.Open();
            Debug.Log("Initialize Serial Port: " + comPort);
        }

        catch (System.Exception ex)
        {
            Debug.LogError("Error opening " + comPort + "\n" + ex.Message);
        }
    }

    void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            print("Close serialPort");
            serialPort.Close();
        }

        StopThread();
    }

    private void Update()
    {
        if (doSendSerial)
        {
            SendSerial();
        }
    }

    public void SendSerial()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write("!");
            serialPort.Write(byteValues, 0, byteValues.Length);
            serialPort.Write("#");
        }
    }

    //----------------------------------------------
    // Threading Functionality
    //----------------------------------------------
    public void StartThread()
    {
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    // Thread Loop
    public void ThreadLoop()
    {
        print("Thread start");
        while (threadIsLooping)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    string data = serialPort.ReadLine();
                    //print(data);
                    currentValue = int.Parse(data);
                }
                catch (System.Exception ex)
                {
                    print(ex.Message);
                }

            }
        }
    }

    // Stop the thread (by setting the loop bool to false, causing the thread while loop to stop.
    public void StopThread()
    {
        lock (this)
        {
            threadIsLooping = false;
        }
    }
}