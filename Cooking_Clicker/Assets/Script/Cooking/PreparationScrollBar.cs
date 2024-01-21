using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameManagerSpace;

public class PreparationScrollBar : MonoBehaviour
{
    //Here to organize, scroll throught prep buttons

    [SerializeField] Scrollbar m_scrollBar;
    [SerializeField] List<GameObject> m_PreparationButtons;
    [SerializeField] Transform m_firstTargetTransform;

    [Header("Inputs")]
    [SerializeField] StandaloneInputModule m_input;
    [SerializeField] RectTransform m_canvas;
    [SerializeField] int m_thisPanel;
    [Space(8)]
    [SerializeField] bool m_noMaxHeightLimit;
    Vector2 m_heightLimit; // x = MaxHeight // y = MinHeight //
    Vector2 m_startMousePos;
    float m_newScrollValue = 0f;

    [Header("organization values")]
    [SerializeField] Vector2 m_startingPosition;
    [SerializeField] float m_offsetPosition;
    [SerializeField] int m_ignorePrepButtons;
    float defaultYPos;

    [Header("Audio")]
    [SerializeField] List<AudioClip> m_speedProgressionAudio;

    public List<GameObject> PreparationButtons 
    { 
        get => m_PreparationButtons;
        set => m_PreparationButtons = value;
    }

    private void Awake()
    {
        if(m_firstTargetTransform != null)
        {
            m_startingPosition = m_PreparationButtons[0].transform.localPosition;            
        }
        defaultYPos = m_startingPosition.y;
    }

    private void Start()
    {
        if (m_noMaxHeightLimit) m_heightLimit = new Vector2(Camera.main.pixelHeight, Camera.main.pixelHeight * 0.1f);
        else m_heightLimit = new Vector2(Camera.main.pixelHeight / 2, Camera.main.pixelHeight *0.1f);
    }

    private void Update()
    {
        CheckForInput();
    }

    public void UpdateSize()
    {
        if (m_PreparationButtons.Count > 0) m_scrollBar.size = 1f / m_PreparationButtons.Count;
        else m_scrollBar.size = 1;
        m_scrollBar.value = 0;

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

    public void ScrollButtons(float value)
    {
        if (m_PreparationButtons.Count < 3) return;
        m_startingPosition = new Vector2(m_startingPosition.x,
        defaultYPos - value * (m_PreparationButtons.Count - m_ignorePrepButtons) * m_offsetPosition);

        UpdateButtonsPositions();
    }

    private void CheckForInput()
    {
        if (m_thisPanel == GameManager.Instance.CurrentPanel)
        {
            if (m_input.input.GetMouseButtonDown(0))
                m_startMousePos = m_input.input.mousePosition;

            if (m_input.input.GetMouseButton(0) && 
                m_startMousePos.y > m_heightLimit.y && ((m_startMousePos.y < m_heightLimit.x) || m_noMaxHeightLimit) &&
                m_startMousePos.x < Camera.main.pixelWidth * 0.89f)
            {
                float currentGapRatio = (m_input.input.mousePosition.y - m_startMousePos.y) / Camera.main.scaledPixelHeight;

                float factor = 1f;
                if (!m_noMaxHeightLimit) factor *= 2; 
                float screenButtonRatio = (400* m_PreparationButtons.Count +50) / (m_canvas.rect.height * factor);

                float value = m_scrollBar.value + (currentGapRatio / screenButtonRatio);
                if (value < 0) value = 0; else if (value > 1) value = 1;

                ScrollButtons(value);
                m_newScrollValue = value;
            }

            if (m_input.input.GetMouseButtonUp(0) &&
                m_startMousePos.y > m_heightLimit.y && ((m_startMousePos.y < m_heightLimit.x) || m_noMaxHeightLimit) &&
                m_startMousePos.x < Camera.main.pixelWidth * 0.89f)
                m_scrollBar.value = m_newScrollValue;
        }
    }
}