using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Color textColor;
    private int i = 0;

    public void Setup(int damage, string type)
    {
        textMesh = GetComponent<TextMeshPro>();
        string appearText = "";
        if(type == "instantDeath")
        {
            appearText = "Instant Death!";
            textColor = Color.black;
        }
        else if(type == "barrier")
        {
            appearText = damage + " Barrier";
            textColor = Color.blue;
        }
        else if(type == "resist")
        {
            appearText = "Resist";
            textColor = Color.gray;
        }
        else if(type == "break")
        {
            appearText = damage + " Break";
            textColor = Color.yellow;
        }
        else if(type == "true")
        {
            appearText = damage + " True";
            textColor = Color.red;
        }
        else if(type == "burn")
        {
            appearText = damage + " Burn";
            textColor = Color.red;
        }
        else
        {
            appearText = damage.ToString();
            textColor = textMesh.color;
        }
        textMesh.SetText(appearText);
    }

    // Update is called once per frame
    private void Update()
    {
        if(i >= 200)
        {
            Destroy(gameObject);
        }
        textColor.a -= 0.001f;
        textMesh.color = textColor;
        transform.position += new Vector3(0, 0.005f, 0);
        i += 1;
    }
}
