using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using GameManagerSpace;
using Random = UnityEngine.Random;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;

    [SerializeField] List<DishBehavior> m_dishOrdered = new List<DishBehavior>();
    [SerializeField] List<DishBehavior> m_dishReady = new List<DishBehavior>();

    [Header("Preparation")]
    [SerializeField] GameObject m_preparationPrefab;
    [SerializeField] Transform m_preparationParent;
    [SerializeField] PreparationScrollBar m_preparationScrollBar;

    [Header("Customer")]
    [SerializeField] GameObject m_customerPrefab;
    [SerializeField] Transform m_customerParent;
    [SerializeField] Transform m_customerSpawnPoint;
    [SerializeField] List<Seat> m_seatList;

    [Header("Waiter")]
    [SerializeField] List<GameObject> m_waiterList;

    [Header("Sound")]
    [SerializeField] AudioClip m_newCustomerSound;
    [SerializeField] AudioClip m_ServeCustomerSound;

    bool m_spawnCustomer = true;

    public List<DishBehavior> DishReady { get => m_dishReady; set => m_dishReady = value; }

    public event Action<int> _OnGiveDish;

    private void Awake()
    {
        m_dishOrdered.Clear();
        m_dishReady.Clear();

        if (instance != null) Destroy(gameObject);
        instance = this;

        StartCoroutine(InfiniteCustomerSpawner());
    }

    IEnumerator InfiniteCustomerSpawner()
    {
        while(m_spawnCustomer)
        {
            yield return new WaitForSeconds(Random.Range(1, 10));
            if (HasAvailableSeat())
                SpawnCustomer();
        }
    }

    public int OrderDish(DishBehavior newDish)
    {
        m_dishOrdered.Add(newDish);
        GameObject prepButton = Instantiate(m_preparationPrefab, m_preparationParent);
        prepButton.GetComponent<PreparationButton>().dish = newDish;
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

        //if (m_dishOrdered.Count <= dishIndex)
        //    throw new Exception("dishIndex too high, served dish can't be in m_dishOrdered");

        DishBehavior servedDish = m_dishOrdered[dishIndex];

        if (!m_dishReady.Contains(servedDish))
            throw new Exception("Served dish isn't in m_dishReady");

        _OnGiveDish?.Invoke(dishIndex);
        m_dishOrdered.Remove(servedDish);
        m_dishReady.Remove(servedDish);

        GameManager.Instance.Money += (uint)servedDish.moneyValue;
        GameManager.soundManager.SpawnSound(m_ServeCustomerSound);
    }

    public void SpawnCustomer()
    {
        //Select available seat
        Seat newSeat = null;
        do
        {
            int randIndex = Random.Range(0, m_seatList.Count);
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
        }
    }
    bool HasAvailableSeat()
    {
        for(int i = 0; i < m_seatList.Count; i++)
            if (!m_seatList[i].Occupied)
                return true;

        return false;
    }

    public void SpawnWaiter()
    {
        m_waiterList[m_dishReady.Count - 1].SetActive(true);
    }

    [Space(20)]
    [Header("DEBUG")]
    [SerializeField] DishBehavior m_testDish;
    [Button]
    void DEBUG_CreateTestCustomer() => SpawnCustomer();
}