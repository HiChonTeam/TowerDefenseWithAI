using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesList : MonoBehaviour
{
    EnemiesOnMap enemy;
    public Image ShowEnemies1;
    public Image ShowEnemies2;
    // private int i;
    // public GameObject NormalEnemy;
    // private int rounds = 0 ;
    // private int round = 0;
    // public Image Backup;
    public Sprite NormalEnemy;
    public Sprite EliteEnemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // public void ClickToChangeImage(){
    //     Debug.Log("Change Image Already");
    //     // Backup = original;

    //     // newSprite.sprite = Backup;
    // }
    // Update is called once per frame


    void Update()
    {  
        // enemy = FindObjectOfType<Enemy>();
        foreach(List<GameObject> enemyPath in EnemiesOnMap.enemiesOnMap)
        {
            if(enemyPath.Count > 0)
            {
                // Debug.Log("Kao Ma Nee Laew");
                foreach(GameObject enemy in enemyPath)
                    {
                        // i++;
                        // Debug.Log("Number is bere ===>" + i);
                        Enemy enemyComp = enemy.GetComponent<Enemy>();
                        //Debug.Log("Name is ==>" + enemyComp.getName());
                        if(enemyComp.getName() == "Basic"){
                            // Debug.Log(enemyComp.getName());
                            ShowEnemies1.sprite = NormalEnemy;
                            ShowEnemies2.sprite = NormalEnemy; 
                        }
                        else if(enemyComp.getName() == "Elite"){
                            // Debug.Log(enemyComp.getName());
                            ShowEnemies1.sprite = NormalEnemy; 
                            ShowEnemies2.sprite = NormalEnemy; 
                        }
                        // enemyComp.takeConstantDamage(0.25f);
                        // enemyComp.increasePhysicDamageReceive(0.1f);
                        // enemyComp.increaseMagicDamageReceive(0.1f);
                    }
            }
            else{
                // Debug.Log("Here");
            }
        }

        // foreach(List<GameObject> pathOfEnemies in EnemiesOnMap.enemiesOnMap){
        //     // Debug.Log("Name is ==>" + EnemiesOnMap.enemiesOnMap);
        //     Debug.Log("path is here =>" + EnemiesOnMap.GetComponent<Enemy>().getName());
        // }
            // round = RoundController.round + 1 ;
            // // Debug.Log("Round now is +++" + round);
            // // if(EnemiesOnMap.enemiesOnMap == basicEnemy){
            // //     Debug.Log("basicEnemy");
            // // }
            // if(round % 5 != 0){
            //     // Instantiate(NormalEnemy, ShowEnemies1, false);
            //     // Replace(ShowEnemies1,NormalEnemy);
            //     // Debug.Log(EnemiesOnMap.enemiesOnMap);
            //     // ShowEnemies1.SetActive(false);
            //     ShowEnemies1.sprite = NormalEnemy;
            //     ShowEnemies2.sprite = NormalEnemy; 
            // }
            // else{
            //     // Debug.Log(EnemiesOnMap.enemiesOnMap);
            //     // ShowEnemies1.SetActive(true);
            //     ShowEnemies1.sprite = NormalEnemy; 
            //     ShowEnemies2.sprite = NormalEnemy; 
            // }

            // }
        // if(round % 5 == 0){
        // }
        // Debug.Log("Round is =====" + RoundController.round);

    }

    // void Replace(GameObject obj1, GameObject obj2){
    //     ShowEnemies1 = Instantiate(obj2, obj1.transform.position, Quaternion.identity);
    //     Destroy(obj1);
    // }
}
