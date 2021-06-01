using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class UniNeoPixel : MonoBehaviour {

    public static UniNeoPixel Instance;

    public int numberLeds;
    [Range(0,255)]
    public int globalLuminosity;
    public GameObject pixelPrefab;
    public GameObject ledsHolder;

    public bool useHover = false;

    void Awake()
    {
        Instance = this;
        for (int i=0; i< numberLeds; i++)
        {
            GameObject pixel = GameObject.Instantiate(pixelPrefab);
            pixel.transform.SetParent(ledsHolder.transform);
            pixel.transform.name = i.ToString();
        }
    }

	void Update ()
    {
		
	}

    public void SetPixelColor(int pixel, string colorCode, bool on = true)
    {
        string colorString;
        if (colorCode == null)
        {
            colorString = Mathf.Round(CUIColorPicker.Instance.Color.r * globalLuminosity) + " " + Mathf.Round(CUIColorPicker.Instance.Color.g * globalLuminosity) + " " + Mathf.Round(CUIColorPicker.Instance.Color.b * globalLuminosity);
            if (on == false)
                colorString = "0 0 0";
        }
        else
        {
            colorString = colorCode;
        }


        UduinoManager.Instance.Write("pixels", "SetPixel " + pixel + " " + colorString );
    }

    public void lcdDisplay(int input)
    {
        UduinoManager.Instance.Write("pixels", "changeText " + input);
    }

    public void initialize()
    {
        UduinoManager.Instance.Write("pixels", "startLCD");
    }
}
