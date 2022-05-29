using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesList : MonoBehaviour
{
    //EnemiesOnMap enemy;
    //[SerializeField] private Image ShowEnemies1;
    //[SerializeField] private Image ShowEnemies2;
    // private int i;
    // public GameObject NormalEnemy;
    // private int rounds = 0 ;
    // private int round = 0;
    // public Image Backup;

    //public GameObject NormalEnemy;
    private Vector2 posslot1;

    public Sprite NormalEnemy;
    public Sprite EliteEnemy;
    public Sprite CamouBasic;
    public Sprite CamouElite;
    public Sprite ResetImage;
    // [SerializeField] public Sprite CamouBasicEnemy;
    // [SerializeField] public Sprite EliteBasicEnemy;
    public  bool[] isFull;
    public  string[] nameslot;
    public  Image[] slots;
    public static List<GameObject> ThisRoundMonster = new List<GameObject>();

    //public GameObject slots1;

    //public static bool isthathaveEnem;
    // Start is called before the first frame update
    void Start()
    {
        //posslot1 = new Vector2 (slots1.transform.position.x, slots1.transform.position.y);
        //Destroy(slots1);

        // var MynewEnemy = Instantiate(NormalEnemy,slots1.transform.position,Quaternion.identity);
        // Destroy(slots1);
        // MynewEnemy.transform.parent = gameObject.transform;
        //Debug.Log("position is" + new Vector2 (slots1.transform.position.x, slots1.transform.position.y));
        //Debug.Log("Slot Length ==" + slots.Length);
        // isthathaveEnem = false;
        //EnemyList = GameObject.FindGameObjectWithTag("EnemyBar").GetComponent<EnemyList>();
    }

    // public bool isEnemyAlive(){
    //     foreach(List<GameObject> EnemOnMap in EnemiesOnMap.enemiesOnMap){
    //         if(EnemOnMap.Count > 0){
    //             return true;
    //             break;
    //         }
    //     }

    //     //return true;
    // }

    public static void SetEnemy(List<GameObject> monsterAvailable){
        ThisRoundMonster = monsterAvailable;
        // CheckSlotIsFull();  
    }
    // public static void EnemyToShow(){
    //     Debug.Log("Can use");
    // }


    public void CheckSlotIsFull(List<GameObject> monsterAvailable){
        for(int x = 0; x < slots.Length; x++){
            isFull[x] = false;
            slots[x].sprite = ResetImage;
            nameslot[x] = "";
        }
        foreach(GameObject enemy in monsterAvailable){
            Enemy enemyComp = enemy.GetComponent<Enemy>();
            Debug.Log("Enemy Name is ==> " + enemyComp.getName());
            for(int i = 0; i < slots.Length; i++){
                if(isFull[i] == false){// Can place slot 
                    if(enemyComp.getName() == "Basic"){
                        //nameslot[i] = "Basic";
                        //Debug.Log("Name is " + nameslot[i]);
                        slots[i].sprite = NormalEnemy;
                        isFull[i] = true;
                        break;
                        }
                    else if(enemyComp.getName() == "Elite"){
                        //nameslot[i] = "Elite";
                        //Debug.Log("Name is " + nameslot[i]);
                        slots[i].sprite = EliteEnemy;
                        isFull[i] = true;
                        break;
                    }
                    else if(enemyComp.getName() == "Camou Basic"){
                        //nameslot[i] = "Elite";
                        //Debug.Log("Name is " + nameslot[i]);
                        slots[i].sprite = CamouBasic;
                        isFull[i] = true;
                        break;
                    }
                    else if(enemyComp.getName() == "Camou Elite"){
                        //nameslot[i] = "Elite";
                        //Debug.Log("Name is " + nameslot[i]);
                        slots[i].sprite = CamouElite;
                        isFull[i] = true;
                        break;
                    }
                }
                else{
                    continue;
                }
            }
        }

        // for(int i = 0; i < slots.Length; i++){
        //     if(isFull[i] == false ){
        //         Debug.Log("Come Here");
        //         isFull[i] = true;
        //         break;
        //     }
        //     else{
        //         continue;
        //     }
        // }
    }

    // public static void getEnemyName(){
    //     foreach(List<GameObject> )
    // }
    // public void ClickToChangeImage(){
    //     Debug.Log("Change Image Already");
    //     // Backup = original;

    //     // newSprite.sprite = Backup;
    // }
    // Update is called once per frame


    void Update()
    {  
        //     Debug.Log("Have Enemy");
        //     foreach(GameObject enemy in enemyPath)
        //         {
        //             Enemy enemyComp = enemy.GetComponent<Enemy>();
        //             CheckSlotIsFull(enemyComp);

        //         }
        // else{
        //     Debug.Log("Dont Have Enemy");
        //     for(int i = 0; i < slots.Length; i++){
        //         isFull[i] = false;
        //         nameslot[i] = "";
        //         slots[i].sprite = ResetImage ;
        //     }

        // }
        // }
        // foreach(List<int> MonThisRound in RoundController.monsterReleaseThisRound){
        //     Debug.Log("Enemy Count Is =>"+ MonThisRound);
        // }
        //Instantiate(NormalEnemy,slots1.transform, false);
        //Instantiate(NormalEnemy,slots[0].transform.position,Quaternion.identity);
        // foreach(List<GameObject> enemyPath in EnemiesOnMap.enemiesOnMap){
        //     if(enemyPath.Count > 0){
        //         Debug.Log("Have Enemy");
        //         Instantiate(NormalEnemy,slots[0].transform.position,Quaternion.identity);

        //     }
        //     else{
        //         Debug.Log("Dont Have Enemy");
        // }
        // }


        // Instantiate(NormalEnemy,slots[1].transform,Quaternion.identity);
        // CheckSlotIsFull();
        // enemy = FindObjectOfType<Enemy>();



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
