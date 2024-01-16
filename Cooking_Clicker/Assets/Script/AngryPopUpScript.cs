using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngryPopUpScript : MonoBehaviour
{
    [SerializeField] Image m_img;
    [SerializeField] Sprite m_askOrderSprite;
    [SerializeField] Sprite m_waitDishSprite;

    [SerializeField] int m_askOrderNumber;
    [SerializeField] int m_waitDishNumber;
    bool m_canDisplay;
    public int AskOrderNumber 
    { 
        get => m_askOrderNumber;
        set 
        { 
            m_askOrderNumber = value;
            UpdatePopUp();
        }
    }
    public int WaitDishNumber 
    { 
        get => m_waitDishNumber;
        set
        {
            m_waitDishNumber = value;
            UpdatePopUp();
        }
    }
    private void Awake()
    {
        SwitchScript.OnSwitchScreen += DisplayPopUp;
        CustomerBehaviour.changeAngryPopUp += changeAngryValues;
    }

    private void changeAngryValues(bool type, int change)
    {
        if(type)
        {
            AskOrderNumber += change;
        }
        else
        {
            WaitDishNumber += change;
        }
    }

    void DisplayPopUp(int currentScreen)
    {
        m_canDisplay = (currentScreen != 0);
        if(m_askOrderNumber + m_waitDishNumber > 0)
            m_img.enabled = m_canDisplay;
    }

    void UpdatePopUp()
    {
        m_img.enabled = m_canDisplay;
        if (m_askOrderNumber > 0)
        {
            m_img.sprite = m_askOrderSprite;
        }
        else if(m_waitDishNumber > 0)
        {
            m_img.sprite = m_waitDishSprite;
        }
        else
        {
            m_img.enabled = false;
        }
    }
}
