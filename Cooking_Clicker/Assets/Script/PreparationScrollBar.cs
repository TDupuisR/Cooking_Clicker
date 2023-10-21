using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationScrollBar : MonoBehaviour
{
    //Here to organize, scroll throught prep buttons

    [SerializeField] Scrollbar m_scrollBar;
    [SerializeField] List<GameObject> m_PreparationButtons;

    [Header("organization values")]
    [SerializeField] Vector2 m_startingPosition;
    [SerializeField] float m_offsetPosition;

    public List<GameObject> PreparationButtons 
    { 
        get => m_PreparationButtons;
        set => m_PreparationButtons = value;
    }

    public void UpdateSize()
    {
        if (m_PreparationButtons.Count > 0) m_scrollBar.size = 1f / m_PreparationButtons.Count;
        else m_scrollBar.size = 1;

        UpdateButtonsPositions();
    }

    public void UpdateButtonsPositions()
    {
        if(m_PreparationButtons.Count == 0) return;

        m_PreparationButtons[0].transform.localPosition = m_startingPosition;
        if (m_PreparationButtons.Count > 1)
        {
            Vector2 currentPosition = m_startingPosition;
            for (int i = 1; i < m_PreparationButtons.Count; i++)
            {
                currentPosition.y += m_offsetPosition;
                m_PreparationButtons[i].transform.localPosition = currentPosition;
            }
        }
    }
}