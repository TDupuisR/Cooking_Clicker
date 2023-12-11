using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameManagerSpace;
using Random = UnityEngine.Random;

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

    [Header("Audio")]
    [SerializeField] List<AudioClip> m_speedProgressionAudio;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    public delegate void OnTouchdelegate(Vector2 spawnPos, int[] id);
    public static event OnTouchdelegate OnTouch;

    public DishBehavior dish { get => m_dish; set => m_dish = value; }
    public PreparationScrollBar scrollbar { set => m_preparationScrollBar = value; }
    public GameManagerStatic.DishStates currentState { set => m_currentStates = value; }
    
    private void Awake()
    {
        //recipe = new Recipes(name, m_ressourcesNeeded, m_preparationTime, m_machineNeeded, m_cookingTime);

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
    public void AddProgress(int amount) { 
        m_progress += amount;
        OnProgression.Invoke();

        GameManager.soundManager.SpawnSound(m_speedProgressionAudio[Random.Range(0, m_speedProgressionAudio.Count)]);


        int nbIngredient = 0;
        foreach (GameManagerStatic.RessourcesNames ingredient in m_dish.ingredients) nbIngredient++;

        int[] ressourcesAmount = new int[nbIngredient]; int i = 0;
        foreach (GameManagerStatic.RessourcesNames ingredient in m_dish.ingredients) { ressourcesAmount[i] = (int)ingredient; i++; }

        OnTouch.Invoke(transform.position, ressourcesAmount);
    }
    IEnumerator AutoPreparation()
    {
        float waitTime = m_dish.prepTime / 100f;
        while (m_progress < 100)
        {
            yield return new WaitForSeconds(waitTime);
            m_progressionSlider.value = m_progress;
            m_progress++;
        }
        CookerManager.instance.LinkedPrepButton.Add(this);
        CookerManager.instance.DishQueue.Add(m_dish);
        m_image.color = new Color(.5f, .5f, .5f);
        m_currentStates = GameManagerStatic.DishStates.QueueCook;
    }

    public void ChangeInterface()
    {
        m_nameText.text = m_dish.name;
        m_image.sprite = m_dish.sprite;
    }

    public void ShowHelp()
    {
        GameManager.dishManager.ShowDishDico(m_dish);
    }
}