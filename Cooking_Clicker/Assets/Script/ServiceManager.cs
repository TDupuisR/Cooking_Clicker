using GameManagerSpace;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ServiceManager : MonoBehaviour
{
    public static ServiceManager instance;

    [SerializeField] List<DishBehavior> m_dishOrdered = new List<DishBehavior>();
    [SerializeField] List<DishBehavior> m_dishReady = new List<DishBehavior>();

    [SerializeField] GameObject m_preparationPrefab;
    [SerializeField] Transform m_preparationParent;
    [SerializeField] PreparationScrollBar m_preparationScrollBar;

    public List<DishBehavior> DishReady { get => m_dishReady; set => m_dishReady = value; }

    private void Awake()
    {
        m_dishOrdered.Clear();
        m_dishReady.Clear();

        if (instance != null) Destroy(gameObject);
        instance = this;
    }

    public void OrderDish(DishBehavior newDish)
    {
        m_dishOrdered.Add(newDish);
        GameObject prepButton = Instantiate(m_preparationPrefab, m_preparationParent);
        prepButton.GetComponent<PreparationButton>().dish = newDish;
        m_preparationScrollBar.PreparationButtons.Add(prepButton);
        m_preparationScrollBar.UpdateSize();
    }

    public void ServeDish(DishBehavior servedDish)
    {
        if (m_dishOrdered.Contains(servedDish) && m_dishReady.Contains(servedDish))
        {
            m_dishOrdered.Remove(servedDish);
            m_dishReady.Remove(servedDish);

            GameManager.Instance.Money += (uint)servedDish.moneyValue;
        }
        else throw new System.Exception("Served dish isn't in m_dishOrdered and in m_dishReady");
    }


    [Space(20)]
    [Header("DEBUG")]
    [SerializeField] DishBehavior m_testDish;
    [Button]
    void DEBUG_CreateTestOrder() => OrderDish(m_testDish);
    [Button]
    void DEBUG_ServeTestOrder() => ServeDish(m_testDish);
}
