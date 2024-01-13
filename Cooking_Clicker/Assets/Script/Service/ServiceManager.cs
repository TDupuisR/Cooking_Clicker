using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using GameManagerSpace;
using Random = UnityEngine.Random;
using TMPro;

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
    [Header("Queue")]
    [SerializeField] int m_queueCount;
    [SerializeField] GameObject m_queueObject; 
    [SerializeField] TMP_Text m_queueText; 

    [Header("Waiter")]
    [SerializeField] List<GameObject> m_waiterList;
    [SerializeField] float m_maximumTip;

    [Header("Sound")]
    [SerializeField] AudioClip m_newCustomerSound;
    [SerializeField] AudioClip m_ServeCustomerSound;

    bool m_spawnCustomer = true;

    public float currentTipMultiplier { get; set; }

    public List<DishBehavior> DishReady { get => m_dishReady; set => m_dishReady = value; }

    public event Action<int> _OnGiveDish;
    public event Action<int> _OnCallForDecrement;

    private void Awake()
    {
        m_dishOrdered.Clear();
        m_dishReady.Clear();

        if (instance != null) Destroy(gameObject);
        instance = this;

        StartCoroutine(InfiniteCustomerSpawner());

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

    public int OrderDish(DishBehavior newDish, int m_linkedSeat)
    {
        m_dishOrdered.Add(newDish);
        GameObject prepButton = Instantiate(m_preparationPrefab, m_preparationParent);
        prepButton.GetComponent<PreparationButton>().dish = newDish;
        prepButton.GetComponent<PreparationButton>().LinkedSeat = m_linkedSeat;
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
        m_dishReady.Remove(servedDish);
        m_dishIsReady[dishIndex] = false;
        ReArrengeWaiters(dishIndex);
        _OnCallForDecrement?.Invoke(dishIndex);

        GameManager.Instance.Money += (uint)((servedDish.moneyValue * GameManager.Instance.Multiplier) + m_maximumTip * currentTipMultiplier);
        GameManager.soundManager.SpawnSound(m_ServeCustomerSound);
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
        m_seatList[seatNumber].Occupied = false;
        if (m_queueCount > 0)
            SpawnCustomer();
    }

    private void DestroyServer(int dishIndex)
    {
        DishBehavior servedDish = m_dishOrdered[dishIndex];

        m_dishOrdered.Remove(servedDish);
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