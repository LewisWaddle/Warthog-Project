using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO.Ports;

[System.Serializable]
public struct SerialPacket
{
    // An example packet struct for organizing complex data (optional)
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
}

public class SerialThread : MonoBehaviour
{
    private static SerialThread _instance;

    // Data structures
    [SerializeField] SerialPacket currentPacket; // private variable to that holds the most recently read packet
    private Queue serialPacketQueue; // Queue that holds packets that have been successfully read and parsed
    public float[] floatValues = { 0, 0, 0, 0, 0, 0 }; // data for our motion platform {sway, surge, heave, pitch, roll, yaw)
    public byte[] byteValues = { 0, 0, 0, 0, 0, 0 }; // data for our motion platform {sway, surge, heave, pitch, roll, yaw)

    // Serial parameters
    private SerialPort serialPort;
    private string comPort = "COM8";
    private int baudRate = 9600;

    // Thread variables
    private Thread thread;
    private bool loopThread = false;

    //-------------------------------------------------------------------
    // Singleton Instantiation
    //-------------------------------------------------------------------
    public static SerialThread instance
    {
        get
        {
            // If a singleton instance is null we must create one, otherwise
            // we return a reference to the existing one, thus ensuring there is
            // always exactly one instance.

            if (_instance == null)
            {
                GameObject go = new GameObject("SerialThreadSingleton");

                // Get a reference to the component, this becomes our singleton instance 
                _instance = go.AddComponent<SerialThread>();

                // Prevent this object from getting unloaded/destroyed when changing scenes
                DontDestroyOnLoad(go);

                // Run initialization function
                _instance.Init();
            }

            // Return the instance
            return _instance;
        }
    }

    public void Wake()
    {
        print(this.name + " awake");
    }

    private void Init()
    {
        loopThread = true; // Bool for thread control loop
        serialPacketQueue = Queue.Synchronized(new Queue()); // holds data structures that have been parsed and are ready to use
    }

    //-------------------------------------------------------------------
    // Unity Methods
    //-------------------------------------------------------------------
    void Update()
    {
        //if (serialPacketQueue.Count > 0)
        //{
        //    lock (serialPacketQueue)
        //    {
        //        // By default, we're constantly dequing to get the latest data, hence the while loop.
        //        // However, in some cases you may want to examine each piece of data as it is dequeued.
        //        while (serialPacketQueue.Count > 0)
        //        {
        //            currentPacket = (SerialPacket)serialPacketQueue.Dequeue();
        //        }
        //    }
        //}
    }

    private void OnApplicationQuit()
    {
        // Try to close things down properly        
        if (serialPort != null && serialPort.IsOpen)
        {
            print("Close serialPort");
            serialPort.Close();
        }

        StopThread();
    }

    //-------------------------------------------------------------------
    // Serial Connection and Parsing
    //-------------------------------------------------------------------
    public void SetComParameters(string com, int baud)
    {
        comPort = com;
        baudRate = baud;
        OpenPort();
    }

    public void OpenPort()
    {
        if (serialPort == null)
        {
            serialPort = new SerialPort(@"\\.\" + comPort); // format to force Unity to recognize ports beyond COM9
            serialPort.BaudRate = baudRate;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.ReadTimeout = 5000;
        }

        bool success = false;
        try
        {
            serialPort.Open();
            success = true;
            Debug.Log("Initialize Serial Port: " + comPort);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error opening " + comPort + "\n" + ex.Message);
        }

        if (success) { StartThread(); }
    }

    void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    public SerialPacket GetCurrentPacket
    {
        get { return currentPacket; }
    }

    public void SendSerial()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write("!");
            //for (int i = 0; i < floatValues.Length; i++)
            //{
            //    byte[] myBytes = System.BitConverter.GetBytes(floatValues[i]);
            //    serialPort.Write(myBytes, 0, myBytes.Length);
            //}
            serialPort.Write(byteValues, 0, byteValues.Length);

            serialPort.Write("#");
        }
    }

    void ParseSerialPacket(string message)
    {
        SerialPacket serialPacket = new SerialPacket();
        string[] values = message.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries); // split the string based on the desired delimiter

        bool success = values.Length == 4 ? true : false; // check if the split message contains the expected number of elements
        if (success) success = success && int.TryParse(values[0], out serialPacket.intValue); // try parsing the value at [0] into an int
        if (success) success = success && float.TryParse(values[1], out serialPacket.floatValue); // try parsing the value at [1] into an float
        if (success) serialPacket.stringValue = values[2]; // both values are strings, so we just set it equal
        if (success)
        {
            // a string or char value of "1" or "0" isn't always explicitly equal to true or false, so we manually check
            if (values[3] == "1") { serialPacket.boolValue = true; }
            else if (values[3] == "0") { serialPacket.boolValue = false; }
            else { serialPacket.boolValue = false; success = false; }
        }

        // if all parsing was successful, we enqueue the message so that 
        // Unity can pull the message out of the queue on it's main thread Update()
        if (success)
        {
            lock (serialPacketQueue)
            {
                serialPacketQueue.Enqueue(serialPacket);
            }
        }
        else { Debug.LogError("Bad Serial Packet: " + message); }
    }

    //-------------------------------------------------------------------
    // Threading
    //-------------------------------------------------------------------
    public void StartThread()
    {
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    // Thread Loop
    public void ThreadLoop()
    {
        print("Thread start");

        // The inner while loop that runs until the program ends
        while (loopThread)
        {
            // Read (if there is data available)
            if (serialPort.BytesToRead > 0)
            {
                try
                {
                    string data = serialPort.ReadLine();
                    print(data);
                    //ParseSerialPacket(data);
                }
                catch (System.Exception ex)
                {
                    print(ex.Message);
                }
            }

            // Write Serial - The line below is commented out by default.
            // If you're sending and receiving data to the same device,
            // this may be where you want to do your sending.

            SendSerial();
        }
    }

    // Stop the thread (by setting the loop bool to false, causing the thread while loop to stop.
    public void StopThread()
    {
        lock (this)
        {
            loopThread = false;
        }
    }
}

/*
 *  alternate send serial code
    //byte[] bytesToSend = new byte[6 * 4 + 3];
    //int byteIndex = 1;
    //System.Array.Copy(System.BitConverter.GetBytes('!'), bytesToSend, 1);
    //for (int i = 0; i < floatValues.Length; i++)
    //{
    //    byte[] myBytes = System.BitConverter.GetBytes(floatValues[i]);
    //    System.Array.Copy(myBytes, 0, bytesToSend, byteIndex, 4);
    //    byteIndex += 4;
    //}
    //System.Array.Copy(new byte[] { rand }, 0, bytesToSend, byteIndex, 1);
    //byteIndex += 1;
    //System.Array.Copy(System.BitConverter.GetBytes('#'), 0, bytesToSend, byteIndex, 1);
    //serialPort.Write(bytesToSend, 0, bytesToSend.Length);
*/
