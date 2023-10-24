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
    [SerializeField] TMP_Text m_productName;
    [SerializeField] TMP_Text m_productAmount;
    [SerializeField] Slider m_progressionSlider;

    [Header("Field")]
    [SerializeField] int m_productType;
    [SerializeField] int m_progression;
    [SerializeField] float m_progressionTime;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    public delegate void OnTouchdelegate(Vector2 spawnPos, int[] id);
    public static event OnTouchdelegate OnTouch;

    private void Start()
    {
        m_productName.text = GameManager.ressourceManager.ReturnRessourceName(m_productType);
        m_productImage.sprite = GameManager.ressourceManager.ReturnRessourceSprite(m_productType);

        StartCoroutine(AutoProgression());
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
