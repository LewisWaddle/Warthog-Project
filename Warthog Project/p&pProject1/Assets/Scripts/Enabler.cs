using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enabler : MonoBehaviour
{
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
        if(other.gameObject.CompareTag("FloorPanel"))
        {
            for (int i = 0; i < other.gameObject.transform.childCount; i++)
            {
                var child = other.gameObject.transform.GetChild(i).gameObject;
                if (child != null || child.activeSelf == false)
                    child.SetActive(true);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("FloorPanel"))
        {
            for (int i = 0; i < other.gameObject.transform.childCount; i++)
            {
                GameObject child = other.gameObject.transform.GetChild(i).gameObject;
                if (child != null || child.activeSelf == true)
                    child.SetActive(false);
            }
        }
    }
}
