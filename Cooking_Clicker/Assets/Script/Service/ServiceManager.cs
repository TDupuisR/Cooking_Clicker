using GameManagerSpace;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using Random = UnityEngine.Random;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;

    List<DishBehavior> m_dishOrdered = new List<DishBehavior>();
    List<DishBehavior> m_dishReady = new List<DishBehavior>();

    [Header("Preparation")]
    [SerializeField] GameObject m_preparationPrefab;
    [SerializeField] Transform m_preparationParent;
    [SerializeField] PreparationScrollBar m_preparationScrollBar;

    [Header("Customer")]
    [SerializeField] GameObject m_customerPrefab;
    [SerializeField] Transform m_customerParent;
    [SerializeField] Transform m_customerSpawnPoint;
    [SerializeField] List<Seat> m_seatList;

    public List<DishBehavior> DishReady { get => m_dishReady; set => m_dishReady = value; }

    public event Action<int> _OnGiveDish;

    private void Awake()
    {
        m_dishOrdered.Clear();
        m_dishReady.Clear();

        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    public int OrderDish(DishBehavior newDish)
    {
        m_dishOrdered.Add(newDish);
        GameObject prepButton = Instantiate(m_preparationPrefab, m_preparationParent);
        prepButton.GetComponent<PreparationButton>().dish = newDish;
        prepButton.GetComponent<PreparationButton>().scrollbar = m_preparationScrollBar;
        prepButton.transform.SetSiblingIndex(2);

        m_preparationScrollBar.PreparationButtons.Add(prepButton);
        m_preparationScrollBar.UpdateSize();
        
        return m_dishOrdered.IndexOf(newDish);
    }

    public void ServeDish(DishBehavior servedDish)
    {
        if (m_dishOrdered.Contains(servedDish) && m_dishReady.Contains(servedDish))
        {
            _OnGiveDish?.Invoke(m_dishOrdered.IndexOf(servedDish));
            m_dishOrdered.Remove(servedDish);
            m_dishReady.Remove(servedDish);

            GameManager.Instance.Money += (uint)servedDish.moneyValue;
        }
        else throw new System.Exception("Served dish isn't in m_dishOrdered and in m_dishReady");
    }

    public void SpawnCustomer(DishBehavior newOrderDish)
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
        print(newSeat);

        if(newSeat != null)
        {
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

    [Space(20)]
    [Header("DEBUG")]
    [SerializeField] DishBehavior m_testDish;
    [Button]
    void DEBUG_CreateTestCustomer() => SpawnCustomer(m_testDish);
    [Button]
    void DEBUG_ServeTestOrder() => ServeDish(m_testDish);
}
