using GameManagerSpace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DishDico : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_helpParent;
    [SerializeField] TMP_Text m_dishName;
    [SerializeField] TMP_Text m_dishDescription;
    [SerializeField] Image m_dishImage;

    [SerializeField] TMP_Text m_dishIngredients;
    [SerializeField] TMP_Text m_dishPrepTime;
    [SerializeField] TMP_Text m_dishCookTime;
    [SerializeField] TMP_Text m_dishPrice;

    public void ShowHelp(DishBehavior dish)
    {
        m_helpParent.SetActive(true);
        m_dishName.text = dish.name;
        m_dishDescription.text = dish.description;
        m_dishImage.sprite = dish.sprite;


        string ingredientText = "<b>Ingredients :</b> \n";
        foreach (var ingredient in dish.ingredients)
        {
            ingredientText += "<indent=15%>- " + GameManager.ressourceManager.ReturnRessourceName((int)ingredient) + "\n";
        }
        m_dishIngredients.text = ingredientText;

        m_dishPrepTime.text = "Preparation time : " + dish.prepTime.ToString() + " seconds";
        m_dishCookTime.text = "Cooking time : " + dish.cookTime.ToString() + " seconds";
        m_dishPrice.text = "Price (without tip) : " + (int)(dish.moneyValue * GameManager.Instance.Multiplier) + "$";
    }

    public void HideHelp()
    {
        m_helpParent.SetActive(false);
    }
}
