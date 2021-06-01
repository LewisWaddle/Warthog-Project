using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public BGMManager AudioManager;
    public AudioSource BGMMusic;
    [SerializeField] GameObject warthog;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Body")
        {
            gameObject.SetActive(false);
            warthog = other.gameObject;
            //BGMMusic.Play();
            AudioManager.Activate = true;
        }
    }
}
