using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Recipes
{
    public enum States
    {
        WAIT, PREP, COOK, SERVE
    }
    public States currentState;
    
    public List<RessourceManager.RessourcesNames> ressourcesNeeded;
    
    string m_recipesName;
    int m_progress;
    float m_preparationTime;
    float m_cookingTime;

    public string recipesName { get => m_recipesName; set => m_recipesName = value; }
    public int progress { get => m_progress; set => m_progress = value; }
    public float preparationTime { get => m_preparationTime; set => m_preparationTime = value; }
    public float cookingTime { get => m_cookingTime; set => m_cookingTime = value; }

    public Recipes(string name, List<RessourceManager.RessourcesNames> ingredients, float preparation, float cooking)
    {
        currentState = States.WAIT;
        ressourcesNeeded = ingredients;

        m_recipesName = name;
        m_progress = 0;
        m_preparationTime = preparation;
        m_cookingTime = cooking;
    }
}
