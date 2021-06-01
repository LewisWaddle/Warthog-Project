using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public GameObject warthog;
    public UniNeoPixel UduinoBoard;
    public float timeRemaining = 200;
    public bool end = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(warthog.transform.position.x, transform.position.y, warthog.transform.position.z);
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, warthog.transform.position.y + 4, transform.position.z), Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, warthog.transform.rotation.y, transform.rotation.z, warthog.transform.rotation.w), Time.deltaTime);
        if(timeRemaining > 0 && !end)
        {
            timeRemaining -= Time.deltaTime;
            UduinoBoard.lcdDisplay((int)timeRemaining);
        }

    }
}
