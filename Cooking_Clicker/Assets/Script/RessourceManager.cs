using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceManager : MonoBehaviour
{
    public static RessourceManager instance;

    public enum RessourcesNames
    {
        Tomate, Carotte, Pomme_de_terre, Petit_pois, Haricot, Veau, Boeuf, Poulet, Canard, Mouton,
        Lait, Beurre, Riz, Pates, Oeufs
    }
    public int[] ressourcesAmount = new int[15];
    
    [SerializeField] private List<Sprite> m_productImage;
    public List<Sprite> productImage { get => m_productImage;}

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }


    public string GetProductName(int id)
    {
        RessourcesNames name = (RessourcesNames)id;
        return name.ToString();
    }
}
