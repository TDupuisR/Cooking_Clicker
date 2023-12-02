using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameManagerSpace;

public class ProductionButton : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Image m_productImage;
    [SerializeField] Button m_productionButton;
    [SerializeField] TMP_Text m_productName;
    [SerializeField] TMP_Text m_productAmount;
    [SerializeField] Slider m_progressionSlider;

    [Header("Field")]
    [SerializeField] int m_productType;
    [SerializeField] int m_progression;
    [SerializeField] float m_progressionTime;
    [Space(5)]
    [SerializeField] bool m_isUnlocked;
    [SerializeField] GameObject m_lockedObject;
    Coroutine m_autoCoRoutine;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    public delegate void OnTouchdelegate(Vector2 spawnPos, int[] id);
    public static event OnTouchdelegate OnTouch;

    private void Start()
    {
        m_productName.text = GameManager.ressourceManager.ReturnRessourceName(m_productType);
        m_productImage.sprite = GameManager.ressourceManager.ReturnRessourceSprite(m_productType);

        if (m_isUnlocked) 
               UnlockButton();
        else
            m_productionButton.interactable = false;
    }

    private void FixedUpdate()
    {
        m_productAmount.text = GameManager.ressourceManager.ressourcesAmount[m_productType].ToString();
        m_progressionSlider.value = m_progression;

        if(m_progression > 100)
        {
            m_progression = 0;
            GameManager.ressourceManager.ressourcesAmount[m_productType]++;
            OnCompletion.Invoke();
        }
    }

    public void UnlockButton()
    {
        m_autoCoRoutine = StartCoroutine(AutoProgression());
        m_productionButton.interactable = true;
        m_isUnlocked = false;
        m_lockedObject.SetActive(false);
    }

    public void SpeedProgression(int amount)
    {
        m_progression += amount;
        OnProgression.Invoke();

        int[] id = { m_productType };
        OnTouch.Invoke(transform.position, id);
    }

    IEnumerator AutoProgression()
    {
        yield return new WaitForSeconds(m_progressionTime);
        m_progression++;
        StartCoroutine(AutoProgression());
    }
}
