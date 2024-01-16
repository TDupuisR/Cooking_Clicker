using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparationPopUp : MonoBehaviour
{
    [SerializeField] Image m_imgPreparation; 
    [SerializeField] TMP_Text m_numberOfPreparationText; 
    [SerializeField] private int m_numberOfDishInPreparation;

    private void Awake()
    {
        PreparationButton.ChangePreparationButton += ChangeNumberPreparation;

        ChangeNumberPreparation(0);
    }

    private void ChangeNumberPreparation(int change)
    {
        m_numberOfDishInPreparation += change;

        m_numberOfPreparationText.text = m_numberOfDishInPreparation.ToString();
        m_imgPreparation.enabled = m_numberOfDishInPreparation != 0;
        m_numberOfPreparationText.enabled = m_numberOfDishInPreparation != 0;
    }
}
