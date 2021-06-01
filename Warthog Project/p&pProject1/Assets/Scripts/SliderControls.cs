
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

public class SliderControls : MonoBehaviour
{
    public static SliderControls instance;
    public Slider[] sliders; // list of references to the sliders
    public float[] DOFMins;
    public float[] DOFMaxs;
    public float[] DOFHomes;
    public Text[] textFields;
    PlatformController platformRef;

    void Awake()
    {
        instance = this; // static reference to the most recent instance of this class (lazy singleton)
    }

    public void SetSliderParameters(PlatformController _platformRef)
    {
        platformRef = _platformRef;
        // Unity's sliders need to know the min and max values.
        // The values for 8 bit mode will always been from 0 to 255        
        if (platformRef.mode == PlatformController.PlatformModes.Mode_8Bit)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = true;
                sliders[i].minValue = byte.MinValue;
                sliders[i].maxValue = byte.MaxValue;
            }
        }
        else if (platformRef.mode == PlatformController.PlatformModes.Mode_16Bit)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = true;
                sliders[i].minValue = short.MinValue;
                sliders[i].maxValue = short.MaxValue-1;
            }
        }
        else if (platformRef.mode == PlatformController.PlatformModes.Mode_32Bit)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = true;
                sliders[i].minValue = int.MinValue;
                sliders[i].maxValue = int.MaxValue-1;
            }
        }
        // The values for 32 bit mode are the real-world position and rotation values             
        // position values are in millimeters, rotation values are in degrees
        // Most Stewart platforms can not move more than 30 mm or rotate more than 30 degrees on any axis
        else if (platformRef.mode == PlatformController.PlatformModes.Mode_Float)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].wholeNumbers = false;
                sliders[i].minValue = DOFMins[i];
                sliders[i].maxValue = DOFMaxs[i];
            }
        }

        ResetSliders();
    }

    void Update()
    {
        for (int i = 0; i < textFields.Length; i++)
        {
            if (textFields[i] != null) { textFields[i].text = sliders[i].value.ToString(sliders[i].wholeNumbers ? "F0" : "F2"); }
        }
    }

    public void ResetSliders()
    {
        for (int i = 0; i < SliderControls.instance.sliders.Length; i++)
        {
            // set everything to "middle" values (except 8 bit mode, since a byte is always unsigned)
            SliderControls.instance.sliders[i].value = platformRef.mode == PlatformController.PlatformModes.Mode_8Bit ? 128 : 0;
        }
    }
}
