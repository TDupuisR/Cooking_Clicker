using System.Collections.Generic;
using UnityEngine;
using GameManagerSpace;
using System;

public class RessourceManager : MonoBehaviour
{
    public int[] ressourcesAmount = new int[Enum.GetNames(typeof (GameManagerStatic.RessourcesNames)).Length];

    [SerializeField] Sprite m_debugSprite;
    [SerializeField] List<Sprite> m_productImage;


    public GameManagerStatic.RessourcesNames ReturnRessource(int id)
    {
        if (id >= ressourcesAmount.Length || id < 0) { throw new ArgumentException("ID is out of range for: RessourcesNames"); }
        GameManagerStatic.RessourcesNames ressource = (GameManagerStatic.RessourcesNames)id;
        return ressource;
    }

    public string ReturnRessourceName(int id)
    {
        if (id >= ressourcesAmount.Length || id < 0) { Debug.LogError("ID is out of range for: RessourcesNames"); return "Hagrid!"; }
        GameManagerStatic.RessourcesNames name = (GameManagerStatic.RessourcesNames)id;
        return name.ToString();
    }

    public Sprite ReturnRessourceSprite(int id)
    {
        if (id >= ressourcesAmount.Length || id < 0) { Debug.LogError("ID is out of range for: RessourcesSprite"); return m_debugSprite; }
        return m_productImage[id];
    }
}
