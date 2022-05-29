using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private LayerMask placableMask;
    [SerializeField] private LayerMask nonplacableMask;
    [SerializeField] private LayerMask towerMask;

    private GameObject selectingTower = null;

    [SerializeField] private GameObject swordsman;
    [SerializeField] private GameObject sniperMagician;
    [SerializeField] private GameObject sorcerer;
    [SerializeField] private GameObject archer;
    [SerializeField] private GameObject witch;

    private static int TowerTypeNum = 0; 
    private static bool clickbuytower = false;
    private bool buyingPharse = false;
    private bool directionPharse = false;
    private Vector2 towerPosition;

    private void Start()
    {
        buyingPharse = true;
    }

    public Vector2 GetMousePosition()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public void GetCurrentHoverTile() //check if place tower on placable tile?
    {
        Vector2 mousePosition = GetMousePosition();

        RaycastHit2D legalHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, placableMask, -100, 100);
        RaycastHit2D illegalHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, nonplacableMask, -100, 100);
        RaycastHit2D towerHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, towerMask, -100, 100);

        if(selectingTower != null)
        {
            if(legalHit.collider != null)
            {
                if(!illegalHit && !towerHit)
                {
                    Debug.Log("able to place tower");
                    towerPosition = legalHit.transform.position;
                    buyingPharse = false;
                    clickbuytower = false;
                    directionPharse = true;
                }
                else
                {
                    Debug.Log("unable to place tower");
                }
            }
        }
        else
        {
            if(towerHit)
            {
                towerHit.transform.gameObject.GetComponent<Tower>().UpgradeSkill(1);
            }
        }

    }

    public void GetCurrentDirection()
    {
        Vector2 mousePosition = GetMousePosition();

        RaycastHit2D legalHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, placableMask, -100, 100);

        if(legalHit && legalHit.transform.position.Equals(towerPosition))
        {
            // Debug.Log("cancel place");
        }
        else if(
            mousePosition.y >= towerPosition.y &&
            mousePosition.x >= towerPosition.x - (mousePosition.y - towerPosition.y) &&
            mousePosition.x <= towerPosition.x + (mousePosition.y - towerPosition.y)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, Quaternion.identity);
            SetTowerRotate(newTower, "up");
            TowerOnMap.towersOnMap.Add(newTower);
        }
        else if(
            mousePosition.y <= towerPosition.y &&
            mousePosition.x >= towerPosition.x - (towerPosition.y - mousePosition.y) &&
            mousePosition.x <= towerPosition.x + (towerPosition.y - mousePosition.y)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, Quaternion.identity);
            SetTowerRotate(newTower, "down");
            TowerOnMap.towersOnMap.Add(newTower);
        }
        else if(
            mousePosition.x <= towerPosition.x &&
            mousePosition.y <= towerPosition.y + (towerPosition.x - mousePosition.x) &&
            mousePosition.y >= towerPosition.y - (towerPosition.x - mousePosition.x)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, Quaternion.identity);
            SetTowerRotate(newTower, "left");
            TowerOnMap.towersOnMap.Add(newTower);
        }
        else if(
            mousePosition.x >= towerPosition.x &&
            mousePosition.y <= towerPosition.y + (mousePosition.x - towerPosition.x) &&
            mousePosition.y >= towerPosition.y - (mousePosition.x - towerPosition.x)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, Quaternion.identity);
            SetTowerRotate(newTower, "right");
            TowerOnMap.towersOnMap.Add(newTower);
        }
        else{
            // Debug.Log("bug found, mouse: " + mousePosition + ", tower:" + towerPosition );
        }
        buyingPharse = true;
        directionPharse = false;
        selectingTower = null;
    }

    private void SetTowerRotate(GameObject tower, string rotate)
    {
        tower.GetComponent<Tower>().SetTowerRotate(rotate);
    }

    public static void PurchaseTurrent(int TowerType){
        TowerTypeNum = TowerType;
        
        clickbuytower = true;
        // if(buyingPharse)
        // {
        //     selectingTower = swordsman;
        // }
    }   

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if(buyingPharse)
            {
                GetCurrentHoverTile();
            }
            else if(directionPharse)
            {
                GetCurrentDirection();
            }
            
        }
        if(TowerTypeNum == 1)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = swordsman;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 2)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = sniperMagician;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 3)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = sorcerer;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 4)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = archer;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 5)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = witch;
                Debug.Log(selectingTower);
            }
        }
        // if(Input.GetKeyDown(KeyCode.Alpha0))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = null;
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = swordsman;
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = sniperMagician;
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = sorcerer;
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = archer;
        //     }
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha5))
        // {
        //     if(buyingPharse)
        //     {
        //         selectingTower = witch;
        //     }
        // }
        
    }
}
