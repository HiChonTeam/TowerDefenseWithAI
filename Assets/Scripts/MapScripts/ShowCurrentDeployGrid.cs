using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCurrentDeployGrid : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    private GameObject newGrid;

    private void OnMouseEnter()
    {
        if(!StatusController.isGameOver && TowerPlacement.buyingPharse && !RoundController.AIsimulate)
        {
            newGrid = Instantiate(grid, gameObject.transform.position, Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        Destroy(newGrid);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
