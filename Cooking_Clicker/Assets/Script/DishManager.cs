using GameManagerSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DishManager : MonoBehaviour
{
    [SerializeField] DishDico m_dishDico;

    [SerializeField] List<DishBehavior> m_dishList;
    [SerializeField] List<DishBehavior> m_availableDishs;
    [SerializeField] List<GameManagerStatic.RessourcesNames> m_availableIngredients;

    public List<GameManagerStatic.RessourcesNames> availableIngredients
    {
        get => m_availableIngredients; 
        set => m_availableIngredients = value;
    }

    public DishBehavior ReturnRandomDish()
    {
        return m_availableDishs[Random.Range(0, m_availableDishs.Count)];
    }

    public void CheckForNewDish()
    {
        foreach(DishBehavior dish in m_dishList)
        {
            if (m_availableDishs.Contains(dish)) continue;

            bool missingIngredients = false;
            foreach(GameManagerStatic.RessourcesNames ingredients in dish.ingredients)
                if(!m_availableIngredients.Contains(ingredients)) missingIngredients = true;

            if(!missingIngredients)
            {
                m_availableDishs.Add(dish);
            }
        }
    }

    public void ShowDishDico(DishBehavior dish) => m_dishDico.ShowHelp(dish);
}
