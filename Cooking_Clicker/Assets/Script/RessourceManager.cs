using System.Collections.Generic;
using UnityEngine;
using GameManagerSpace;
using System;

public class RessourceManager : MonoBehaviour
{
    public int[] ressourcesAmount = new int[Enum.GetNames(typeof (GameManagerStatic.RessourcesNames)).Length];
    
    [SerializeField] List<Sprite> m_productImage;
    public List<Sprite> productImage { get => m_productImage;}

    public string ReturnRessourceName(int id)
    {
        GameManagerStatic.RessourcesNames name = (GameManagerStatic.RessourcesNames)id;
        return name.ToString();
    }
}
