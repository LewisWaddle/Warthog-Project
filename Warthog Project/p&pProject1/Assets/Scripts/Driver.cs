using UnityEngine;
using System.Collections;

public class Driver : MonoBehaviour
{

    private WheelCollider[] wheels;

    public float maxAngle = 30;
    public float maxTorque = 3000;
    private float maxTorqueGear1 = 3000;
    private float maxTorqueGear2;
    private float maxTorqueGear3;
    private float maxTorqueGear4;
    private float maxTorqueGear5;
    public GameObject wheelShapeL;
    public GameObject wheelShapeR;
    public GameObject SteeringWheelAxis;
    public GameObject SteeringWheel;
    public float wheelStartPos;
    public Material WarthogLights;
    public Material WarthogNoLights;
    public GameObject WarthogTop;
    public GameObject WarthogBot;
    private bool Lights;
    public Light[] LightSources;
    private int gear;
    public Animator GearStick;
    public AudioSource WarthogAudio;
    public AudioClip idle;
    public AudioClip Active;
    private UniNeoPixel NeoPixel;
    bool one, two, three, four, five, six, seven, eight, reverse = false;

    [SerializeField] Camera thirdPersonCamera;
    [SerializeField] Camera firstPersonCamera;

    public float MapRange(float val, float min, float max, float newMin, float newMax)
    {
        return Mathf.Clamp(((val - min) / (max - min) * (newMax - newMin) + newMin), newMin, newMax);
        // or Y = (X-A)/(B-A) * (D-C) + C
    }

    public void Start()
    {
        NeoPixel = GameObject.Find("UniNeoPixel").GetComponent<UniNeoPixel>();
        if (Lights)
        {
            Lights = false;
            WarthogTop.GetComponent<Renderer>().material = WarthogNoLights;
            WarthogBot.GetComponent<Renderer>().material = WarthogNoLights;
        }
        else
        {
            Lights = true;
            WarthogTop.GetComponent<Renderer>().material = WarthogLights;
            WarthogBot.GetComponent<Renderer>().material = WarthogLights;
        }
        gear = 1;
        maxTorqueGear2 = maxTorque + 500;
        maxTorqueGear3 = maxTorque + 1000;
        maxTorqueGear4 = maxTorque + 1500;
        maxTorqueGear5 = maxTorque + 2000;
        firstPersonCamera.enabled = false;
        wheels = GetComponentsInChildren<WheelCollider>();
        wheelStartPos = SteeringWheel.transform.rotation.z;

        for (int i = 0; i < wheels.Length; ++i)
        {
            var wheel = wheels[i];

            // create wheel shapes only when needed
            if (wheelShapeL != null && i % 2 == 0)
            {
                var ws = GameObject.Instantiate(wheelShapeL);
                ws.transform.parent = wheel.transform;
            }
            else if (wheelShapeR != null)
            {
                var ws = GameObject.Instantiate(wheelShapeR);
                ws.transform.parent = wheel.transform;
            }
        }
    }

    // this is a really simple approach to updating wheels
    // here we simulate a rear wheel drive car and assume that the car is perfectly symmetric at local zero
    // this helps us to figure our which wheels are front ones and which are rear
    public void Update()
    {
        float wheelAngle = maxAngle * Input.GetAxis("Horizontal");
        float angle = Input.GetAxis("Horizontal");
        float torque = maxTorque * Input.GetAxis("Vertical");

        //print(Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 5, transform.localPosition.z);
            transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button3) || Input.GetKeyDown(KeyCode.L))
        {
            if (Lights)
            {
                Lights = false;
                WarthogTop.GetComponent<Renderer>().material = WarthogNoLights;
                WarthogBot.GetComponent<Renderer>().material = WarthogNoLights;
                for (int i = 0; i < LightSources.Length; i++)
                {
                    LightSources[i].enabled = false;
                }
                SetPixel(10, false);
            }
            else
            {
                Lights = true;
                WarthogTop.GetComponent<Renderer>().material = WarthogLights;
                WarthogBot.GetComponent<Renderer>().material = WarthogLights;
                for (int i = 0; i < LightSources.Length; i++)
                {
                    LightSources[i].enabled = true;
                }
                SetPixel(10, true);
            }

        }
        print(Input.GetAxis("Vertical"));
        if (Input.GetAxis("Vertical") < 0 && !reverse)
        {
            SetPixel(-1, true);
            reverse = true;
            
        }
        else if (Input.GetAxis("Vertical") >= 0 && reverse)
        {
            SetPixel(-1, false);
            reverse = false;
        }

        if (Input.GetAxis("Vertical") >= 0.0625f && !one)
        {
            SetPixel(0, true);
            one = true;
        }
        else if(Input.GetAxis("Vertical") < 0.0625f && one)
        {
            SetPixel(0, false);
            SetPixel(9, false);
            one = false;
        }
        if (Input.GetAxis("Vertical") >= 0.125f && !two)
        {
            SetPixel(1, true);
            two = true;
        }
        else if(Input.GetAxis("Vertical") < 0.125f && two)
        {
            SetPixel(1, false);
            two = false;
        }

        if (Input.GetAxis("Vertical") >= 0.1875f && !three)
        {
            SetPixel(2, true);
            three = true;
        }
        else if(Input.GetAxis("Vertical") < 0.1875f && three)
        {
            SetPixel(2, false);
            three = false;
        }
        if (Input.GetAxis("Vertical") >= 0.25f && !four)
        {
            SetPixel(3, true);
            four = true;
        }
        else if (Input.GetAxis("Vertical") < 0.25f && four)
        {
            SetPixel(3, false);
            four = false;
        }
        if (Input.GetAxis("Vertical") >= 0.3125f && !five)
        {
            SetPixel(4, true);
            five = true;
        }
        else if (Input.GetAxis("Vertical") < 0.3125f && five)
        {
            SetPixel(4, false);
            five = false;
        }

        if (Input.GetAxis("Vertical") >= 0.375f && !six)
        {
            SetPixel(5, true);
            six = true;
        }
        else if (Input.GetAxis("Vertical") < 0.375f && six)
        {
            SetPixel(5, false);
            six = false;
        }

        if (Input.GetAxis("Vertical") >= 0.4375f && !seven)
        {
            SetPixel(6, true);
            seven = true;
        }
        else if (Input.GetAxis("Vertical") < 0.4375f && seven)
        {
            SetPixel(6, false);
            seven = false;
        }

        if (Input.GetAxis("Vertical") >= 0.5f && !eight)
        {
            SetPixel(7, true);
            eight = true;
        }
        else if (Input.GetAxis("Vertical") < 0.5f && eight)
        {
            SetPixel(7, false);
            eight = false;
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetAxis("Vertical") >= 0.4)
            {

                gear++;
                switch (gear)
                {
                    case 1:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(1);
                        maxTorque = maxTorqueGear1;
                        break;

                    case 2:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(2);
                        maxTorque = maxTorqueGear2;
                        break;

                    case 3:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(3);
                        maxTorque = maxTorqueGear3;
                        break;

                    case 4:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(4);
                        maxTorque = maxTorqueGear4;
                        break;

                    case 5:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(5);
                        maxTorque = maxTorqueGear5;
                        break;

                    default:
                        gear--;
                        break;
                }
            }
            else if (Input.GetAxis("Vertical") == 0)
            {

                gear--;
                switch (gear)
                {
                    case 1:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(1);
                        maxTorque = maxTorqueGear1;
                        break;

                    case 2:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(2);
                        maxTorque = maxTorqueGear2;
                        break;

                    case 3:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(3);
                        maxTorque = maxTorqueGear3;
                        break;

                    case 4:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(4);
                        maxTorque = maxTorqueGear4;
                        break;

                    case 5:
                        GearStick.SetTrigger("Shift");
                        NeoPixel.lcdDisplay(5);
                        maxTorque = maxTorqueGear5;
                        break;

                    default:
                        gear++;
                        break;
                }
            }
        }

        foreach (WheelCollider wheel in wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = wheelAngle;
            }
            else
            {
                wheel.steerAngle = -wheelAngle / 4;
            }
            //Mathf.Clamp(SteeringWheel.transform.localRotation.x, 0, 0);
            //Mathf.Clamp(SteeringWheel.transform.localRotation.y, 0, 0);
            //SteeringWheel.transform.Rotate(transform.forward, -angle * Time.deltaTime);

            Quaternion wheelRot = Quaternion.AngleAxis(-angle * 80, Vector3.forward);
            SteeringWheel.transform.localRotation = wheelRot;
            WarthogAudio.pitch = Input.GetAxis("Vertical") * 2;
            if (torque > 0)
            {
                if (WarthogAudio.clip != Active || !WarthogAudio.isPlaying)
                {
                    WarthogAudio.Stop();
                    WarthogAudio.volume = 0.3f;
                    WarthogAudio.clip = Active;
                    WarthogAudio.PlayOneShot(Active);
                }

            }
            else if (torque == 0)
            {
                WarthogAudio.pitch = 1;
                if (WarthogAudio.clip != idle || !WarthogAudio.isPlaying)
                {
                    WarthogAudio.Stop();
                    WarthogAudio.volume = 1;
                    WarthogAudio.clip = idle;
                    WarthogAudio.PlayOneShot(idle);
                }

            }
            wheel.motorTorque = torque;


            // update visual wheels if any
            if (wheelShapeL || wheelShapeR)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                // assume that the only child of the wheelcollider is the wheel shape
                Transform shapeTransform = wheel.transform.GetChild(0);
                shapeTransform.position = p;
                shapeTransform.rotation = q;
            }

        }
    }

    public void SetPixel(int LEDnum, bool On)
    {
            if (LEDnum < 2)
            {
                if (On)
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "111 111 0");
                }
                else
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "0 0 0");
                }
            }
            else if (LEDnum > 1 && LEDnum < 5)
            {
                if (On)
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "111 0 0");
                }
                else
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "0 0 0");
                }
            }
            else if (LEDnum > 4 && LEDnum < 8)
            {
                if (On)
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "0 0 111");
                }
                else
                {
                    NeoPixel.SetPixelColor(LEDnum + 8, "0 0 0");
                }
            }

            if(LEDnum == 9)
        {
            for (int i = 0; i < 8; i++)
            {
                 NeoPixel.SetPixelColor(i + 8, "0 0 0");
            }
        }
       





        if (LEDnum == 10)
        {
            if (On)
            {
                NeoPixel.SetPixelColor(25, "255 255 255");
                NeoPixel.SetPixelColor(30, "255 255 255");
            }
            else
            {
                NeoPixel.SetPixelColor(25, "0 0 0");
                NeoPixel.SetPixelColor(30, "0 0 0");
            }

        }

        if (LEDnum == -1)
        {
            if (On)
            {
                NeoPixel.SetPixelColor(27, "255 0 0");
                NeoPixel.SetPixelColor(28, "255 0 0");
            }
            else
            {
                NeoPixel.SetPixelColor(27, "0 0 0");
                NeoPixel.SetPixelColor(28, "0 0 0");
            }
        }
    }
}
