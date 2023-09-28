using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceManager : MonoBehaviour
{
    private static RessourceManager Instance;

    private enum RessourcesNames
    {
        Tomate, Carotte, Pomme_de_terre, Petit_pois, Haricot, Veau, Boeuf, Poulet, Canard, Mouton,
        Lait, Beurre, Riz, Pates, Oeufs
    }
    private int[] m_ressourcesAmount = new int[15];

    //penser a mettre propriété de RessourcesAmount

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
