using GameManagerSpace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaiterBehavior : MonoBehaviour
{
    [SerializeField] Transform m_origin;
    [SerializeField] float m_speed = 20f;
    float m_initSpeed;
    [SerializeField] Vector3 m_targetPosition;
    [SerializeField] float m_tolerance = 5f;

    [SerializeField] private bool m_isWaiting;
    [SerializeField] private bool m_isServed;
    [SerializeField] private bool m_isServing;

    [Header("Upgrades")]
    [SerializeField] private uint m_upgradePrice;
    [SerializeField] private AudioClip m_upgradeSound;
    [SerializeField] TMP_Text m_upgradePriceText;
    int m_currentUpgrade = 0;


    public bool IsWaiting { get => m_isWaiting; set => m_isWaiting = value; }
    public bool IsServed { get => m_isServed; set => m_isServed = value; }

    private void Start()
    {
        m_isWaiting = true;
        m_isServed = false;
        m_initSpeed = 20.0f;

        if (PlayerPrefs.HasKey("Waiter_level"))
            m_currentUpgrade = PlayerPrefs.GetInt("Waiter_level");

        m_speed = m_initSpeed * Mathf.Pow(1.1f, m_currentUpgrade);
        m_upgradePriceText.text = (m_upgradePrice * (m_currentUpgrade +1)).ToString() + "$";

        m_targetPosition = m_origin.localPosition;
        transform.localPosition = m_origin.localPosition;

        ServiceManager.instance._OnWaiterStartServing += GoToServe;
    }

    private void FixedUpdate()
    {
        MoveTo(m_targetPosition);

        ChechStatus();
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.localPosition, targetPosition) >= m_tolerance)
        {
            Vector3 move = Vector3.Normalize(targetPosition - transform.localPosition) * (Time.fixedDeltaTime * m_speed);
            transform.Translate(move, Space.Self);
        }
        else transform.localPosition = targetPosition;
    }

    private void ChechStatus()
    {
        if (Vector3.Distance(transform.localPosition, m_origin.localPosition) <= m_tolerance && !m_isServing)
        {
            m_isWaiting = true;
            m_isServed = false;
        }
        if (Vector3.Distance(transform.localPosition, m_targetPosition) <= m_tolerance*8 && m_targetPosition != m_origin.localPosition)
        {
            m_isServed = true;
            m_isServing = false;
            m_targetPosition = m_origin.localPosition;
        }
    }

    private void GoToServe(Vector2 seat)
    {
        m_targetPosition = seat;
        m_isWaiting = false;
        m_isServing = true;
    }

    public void UpgradeWaiter()
    {
        if(GameManager.Instance.Money >= m_upgradePrice * m_currentUpgrade)
        {
            GameManager.Instance.Money -= (uint)(m_upgradePrice * (m_currentUpgrade+1));

            m_currentUpgrade += 1;
            m_speed = m_initSpeed * Mathf.Pow(1.1f, m_currentUpgrade);

            PlayerPrefs.SetInt("Waiter_level", m_currentUpgrade);
            PlayerPrefs.Save();

            m_upgradePriceText.text = (m_upgradePrice * (m_currentUpgrade + 1)).ToString() + "$";
        }
    }
}
