using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using GameManagerSpace;
using Random = UnityEngine.Random;
using TMPro;
using Unity.VisualScripting;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;

    [SerializeField] List<DishBehavior> m_dishOrdered = new List<DishBehavior>();
    [SerializeField] List<DishBehavior> m_dishReady = new List<DishBehavior>();
    [SerializeField] List<bool> m_dishIsReady = new List<bool>();

    [Header("Preparation")]
    [SerializeField] GameObject m_preparationPrefab;
    [SerializeField] Transform m_preparationParent;
    [SerializeField] PreparationScrollBar m_preparationScrollBar;

    [Header("Customer")]
    [SerializeField] GameObject m_customerPrefab;
    [SerializeField] Transform m_customerParent;
    [SerializeField] Transform m_customerSpawnPoint;
    [SerializeField] List<Seat> m_seatList;
    List<Seat> m_dishLinkedSeat = new List<Seat>();

    [Header("Queue")]
    [SerializeField] int m_queueCount;
    [SerializeField] GameObject m_queueObject; 
    [SerializeField] TMP_Text m_queueText; 

    [Header("Waiter")]
    [SerializeField] List<GameObject> m_waiterList;
    [SerializeField] float m_maximumTip;
    [SerializeField] WaiterBehavior m_waiterScript;

    [Header("Sound")]
    [SerializeField] AudioClip m_newCustomerSound;
    [SerializeField] AudioClip m_ServeCustomerSound;

    bool m_spawnCustomer = true;

    public float currentTipMultiplier { get; set; }

    public List<DishBehavior> DishReady { get => m_dishReady; set => m_dishReady = value; }

    public event Action<int> _OnGiveDish;
    public event Action<int> _OnCallForDecrement;
    public event Action<Vector2> _OnWaiterStartServing;

    private void Awake()
    {
        m_dishOrdered.Clear();
        m_dishReady.Clear();

        if (instance != null) Destroy(gameObject);
        instance = this;

        StartCoroutine(InfiniteCustomerSpawner());
        StartCoroutine(InfiniteWaiterChecker());

        CustomerBehaviour.onDoneWaitingGiveOrderDishIndex += DestroyServer;
    }

    IEnumerator InfiniteCustomerSpawner()
    {
        while(m_spawnCustomer)
        {
            yield return new WaitForSeconds(Random.Range(1, 10));
            SpawnCustomer();
        }
    }

    IEnumerator InfiniteWaiterChecker()
    {
        while (true)
        {
            for (int i = 0; i < m_dishIsReady.Count; i++)
            {
                if (m_dishIsReady[i])
                {
                    ServeDishByWaiter(i);
                    yield return new WaitUntil(() => m_waiterScript.IsServed == true);
                    yield return new WaitUntil(() => m_waiterScript.IsWaiting == true);

                    i = m_dishIsReady.Count;
                }
            }

            for (int k = 0; k < m_seatList.Count; k++)
            {
                if (m_seatList[k].Customer == null) continue;
                else if (m_seatList[k].Customer.CurrentState == CustomerBehaviour.customerState.WAITINGORDER)
                {
                    StartCoroutine(TakeOrder(m_seatList[k]));
                    yield return new WaitUntil(() => m_waiterScript.IsServed == true);
                    yield return new WaitUntil(() => m_waiterScript.IsWaiting == true);

                    k = m_seatList.Count;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator TakeOrder(Seat customerSeat)
    {
        _OnWaiterStartServing?.Invoke(customerSeat.position);
        yield return new WaitUntil(() => m_waiterScript.IsServed);

        if (customerSeat.Customer.CurrentState == CustomerBehaviour.customerState.WAITINGORDER)
        {
            customerSeat.Customer.GetOrder();
        }
    }

    public int OrderDish(DishBehavior newDish, CustomerBehaviour linkedCustomer)
    {
        m_dishOrdered.Add(newDish);
        m_dishLinkedSeat.Add(m_seatList[linkedCustomer.designedSeat]);
        GameObject prepButton = Instantiate(m_preparationPrefab, m_preparationParent);
        prepButton.GetComponent<PreparationButton>().dish = newDish;
        prepButton.GetComponent<PreparationButton>().LinkedSeat = linkedCustomer.designedSeat;
        prepButton.GetComponent<PreparationButton>().LinkedCustomer = linkedCustomer;
        prepButton.GetComponent<PreparationButton>().ChangeInterface();
        prepButton.GetComponent<PreparationButton>().scrollbar = m_preparationScrollBar;
        prepButton.transform.SetSiblingIndex(2); 

        m_preparationScrollBar.PreparationButtons.Add(prepButton);
        m_preparationScrollBar.UpdateSize();

        return m_dishOrdered.Count - 1;
    }

    public void ServeDish(int dishIndex)
    {
        m_waiterList[dishIndex].SetActive(false);

        if (m_dishOrdered.Count <= dishIndex)
            throw new Exception("dishIndex too high, served dish can't be in m_dishOrdered");

        DishBehavior servedDish = m_dishOrdered[dishIndex];

        if (!m_dishReady.Contains(servedDish))
            throw new Exception("Served dish isn't in m_dishReady");

        _OnGiveDish?.Invoke(dishIndex);
        m_dishOrdered.Remove(servedDish);
        m_dishLinkedSeat.RemoveAt(dishIndex);
        m_dishReady.Remove(servedDish);
        m_dishIsReady[dishIndex] = false;
        ReArrengeWaiters(dishIndex);
        _OnCallForDecrement?.Invoke(dishIndex);

        GameManager.Instance.Money += (uint)((servedDish.moneyValue * GameManager.Instance.Multiplier) + m_maximumTip * currentTipMultiplier);
        GameManager.soundManager.SpawnSound(m_ServeCustomerSound);
    }

    void ServeDishByWaiter(int dishIndex)
    {
        m_waiterList[dishIndex].SetActive(false);

        if (m_dishOrdered.Count <= dishIndex)
            throw new Exception("dishIndex too high, served dish can't be in m_dishOrdered");

        DishBehavior servedDish = m_dishOrdered[dishIndex];

        if (!m_dishReady.Contains(servedDish))
            throw new Exception("Served dish isn't in m_dishReady");

        StartCoroutine(WaiterCourseCoroutine(dishIndex, servedDish));
    }

    IEnumerator WaiterCourseCoroutine(int dishIndex, DishBehavior servedDish)
    {
        _OnWaiterStartServing?.Invoke(m_dishLinkedSeat[dishIndex].position);
        yield return new WaitUntil(() => m_waiterScript.IsServed);

        if (m_dishOrdered.Contains(servedDish))
        {
            _OnGiveDish?.Invoke(dishIndex);
            m_dishOrdered.Remove(servedDish);
            m_dishLinkedSeat.RemoveAt(dishIndex);
            m_dishReady.Remove(servedDish);
            m_dishIsReady[dishIndex] = false;
            ReArrengeWaiters(dishIndex);
            _OnCallForDecrement?.Invoke(dishIndex);

            GameManager.Instance.Money += (uint)((servedDish.moneyValue * GameManager.Instance.Multiplier) + m_maximumTip * currentTipMultiplier);
            GameManager.soundManager.SpawnSound(m_ServeCustomerSound);
        }
    }

    void ReArrengeWaiters(int dishIndex)
    {
        //foreach (GameObject waiter in m_waiterList)
        //    waiter.SetActive(false);
        //for (int i = 0; i < m_dishReady.Count; i++)
        //    m_waiterList[i].SetActive(true);

        for (int i = dishIndex + 1; i < 8; i++)
        {
            if (m_dishIsReady[i])
            {
                m_waiterList[i].SetActive(false);
                m_dishIsReady[i] = false;

                m_waiterList[i - 1].SetActive(true);
                m_dishIsReady[i - 1] = true;
            }
        }

    }

    public void SpawnCustomer()
    {
        //Select available seat
        Seat newSeat = null;
        int randIndex = Random.Range(0, m_seatList.Count);
        do
        {
            randIndex = Random.Range(0, m_seatList.Count);
            if (!m_seatList[randIndex].Occupied)
            {
                newSeat = m_seatList[randIndex];
                m_seatList[randIndex].Occupied = true;
            }            
        }
        while (newSeat == null && HasAvailableSeat());

        if(newSeat != null)
        {
            GameManager.soundManager.SpawnSound(m_newCustomerSound);
            GameObject newCustomer = Instantiate(m_customerPrefab, m_customerParent);
            newCustomer.transform.localPosition = m_customerSpawnPoint.localPosition;

            CustomerBehaviour newCustomerBehaviour = newCustomer.GetComponent<CustomerBehaviour>();
            m_seatList[randIndex].Customer = newCustomerBehaviour;
            newCustomerBehaviour.orderDish = GameManager.Instance.GetRandomDish();
            newCustomerBehaviour.placePosition = newSeat.position;
            newCustomerBehaviour.designedSeat = randIndex;

            if(m_queueCount > 0)
            {
                m_queueCount--;
                DisplayQueue();
            }
        }
        else
        {
            //Add to queue
            m_queueCount++;
            DisplayQueue();
        }
    }
    bool HasAvailableSeat()
    {
        for(int i = 0; i < m_seatList.Count; i++)
            if (!m_seatList[i].Occupied)
                return true;

        return false;
    }
    void DisplayQueue()
    {
        m_queueObject.SetActive(m_queueCount > 0);
        m_queueText.text = "x" + m_queueCount.ToString();
    }
    public void SpawnWaiter(DishBehavior dish)
    {
        for(int i = 0;i < m_dishOrdered.Count; i++)
        {
            if (m_dishOrdered[i] == dish && !m_dishIsReady[i])
            {
                m_waiterList[i].SetActive(true);
                m_dishIsReady[i] = true;
                return;
            }
        }
    }

    public void FreeSeat(int seatNumber)
    {
        m_seatList[seatNumber].Customer = null;
        m_seatList[seatNumber].Occupied = false;
        if (m_queueCount > 0)
            SpawnCustomer();
    }

    private void DestroyServer(int dishIndex)
    {
        DishBehavior servedDish = m_dishOrdered[dishIndex];

        m_dishOrdered.Remove(servedDish);
        m_dishLinkedSeat.RemoveAt(dishIndex);
        m_dishReady.Remove(servedDish);
        m_waiterList[dishIndex].SetActive(false);
        m_dishIsReady[dishIndex] = false;
        ReArrengeWaiters(dishIndex);
        _OnCallForDecrement?.Invoke(dishIndex);
    }

    [Space(20)]
    [Header("DEBUG")]
    [SerializeField] DishBehavior m_testDish;
    [Button]
    void DEBUG_CreateTestCustomer() => SpawnCustomer();
}