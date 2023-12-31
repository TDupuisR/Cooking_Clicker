using System.Collections;
using UnityEngine;

public class CustomerBehaviour : MonoBehaviour
{
    enum customerState
    {
            MOVETOPLACE,
            WAITINGORDER,
            WAITINGDISH,
            MOVETOEXIT
    }

    [SerializeField] DishBehavior m_orderDish;
    [SerializeField] customerState m_currentState;
    [SerializeField] Vector2 m_placePosition;
    Vector2 m_startPosition;
    [SerializeField] int m_orderDishIndex = -1;

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

        ServiceManager.instance._OnGiveDish += GiveDish;
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
            m_currentState=customerState.WAITINGDISH;
            m_orderDishIndex = ServiceManager.instance.OrderDish(m_orderDish);
        }
    }

    void GiveDish(int orderIndex)
    {
        if(m_orderDishIndex == orderIndex)
        {
            m_currentState = customerState.MOVETOEXIT;
            StartCoroutine(ReturnToSpawnPoint(1f));
        }
    }
    IEnumerator ReturnToSpawnPoint(float moveSpeed)
    {
        float timeElapsed = 0;

        timeElapsed = 0;
        while (timeElapsed < moveSpeed)
        {
            Vector3 newPosition = Vector3.Lerp(m_placePosition, m_startPosition, timeElapsed / moveSpeed);
            newPosition.y = transform.localPosition.y; //Clamp Y position so only move in X
            transform.localPosition = newPosition;
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = new Vector2(m_startPosition.x, m_placePosition.y);

        timeElapsed = 0;
        while (timeElapsed < moveSpeed)
        {
            Vector3 newPosition = Vector3.Lerp(m_placePosition, m_startPosition, timeElapsed / moveSpeed);
            newPosition.x = m_startPosition.x; //Clamp X position so only move in Y
            transform.localPosition = newPosition;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
