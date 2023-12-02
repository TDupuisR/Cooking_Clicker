using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DishManager : MonoBehaviour
{
    [SerializeField] List<DishBehavior> m_dishList;
    [SerializeField] List<DishBehavior> m_availableDishs;

    public DishBehavior ReturnRandomDish()
    {
        return m_availableDishs[Random.Range(0, m_availableDishs.Count)];
    }

    public void CheckForNewDish()
    {
        //Use when buy new ingredients
        throw new NotImplementedException();
    }
}
