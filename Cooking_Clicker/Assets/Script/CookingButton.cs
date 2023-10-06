using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookingButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text m_nameText;
    [SerializeField] Image m_image;
    [SerializeField] Slider m_progressionSlider;

    Recipes recepie;
    [Header("Recepies Values")]
    [SerializeField] List<RessourceManager.RessourcesNames> m_ressourcesNeeded;
    [SerializeField] string m_recipesName;
    [SerializeField] float m_preparationTime;
    [SerializeField] float m_cookingTime;

    private void Awake()
    {
        recepie = new Recipes(name, m_ressourcesNeeded, m_preparationTime, m_cookingTime);

        m_nameText.text = m_recipesName;
    }

    public void StartPreparation() => StartCoroutine(AutoPreparation());
    public void AddProgress(int amount) => recepie.progress += amount;
    IEnumerator AutoPreparation()
    {
        while (recepie.progress < 100)
        {
            yield return new WaitForSeconds(recepie.preparationTime / 100);
            m_progressionSlider.value = recepie.progress;
            recepie.progress++;
        }
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