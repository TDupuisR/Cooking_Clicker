using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dish", menuName = "Dishes")]
public class DishBehavior : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite sprite;

    public enum RessourcesNames
    {
        Tomate, Carotte, Pomme_de_terre, Petit_pois, Haricot, Veau, Boeuf, Poulet, Canard, Mouton,
        Lait, Beurre, Riz, Pates, Oeufs
    }

    public List<RessourcesNames> ingredients = new List<RessourcesNames>();
}