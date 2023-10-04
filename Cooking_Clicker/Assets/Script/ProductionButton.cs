using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start()
    {
        m_productName.text = RessourceManager.instance.GetProductName(m_productType);
        m_productImage.sprite = RessourceManager.instance.productImage[m_productType];

        StartCoroutine(AutoProgression());
    }

    private void FixedUpdate()
    {
        m_productAmount.text = RessourceManager.instance.ressourcesAmount[m_productType].ToString();
        m_progressionSlider.value = m_progression;

        if(m_progression > 100)
        {
            m_progression = 0;
            RessourceManager.instance.ressourcesAmount[m_productType]++;
        }
    }

    public void SpeedProgression(int amount)
    {
        m_progression += amount;
    }

    IEnumerator AutoProgression()
    {
        yield return new WaitForSeconds(m_progressionTime);
        m_progression++;
        StartCoroutine(AutoProgression());
    }
}
