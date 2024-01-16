using GameManagerSpace;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private int m_designedSeat;

    [Space(10)]
    [SerializeField] GameObject m_askOrderGameObject;
    [SerializeField] GameObject m_waitOrderGameObject;
    [Header("Pressure fields")]
    [SerializeField] Image m_askOrderImg;
    [SerializeField] Image m_waitOrderImg;
    [SerializeField] Color m_startColor;
    [SerializeField] Color m_endColor;
    [SerializeField] float m_waitOrderLimit;
    [SerializeField] float m_waitDishLimit;
    float m_waitingMultiplier;
    Coroutine m_waitCoRoutine;

    public static event Action<int> onIsDoneWaiting;
    public static event Action<int> onDoneWaitingGiveOrderDishIndex;

    [Space(10)]
    [SerializeField] AudioClip m_GetOrderSound;

    public Vector2 placePosition
    {
        get => m_placePosition; set => m_placePosition = value;
    }
    public DishBehavior orderDish
    {
        get => m_orderDish; set => m_orderDish = value;
    }
    public int designedSeat
    {
        get => m_designedSeat; set => m_designedSeat = value;
    }
    public float WaitingMultiplier { get => m_waitingMultiplier; }

    private void Awake()
    {
        GetComponent<Image>().color = new Color(Random.Range(.5f, 1), 
                                                Random.Range(.5f, 1), 
                                                Random.Range(.5f, 1));
        m_startPosition = transform.localPosition;
        m_currentState = customerState.MOVETOPLACE;

        StartCoroutine(MoveToPlace(2f));

        ServiceManager.instance._OnGiveDish += GiveDish;
        ServiceManager.instance._OnCallForDecrement += FixIndex;
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

        m_askOrderGameObject.SetActive(true);
        m_currentState = customerState.WAITINGORDER;
        m_waitCoRoutine = StartCoroutine(WaitingCoRoutine(m_askOrderImg, m_waitOrderLimit));
    }

    public void GetOrder()
    {
        if(m_currentState == customerState.WAITINGORDER) {
            GameManager.soundManager.SpawnSound(m_GetOrderSound);
            m_askOrderGameObject.SetActive(false);
            m_waitOrderGameObject.SetActive(true);

            m_currentState = customerState.WAITINGDISH;
            if (m_waitCoRoutine != null)
                StopCoroutine(m_waitCoRoutine);
            m_waitCoRoutine = StartCoroutine(WaitingCoRoutine(m_waitOrderImg, m_waitDishLimit));
            m_orderDishIndex = ServiceManager.instance.OrderDish(m_orderDish, this);
        }
    }

    void GiveDish(int orderIndex)
    {
        if(m_orderDishIndex == orderIndex)
        {
            if(m_waitCoRoutine != null)
                StopCoroutine(m_waitCoRoutine);

            ServiceManager.instance.currentTipMultiplier = m_waitingMultiplier;
            ServiceManager.instance._OnGiveDish -= GiveDish;
            ServiceManager.instance._OnCallForDecrement -= FixIndex;
            ServiceManager.instance.FreeSeat(designedSeat);

            m_currentState = customerState.MOVETOEXIT;
            m_waitOrderGameObject.SetActive(false);
            StartCoroutine(ReturnToSpawnPoint(1f));
        }
    }

    void FixIndex(int indexBorder)
    {
        if(m_orderDishIndex > indexBorder)
            m_orderDishIndex--;
    }

    IEnumerator ReturnToSpawnPoint(float moveSpeed)
    {
        float timeElapsed = 0;
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

    IEnumerator WaitingCoRoutine(Image img, float delay)
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < delay)
        {
            m_waitingMultiplier = 1 - (timeElapsed / delay);
            img.color = Color.Lerp(m_startColor, m_endColor, timeElapsed / delay);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        GetOut();
        onIsDoneWaiting?.Invoke(designedSeat);
    }

    void GetOut()
    {
        m_currentState = customerState.MOVETOEXIT;
        m_waitOrderGameObject.SetActive(false);
        m_askOrderGameObject.SetActive(false);

        ServiceManager.instance._OnGiveDish -= GiveDish;
        ServiceManager.instance._OnCallForDecrement -= FixIndex;
        ServiceManager.instance.FreeSeat(designedSeat);

        StartCoroutine(ReturnToSpawnPoint(1f));
    }

    [Button]
    public void TestGetOut()
    {
        if(m_waitCoRoutine != null)
            StopCoroutine(m_waitCoRoutine);
        GetOut();
        onIsDoneWaiting?.Invoke(designedSeat);
        onDoneWaitingGiveOrderDishIndex?.Invoke(m_orderDishIndex);
    }
}
