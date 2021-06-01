using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioSource Music1;
    public AudioSource Music2;
    public AudioSource Music2_2;
    public AudioSource Music3;
    public AudioMixer mixer;
    public float length;
    public float currenttime1;
    public float currenttime2;
    public float currenttime3;
    public double totalTime;
    public bool Activate;
    public bool end;
    bool Switch;
    int AC = 0;
    // Start is called before the first frame update
    void Start()
    {
        end = false;
        Switch = false;
        Activate = false;
        length = Music2.clip.length;
        Music1.Play();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime = AudioSettings.dspTime;
        currenttime1 = Music1.time;
        currenttime2 = Music2.time;
        currenttime3 = Music2_2.time;
        if(currenttime1 > 25)
        {
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume1", 4.5f, 0));
        }

        if(end)
        {
            end = false;
            Music3.PlayOneShot(Music3.clip);
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume3", 15.751f, 0));
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume2", 15.751f, 0));
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume4", 15.751f, 0.5f));
            AC = 1;
        }

        if (Activate)
        {
            Activate = false;
            Music2.PlayDelayed(5);
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume2", 15.751f, 0.5f));
            //Music2_2.PlayScheduled(totalTime + length - 16.751f);
        } 

        if(currenttime2 > 109f)
        {
            if (!Switch && AC == 0)
            {
                Switch = true;
                StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume2", 9, 0));
                //Music3.PlayOneShot(Music2.clip);
                Music2_2.PlayDelayed(2);
                StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume3", 9, 0.5f));
                //Music2.PlayScheduled(totalTime + length - 16.751f);
            }
        }

        if (currenttime3 > 109f)
        {
            if (Switch && AC == 0)
            {
                Switch = false;
                StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume3", 9, 0));
                //Music2.PlayOneShot(Music2.clip);
                Music2.PlayDelayed(2);
                StartCoroutine(FadeMixerGroup.StartFade(mixer, "Volume2", 9, 0.5f));

                //Music2.PlayScheduled(totalTime + length - 16.751f);
            }
        }
    }
}
