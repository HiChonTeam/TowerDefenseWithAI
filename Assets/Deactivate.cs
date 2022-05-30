using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deactivate : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject ObjectToDeactive;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            ObjectToDeactive.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.D)){
            ObjectToDeactive.SetActive(true);
        }
    }
}
