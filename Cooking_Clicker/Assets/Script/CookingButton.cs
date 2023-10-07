using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CookingButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text m_nameText;
    [SerializeField] Image m_image;
    [SerializeField] Slider m_progressionSlider;
    [SerializeField] Button m_button;

    Recipes recepie;
    [Header("Recepies Values")]
    [SerializeField] List<RessourceManager.RessourcesNames> m_ressourcesNeeded;
    [SerializeField] string m_recipesName;
    [SerializeField] float m_preparationTime;
    [SerializeField] float m_cookingTime;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    private void Awake()
    {
        recepie = new Recipes(name, m_ressourcesNeeded, m_preparationTime, m_cookingTime);

        m_nameText.text = m_recipesName;
        m_button.interactable = false;
    }

    private void FixedUpdate()
    {
        switch (recepie.currentState)
        {
            case Recipes.States.WAIT:
                if (CheckIngredients())
                {
                    m_button.interactable = true;
                    StartPreparation();
                    recepie.currentState = Recipes.States.PREP;
                }
                break;
        }
    }

    bool CheckIngredients()
    {
        bool HasIngredients = true;
        foreach(RessourceManager.RessourcesNames ingredient in m_ressourcesNeeded)
        {
            if (RessourceManager.instance.ressourcesAmount[(int)ingredient] == 0) HasIngredients = false;
        }
        return HasIngredients;
    }

    public void StartPreparation() => StartCoroutine(AutoPreparation());
    public void AddProgress(int amount) { recepie.progress += amount; OnProgression.Invoke(); }
    IEnumerator AutoPreparation()
    {
        while (recepie.progress < 100)
        {
            yield return new WaitForSeconds(recepie.preparationTime / 100);
            m_progressionSlider.value = recepie.progress;
            recepie.progress++;
        }
        recepie.currentState = Recipes.States.COOK;
    }

    //public void StartCooking() => StartCoroutine(AutoCooking());
    //IEnumerator AutoCooking()
    //{
    //    while (recepie.progress < 100)
    //    {
    //        yield return new WaitForSeconds(recepie.cookingTime / 100);
    //        recepie.progress++;
    //    }
    //}
}