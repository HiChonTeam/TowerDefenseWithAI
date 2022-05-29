using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPopup : MonoBehaviour
{
    private int i = 0;

    public void AcceptGameOver(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

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
