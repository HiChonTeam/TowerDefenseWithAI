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
        Debug.Log(type);
        textMesh.SetText(appearText);
    }

    // Update is called once per frame
    private void Update()
    {
        if(i >= 300)
        {
            Destroy(gameObject);
        }
        if(textMesh != null)
        {
            textColor.a -= 0.0033f;
            textMesh.color = textColor;
        }
        float m = (i >= 150 ? -1.0f : 2.0f);
        transform.position += new Vector3(0.005f * ((450f - i) / 300f), 0.005f * m, 0);
        i += 1;
    }
}
