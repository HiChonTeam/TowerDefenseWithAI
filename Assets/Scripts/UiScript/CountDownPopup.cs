using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountDownPopup : MonoBehaviour
{
    private int i = 0;
    float currentTime;
    public float startingTime = 10f;
    
    [SerializeField] private TextMeshPro Keck;

    void Start(){
        currentTime = startingTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        Keck.text = currentTime.ToString("0");
        if(currentTime <= 0){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            currentTime = 0;
        }

        if(i <= 500)
        {
            transform.position += new Vector3(0f, 0.003f, 0f);
            i++;
        }
    }
}
