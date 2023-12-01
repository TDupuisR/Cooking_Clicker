using System.Collections;
using UnityEngine;

public class CustomerBehaviour : MonoBehaviour
{
    enum customerState
    {
            MOVETOPLACE,
            WAITINGORDER,
            WAITINGDISH,
    }

    [SerializeField] DishBehavior m_orderDish;
    [SerializeField] customerState m_currentState;
    [SerializeField] Vector2 m_placePosition;
    Vector2 m_startPosition;

    public Vector2 placePosition
    {
        get => m_placePosition; set => m_placePosition = value;
    }
    public DishBehavior orderDish
    {
        get => m_orderDish; set => m_orderDish = value;
    }

    private void Awake()
    {
        m_startPosition = transform.localPosition;
        m_currentState = customerState.MOVETOPLACE;

        StartCoroutine(MoveToPlace(2f));
    }

    IEnumerator MoveToPlace(float moveSpeed)
    {
        float timeElapsed = 0;

        while (timeElapsed < moveSpeed)
        {
            Vector3 newPosition = Vector3.Lerp(m_startPosition, m_placePosition, timeElapsed / moveSpeed);
            newPosition.x = m_startPosition.x; //Clamp X position so only move in Y
            transform.localPosition = newPosition; 
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = new Vector2(m_startPosition.x, m_placePosition.y);

        timeElapsed = 0;
        while (timeElapsed < moveSpeed)
        {
            Vector3 newPosition = Vector3.Lerp(m_startPosition, m_placePosition, timeElapsed / moveSpeed);
            newPosition.y = transform.localPosition.y; //Clamp Y position so only move in X
            transform.localPosition = newPosition;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        m_currentState = customerState.WAITINGORDER;
    }

    public void GetOrder()
    {
        if(m_currentState == customerState.WAITINGORDER) {
            ServiceManager.instance.OrderDish(m_orderDish);
            m_currentState=customerState.WAITINGDISH;
        }
    }
}
