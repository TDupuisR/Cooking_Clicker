using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] private bool m_Occupied;
    [SerializeField] private Vector2 m_position;

    public bool Occupied { get => m_Occupied; set => m_Occupied = value; }
    public Vector2 position { get => m_position; set => m_position = value; }
}
