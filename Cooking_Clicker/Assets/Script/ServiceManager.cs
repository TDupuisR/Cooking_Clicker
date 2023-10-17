using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    List<DishBehavior> m_dishQueue = new List<DishBehavior>();
    public List<DishBehavior> DishQueue { get => m_dishQueue; set => m_dishQueue = value; }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
