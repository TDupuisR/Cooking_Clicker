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
    [SerializeField] PreparationScrollBar m_preparationScrollBar;

    [Header("Dish Values")]
    [SerializeField] DishBehavior m_dish;
    GameManagerStatic.DishStates m_currentStates = GameManagerStatic.DishStates.Wait;
    int m_progress;    
    
    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    public DishBehavior dish { get => m_dish; set => m_dish = value; }
    public PreparationScrollBar scrollbar { set => m_preparationScrollBar = value; }
    
    private void Awake()
    {
        //recipe = new Recipes(name, m_ressourcesNeeded, m_preparationTime, m_machineNeeded, m_cookingTime);
        print(transform.localPosition);

        m_nameText.text = m_dish.name;
        m_image.sprite = m_dish.sprite;
        m_button.interactable = false;
    }

    private void FixedUpdate()
    {
        switch (m_currentStates)
        {
            case GameManagerStatic.DishStates.Wait:
                if (CheckIngredients())
                {
                    m_button.interactable = true;

                    foreach(GameManagerStatic.RessourcesNames ingredients in m_dish.ingredients)
                    {
                        GameManager.ressourceManager.ressourcesAmount[(int)ingredients]--;
                    }

                    StartPreparation();
                    m_currentStates = GameManagerStatic.DishStates.Prep;
                }
                break;

            case GameManagerStatic.DishStates.Cook:

                CookerManager.instance.DishQueue.Add(m_dish);
                m_preparationScrollBar.PreparationButtons.Remove(gameObject);
                m_preparationScrollBar.UpdateSize();

                Destroy(gameObject);
                break;
        }
    }

    bool CheckIngredients()
    {
        bool HasIngredients = true;

        //Compte le nombre d'ingrédient
        int[] ressourcesNeededAmount = new int[15];
        foreach (GameManagerStatic.RessourcesNames ingredient in m_dish.ingredients) ressourcesNeededAmount[(int)ingredient]++;

        //Vérifie si on a les ingrédients
        foreach (GameManagerStatic.RessourcesNames ingredient in m_dish.ingredients)
        {
            if (GameManager.ressourceManager.ressourcesAmount[(int)ingredient] < ressourcesNeededAmount[(int)ingredient]) HasIngredients = false;
        }
        return HasIngredients;
    }

    public void StartPreparation() => StartCoroutine(AutoPreparation());
    public void AddProgress(int amount) { m_progress += amount; OnProgression.Invoke(); }
    IEnumerator AutoPreparation()
    {
        float waitTime = m_dish.prepTime / 100f;
        while (m_progress < 100)
        {
            yield return new WaitForSeconds(waitTime);
            m_progressionSlider.value = m_progress;
            m_progress++;
        }
        m_currentStates = GameManagerStatic.DishStates.Cook;
    }
}