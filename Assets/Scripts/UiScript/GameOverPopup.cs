using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    private int i = 0;

    // Update is called once per frame
    private void Update()
    {
        if(i <= 500)
        {
            transform.position += new Vector3(0f, 0.003f, 0f);
            i++;
        }
    }
}
