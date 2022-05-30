using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public Texture2D swordsmanCursor;
    public Texture2D sniperMagicianCursor;
    public Texture2D sorcererCursor;
    public Texture2D archerCursor;
    public Texture2D witchCursor;
    public Vector2 hotSpot = new Vector2(120 / 2f, 200);
    [SerializeField] private Camera cam;

    [SerializeField] private LayerMask placableMask;
    [SerializeField] private LayerMask nonplacableMask;
    [SerializeField] private LayerMask towerMask;

    [SerializeField] private List<GameObject> buttonList; 

    private GameObject selectingTower = null;

    [SerializeField] private GameObject swordsman;
    [SerializeField] private GameObject sniperMagician;
    [SerializeField] private GameObject sorcerer;
    [SerializeField] private GameObject archer;
    [SerializeField] private GameObject witch;

    private static int TowerTypeNum = 0; 
    private static bool clickbuytower = false;
    private static bool buyingPharse = false;
    private bool directionPharse = false;
    private Vector2 towerPosition;
    public static int buyingCost = 0;

    private void Start()
    {
        // Cursor.visible = false;
        //Cursor.SetCursor(PictureCursor,Vector2.zero, CursorMode.ForceSoftware);
        buyingPharse = true;
        foreach(GameObject upgradeButton in buttonList)
        {
            upgradeButton.GetComponent<SkillButtonClick>().setupButton(this);
        }
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
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    Debug.Log("unable to place tower");
                    buyingPharse = false;
                    clickbuytower = false;
                }
            }
        }
        else
        {
            if(towerHit)
            {
                buyingPharse = false;
                clickbuytower = false;
            }
        }
    }

    public void PrepareUpgradeTower()
    {
        Vector2 mousePosition = GetMousePosition();

        RaycastHit2D towerHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, towerMask, -100, 100);
        RaycastHit2D UIHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 1.0f);
        RaycastHit2D legalHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, placableMask, -100, 100);

        if(towerHit)
        {
            bool switchShop = gameObject.GetComponent<HidePanel>().switchShop();
            if(switchShop)
            {
                gameObject.GetComponent<HidePanel>().showUpgrade(towerHit.transform.gameObject.GetComponent<Tower>());
                Tower towerObj = towerHit.transform.gameObject.GetComponent<Tower>();
                int[] skillUpgraded =  towerObj.GetAllSkillUpgraded();
                int mainSkill = -1;
                int secondSkill = -1;
                bool isMainEqualSecond = false;
                for(int i = 0; i < 3; i++)
                {
                    if(skillUpgraded[i] >= 1 && mainSkill == -1)
                    {
                        mainSkill = i;
                    }
                    else if(skillUpgraded[i] > 2 && mainSkill != -1)
                    {
                        secondSkill = mainSkill;
                        mainSkill = i;
                    }
                    else if(skillUpgraded[i] >= 1 && mainSkill != -1)
                    {
                        secondSkill = i;
                    } 
                }
                if(mainSkill != -1 && secondSkill != -1)
                {
                    if(skillUpgraded[mainSkill] <= 2 && skillUpgraded[secondSkill] <= 2)
                    {
                        isMainEqualSecond = true; 
                    }
                }
                for(int i = 0; i < 3; i++)
                {
                    if(mainSkill == -1 || mainSkill == i || (isMainEqualSecond && (mainSkill == i || secondSkill == i)))
                    {
                        if(skillUpgraded[i] < 4)
                        {
                            string name = towerObj.GetSkillName(i, skillUpgraded[i]);
                            string desc = towerObj.GetSkillDetail(i, skillUpgraded[i]);
                            int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i]);
                            buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, (skillUpgraded[i] + 1).ToString(), towerObj);
                        }
                        else
                        {
                            string name = towerObj.GetSkillName(i, skillUpgraded[i] - 1);
                            string desc = towerObj.GetSkillDetail(i, skillUpgraded[i] - 1);
                            int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i] - 1);
                            buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
                        }
                    }
                    else if(secondSkill == -1 || secondSkill == i)
                    {
                        if(skillUpgraded[i] < 2)
                        {
                            string name = towerObj.GetSkillName(i, skillUpgraded[i]);
                            string desc = towerObj.GetSkillDetail(i, skillUpgraded[i]);
                            int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i]);
                            buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, (skillUpgraded[i] + 1).ToString(), towerObj);
                        }
                        else
                        {
                            string name = towerObj.GetSkillName(i, skillUpgraded[i] - 1);
                            string desc = towerObj.GetSkillDetail(i, skillUpgraded[i] - 1);
                            int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i] - 1);
                            buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
                        }
                    }
                    else
                    {
                        string name = towerObj.GetSkillName(i, 0);
                        string desc = towerObj.GetSkillDetail(i, 0);
                        int skillCost = towerObj.GetSkillCost(i, 0);
                        buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
                    }
                }
            }
        }
        else if(UIHit)
        {
            gameObject.GetComponent<HidePanel>().switchShop();
        }
    }

    public void upgradeTower(Tower towerObj, int skillNumber)
    {
        StatusController.userMoney -= towerObj.GetSkillCost(skillNumber, towerObj.GetSkillUpgraded(skillNumber));
        towerObj.GetComponent<Tower>().UpgradeSkill(skillNumber);
        int[] skillUpgraded =  towerObj.GetAllSkillUpgraded();
        int mainSkill = -1;
        int secondSkill = -1;
        bool isMainEqualSecond = false;
        for(int i = 0; i < 3; i++)
        {
            if(skillUpgraded[i] >= 1 && mainSkill == -1)
            {
                mainSkill = i;
            }
            else if(skillUpgraded[i] > 2 && mainSkill != -1)
            {
                secondSkill = mainSkill;
                mainSkill = i;
            }
            else if(skillUpgraded[i] >= 1 && mainSkill != -1)
            {
                secondSkill = i;
            } 
        }
        if(mainSkill != -1 && secondSkill != -1)
        {
            if(skillUpgraded[mainSkill] <= 2 && skillUpgraded[secondSkill] <= 2)
            {
                isMainEqualSecond = true; 
            }
        }
        
        for(int i = 0; i < 3; i++)
        {
            if(mainSkill == -1 || mainSkill == i || (isMainEqualSecond && (mainSkill == i || secondSkill == i)))
            {
                if(skillUpgraded[i] < 4)
                {
                    string name = towerObj.GetSkillName(i, skillUpgraded[i]);
                    string desc = towerObj.GetSkillDetail(i, skillUpgraded[i]);
                    int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i]);
                    buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, (skillUpgraded[i] + 1).ToString(), towerObj);
                }
                else
                {
                    string name = towerObj.GetSkillName(i, skillUpgraded[i] - 1);
                    string desc = towerObj.GetSkillDetail(i, skillUpgraded[i] - 1);
                    int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i] - 1);
                    buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
                }
            }
            else if(secondSkill == -1 || secondSkill == i)
            {
                if(skillUpgraded[i] < 2)
                {
                    string name = towerObj.GetSkillName(i, skillUpgraded[i]);
                    string desc = towerObj.GetSkillDetail(i, skillUpgraded[i]);
                    int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i]);
                    buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, (skillUpgraded[i] + 1).ToString(), towerObj);
                }
                else
                {
                    string name = towerObj.GetSkillName(i, skillUpgraded[i] - 1);
                    string desc = towerObj.GetSkillDetail(i, skillUpgraded[i] - 1);
                    int skillCost = towerObj.GetSkillCost(i, skillUpgraded[i] - 1);
                    buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
                }
            }
            else
            {
                string name = towerObj.GetSkillName(i, 0);
                string desc = towerObj.GetSkillDetail(i, 0);
                int skillCost = towerObj.GetSkillCost(i, 0);
                buttonList[i].GetComponent<SkillButtonClick>().setSkill(name, skillCost, desc, "max", towerObj);
            }
        }
    }

    public void GetCurrentDirection()
    {

        Vector2 mousePosition = GetMousePosition();

        RaycastHit2D legalHit = Physics2D.Raycast(mousePosition, new Vector2(0, 0), 0.1f, placableMask, -100, 100);

        if(legalHit && legalHit.transform.position.Equals(towerPosition))
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else if(
            mousePosition.y <= towerPosition.y &&
            mousePosition.x >= towerPosition.x - (towerPosition.y - mousePosition.y) &&
            mousePosition.x <= towerPosition.x + (towerPosition.y - mousePosition.y)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, new Quaternion(0, 180 , 0, 1));
            SetTowerRotate(newTower, "down");
            TowerOnMap.towersOnMap.Add(newTower);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else if(
            mousePosition.x <= towerPosition.x &&
            mousePosition.y <= towerPosition.y + (towerPosition.x - mousePosition.x) &&
            mousePosition.y >= towerPosition.y - (towerPosition.x - mousePosition.x)
        )
        {
            GameObject newTower = Instantiate(selectingTower, towerPosition, new Quaternion(0, 180 , 0, 1));
            SetTowerRotate(newTower, "left");
            TowerOnMap.towersOnMap.Add(newTower);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else{
            // Debug.Log("bug found, mouse: " + mousePosition + ", tower:" + towerPosition );
        }
        StatusController.userMoney -= buyingCost;
        buyingCost = 0;
        buyingPharse = false;
        directionPharse = false;
        selectingTower = null;
    }

    private void SetTowerRotate(GameObject tower, string rotate)
    {
        tower.GetComponent<Tower>().SetTowerRotate(rotate);
    }

    public static void PurchaseTurrent(int TowerType, int cost){
        TowerTypeNum = TowerType;
        buyingCost = cost;
        buyingPharse = true;
        clickbuytower = true;
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
            else
            {
                PrepareUpgradeTower();
            }
            
        }
        if(TowerTypeNum == 1)
        {
            if(buyingPharse && clickbuytower)
            {
                selectingTower = swordsman;
                Cursor.SetCursor(swordsmanCursor,hotSpot , CursorMode.ForceSoftware);
                TowerTypeNum = 0;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 2)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = sniperMagician;
                Cursor.SetCursor(sniperMagicianCursor,hotSpot , CursorMode.ForceSoftware);
                TowerTypeNum = 0;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 3)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = sorcerer;
                Cursor.SetCursor(sorcererCursor,hotSpot , CursorMode.ForceSoftware);
                TowerTypeNum = 0;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 4)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = archer;
                Cursor.SetCursor(archerCursor,hotSpot , CursorMode.ForceSoftware);
                TowerTypeNum = 0;
                Debug.Log(selectingTower);
            }
        }
        if(TowerTypeNum == 5)
        {

            if(buyingPharse && clickbuytower)
            {
                selectingTower = witch;
                Cursor.SetCursor(witchCursor,hotSpot , CursorMode.ForceSoftware);
                TowerTypeNum = 0;
                Debug.Log(selectingTower);
            }
        }
        
    }
}
