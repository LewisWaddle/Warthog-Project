using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource WarthogSource;
    public AudioClip Horn;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Joystick1Button4))
        {
            WarthogSource.Play();
            print("Button pressed");
        }

        if(Input.GetKeyUp(KeyCode.Joystick1Button4))
        {
            WarthogSource.Stop();
            print("Button released");
        }
    }
}
