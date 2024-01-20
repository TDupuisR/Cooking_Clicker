using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterBehavior : MonoBehaviour
{
    [SerializeField] Transform m_origin;
    [SerializeField] float m_speed;
    [SerializeField] Vector3 m_targetPosition;

    private bool m_isWaiting;
    private bool m_isServed;
    public bool IsWaiting { get => m_isWaiting; set => m_isWaiting = value; }
    public bool IsServed { get => m_isServed; set => m_isServed = value; }

    private void Awake()
    {
        ServiceManager.instance._OnWaiterStartServing += GoToServe;
    }
    private void Start()
    {
        m_isWaiting = true;
        m_isServed = false;
        m_speed = 1.0f;

        m_targetPosition = m_origin.position;
        transform.position = m_origin.position;
    }

    private void FixedUpdate()
    {
        MoveTo(m_targetPosition);

        ChechStatus();
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) >= 0.1)
        {
            Vector3 move = Vector3.Normalize(targetPosition - transform.localPosition) * (Time.fixedDeltaTime * m_speed);
            transform.Translate(move, Space.Self);
        }
        else transform.localPosition = targetPosition;
    }

    private void ChechStatus()
    {
        if (transform.localPosition == m_origin.localPosition)
        {
            m_isWaiting = true;
            m_isServed = false;
        }
        if (transform.localPosition == m_targetPosition && m_targetPosition != m_origin.localPosition)
        {
            m_isServed = true;
            m_targetPosition = m_origin.localPosition;
        }
    }

    private void GoToServe(Vector2 seat)
    {
        m_targetPosition = seat;
        m_isWaiting = false;
    }

    public void UpgradeWaiter()
    {
        m_speed += 0.1f;
    }
}
