using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidePanel : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject upgradePanel;

    [SerializeField] private List<GameObject> skillButtons;

    private bool isShopShowing = true;

    public bool switchShop()
    {
        if(isShopShowing)
        {
            return true;
        }
        else
        {
            hideUpgrade();
            return false;
        }
    }

    public void showUpgrade(Tower tower)
    {
        upgradePanel.SetActive(true);
        shopPanel.SetActive(false);
        isShopShowing = false;
    }

    public void hideUpgrade()
    {
        upgradePanel.SetActive(false);
        shopPanel.SetActive(true);
        isShopShowing = true;
    }
    
    void Start()
    {
        shopPanel.SetActive(true);
        upgradePanel.SetActive(false);
    }
}
