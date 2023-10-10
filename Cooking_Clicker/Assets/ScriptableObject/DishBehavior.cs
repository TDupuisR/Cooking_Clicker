using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagerSpace;

[CreateAssetMenu(fileName = "New Dish", menuName = "Dishes")]
public class DishBehavior : ScriptableObject
{
    public new string name;
    public string description;

    public short prepTime;
    public short cookTime;

    public Sprite sprite;

    public List<GameManagerStatic.RessourcesNames> ingredients = new List<GameManagerStatic.RessourcesNames>();

    public GameManagerStatic.CookingMachines ustensils;
}