using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public BGMManager AudioManager;
    public CameraControls cameraTimer;
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
        if (other.gameObject.name == "Body")
        {
            cameraTimer.end = true;
            gameObject.SetActive(false);
            warthog = other.gameObject;
            AudioManager.end = true;
        }
    }
}
