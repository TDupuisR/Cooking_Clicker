using System.Collections.Generic;
using UnityEngine;
using GameManagerSpace;
using System;
using NaughtyAttributes;
public class RessourceManager : MonoBehaviour
{
    public int[] ressourcesAmount = new int[Enum.GetNames(typeof (GameManagerStatic.RessourcesNames)).Length];


    [SerializeField] Sprite m_debugSprite;
    [SerializeField] List<Sprite> m_productImage;
    [SerializeField] List<bool> m_availableIngredients;


    public static event Action<int> OnUnlockButton;

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
    public void SaveIngredients(int newIngredients)
    {
        m_availableIngredients[newIngredients] = true;

        string ingredientsString = "";
        for(int i = m_availableIngredients.Count - 1; i > -1; i--)
        {
            ingredientsString += m_availableIngredients[i] ? "1" : "0";
        }

        int ingredientsInt = Convert.ToInt32(ingredientsString, 2);

        PlayerPrefs.SetInt("ingredients",ingredientsInt);

        PlayerPrefs.Save();
    }
    public void LoadIngredients()
    {
        if (!PlayerPrefs.HasKey("ingredients"))
        {
            PlayerPrefs.SetInt("ingredients", 1);
            return;
        }

        int ingredientsInt = PlayerPrefs.GetInt("ingredients");
        string ingredientsString = Convert.ToString(ingredientsInt, 2);

        int IngredientCounter = 0;
        foreach (char c in ingredientsString)
        {
            if (c == '1') //Has Ingredient
            {
                OnUnlockButton?.Invoke(IngredientCounter);

                string SaveAmountString = ReturnRessourceName(IngredientCounter) + "_amount";
                if(PlayerPrefs.HasKey(SaveAmountString))
                    ressourcesAmount[IngredientCounter] = PlayerPrefs.GetInt(SaveAmountString);
            }

            IngredientCounter++;
        }
    }

   [Button]
    public void TestSaveIngredients() => SaveIngredients(0);
}
