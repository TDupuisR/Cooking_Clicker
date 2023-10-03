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
    private void Start()
    {
        m_productName.text = RessourceManager.instance.GetProductName(m_productType);
        m_productImage.sprite = RessourceManager.instance.productImage[m_productType];
    }

    private void FixedUpdate()
    {
        m_productAmount.text = RessourceManager.instance.ressourcesAmount[m_productType].ToString();

    }
}
