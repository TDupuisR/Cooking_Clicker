using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameManagerSpace;

public class PreparationButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text m_nameText;
    [SerializeField] Image m_image;
    [SerializeField] Slider m_progressionSlider;
    [SerializeField] Button m_button;

    Recipes recipe;
    [Header("Recepies Values")]
    [SerializeField] List<GameManagerStatic.RessourcesNames> m_ressourcesNeeded;
    [SerializeField] string m_recipesName;
    [SerializeField] float m_preparationTime;
    [SerializeField] CookerManager.CookingMachines m_machineNeeded;
    [SerializeField] float m_cookingTime;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    private void Awake()
    {
        recipe = new Recipes(name, m_ressourcesNeeded, m_preparationTime, m_machineNeeded, m_cookingTime);

        m_nameText.text = m_recipesName;
        m_button.interactable = false;
    }

    private void FixedUpdate()
    {
        switch (recipe.currentState)
        {
            case Recipes.States.WAIT:
                if (CheckIngredients())
                {
                    m_button.interactable = true;

                    foreach(GameManagerStatic.RessourcesNames ingredients in m_ressourcesNeeded)
                    {
                        GameManager.ressourceManager.ressourcesAmount[(int)ingredients]--;
                    }

                    StartPreparation();
                    recipe.currentState = Recipes.States.PREP;
                }
                break;

            case Recipes.States.COOK:
                CookerManager.instance.RecipesQueue.Add(recipe);
                Destroy(gameObject);
                break;
        }
    }

    bool CheckIngredients()
    {
        bool HasIngredients = true;
        foreach(GameManagerStatic.RessourcesNames ingredient in m_ressourcesNeeded)
        {
            if (GameManager.ressourceManager.ressourcesAmount[(int)ingredient] == 0) HasIngredients = false;
        }
        return HasIngredients;
    }

    public void StartPreparation() => StartCoroutine(AutoPreparation());
    public void AddProgress(int amount) { recipe.progress += amount; OnProgression.Invoke(); }
    IEnumerator AutoPreparation()
    {
        while (recipe.progress < 100)
        {
            yield return new WaitForSeconds(recipe.preparationTime / 100);
            m_progressionSlider.value = recipe.progress;
            recipe.progress++;
        }
        recipe.currentState = Recipes.States.COOK;
    }
}