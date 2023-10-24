using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationScrollBar : MonoBehaviour
{
    //Here to organize, scroll throught prep buttons

    [SerializeField] Scrollbar m_scrollBar;
    [SerializeField] List<GameObject> m_PreparationButtons;
    [SerializeField] Transform m_firstTargetTransform;

    [Header("organization values")]
    [SerializeField] Vector2 m_startingPosition;
    [SerializeField] float m_offsetPosition;
    [SerializeField] float m_arbirtayStartingOffset;
    [SerializeField] int m_ignorePrepButtons;
    float defaultYPos;

    public List<GameObject> PreparationButtons 
    { 
        get => m_PreparationButtons;
        set => m_PreparationButtons = value;
    }

    private void Awake()
    {
        if(m_firstTargetTransform != null)
        {
            m_PreparationButtons[0].transform.localPosition = m_firstTargetTransform.localPosition;
            m_startingPosition = m_firstTargetTransform.localPosition;
            
        }
        defaultYPos = m_startingPosition.y;
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

        print(m_startingPosition);
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

    public void ScrollButtons(float value)
    {
        if (m_PreparationButtons.Count < 3) return;
        print(defaultYPos);
        m_startingPosition = new Vector2(m_startingPosition.x,
        defaultYPos - value * (m_PreparationButtons.Count - m_ignorePrepButtons) * m_offsetPosition + m_arbirtayStartingOffset);

        UpdateButtonsPositions();
    }
}