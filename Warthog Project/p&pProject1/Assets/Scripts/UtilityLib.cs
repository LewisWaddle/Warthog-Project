using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class UtilityLib : MonoBehaviour
{
    //-------------------------------------------------------------------------
    /// Audio
    //-------------------------------------------------------------------------

    public static AudioSource PlaySound(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, float spatialBlend = 1f)
    {
        if (clip == null)
        {
            return null;
        }

        GameObject go = new GameObject("One-shot audio");
        go.transform.position = Camera.main.transform.position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = loop;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = spatialBlend;
        source.Play();
        GameObject.Destroy(go, clip.length);
        return source;
    }

    //-------------------------------------------------------------------------
    /// Interpolation
    //-------------------------------------------------------------------------

    public static float Lerp(float a, float b, float t)
    {
        return (a * (1.0f - t)) + (b * t);
    }

    public static float SmoothStep(float edge0, float edge1, float x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        // Evaluate polynomial
        return x * x * (3 - 2 * x);
    }

    //-------------------------------------------------------------------------
    /// Math Operations
    //-------------------------------------------------------------------------

    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static float MapRange(float val, float min, float max, float newMin, float newMax)
    {
        return ((val - min) / (max - min) * (newMax - newMin) + newMin);
        // or Y = (X-A)/(B-A) * (D-C) + C
    }    

    // Utility function for converting float time to formatted string for time output
    public static string FloatToTimeString(float time, bool showMinutes = true, int numberOfDecimals = 0)
    {
        string output = "";
        if (showMinutes)
        {
            float min = Mathf.Floor(time / 60f);
            string mins = (Mathf.FloorToInt(min) < 10 ? "0" : "") + min.ToString("F0");
            output = mins;
        }

        float sec = time % 60f;
        string secs = (Mathf.FloorToInt(sec) < 10 ? ":0" : ":") + sec.ToString("F" + numberOfDecimals.ToString("F0"));
        output += secs;
        return output;
    }
    
    public float GetAngle(Vector3 v1, Vector3 v2)
    {
        float ang = Vector3.Angle(v1, -v2);
        Vector3 cross = Vector3.Cross(v1, -v2);

        if (cross.z > 0)
            ang = 360 - ang;

        return ang;
    }
}
