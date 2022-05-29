using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustRandom : MonoBehaviour
{
    [SerializeField] private int populationSize = 50;
    [SerializeField] private int maxGeneration = 100;

    private List<GameObject> availableMonster = new List<GameObject>();
    private int costLeft = 0;

    private int best_fitness = 0;
    
    public void init(int maxCost, List<GameObject> availableMonster, int round)
    {
        this.availableMonster = availableMonster;
        
        for(int g = 1; g <= maxGeneration; g++)
        {
            for(int p = 0; p < populationSize; p++)
            {
                costLeft = maxCost;
                int current_fitness = 0;

                while(findAffordableMonster().Count > 0)
                {
                    List<int> affordableMonster = findAffordableMonster();
                    int randMon = Random.Range(0, affordableMonster.Count);
                    current_fitness++;
                    costLeft -= availableMonster[affordableMonster[randMon]].GetComponent<Enemy>().getCost();
                }

                best_fitness = current_fitness > best_fitness ? current_fitness : best_fitness;
            }
            Debug.Log("Round: " + round + " Generation: " + g + " Fitness: " + best_fitness);
        }
        
    }

    private List<int> findAffordableMonster()
    {
        List<int> affordableMonster = new List<int>();
        for(int i = 0; i < availableMonster.Count; i++) /* check which enemies are affordable */
        {
            if(costLeft >= availableMonster[i].GetComponent<Enemy>().getCost())
            {
                affordableMonster.Add(i);
            }
        }
        return affordableMonster;
    }
}
