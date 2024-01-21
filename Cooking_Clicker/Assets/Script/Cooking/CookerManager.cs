using GameManagerSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookerManager : MonoBehaviour
{
    public static CookerManager instance;

    [SerializeField] List<Slider> m_progressionSliders;
    [SerializeField] List<Image> m_progressionSlidersFill;
    [SerializeField] List<Image> m_cookerImage;
    [SerializeField] List<bool> m_machineUsed;
    [SerializeField] List<int> m_currentDishIndex;
    [SerializeField] List<bool> m_dishIndexNeedDecrement;
    [SerializeField] Gradient m_fillgradient;

    [SerializeField] List<DishBehavior> m_dishQueue = new List<DishBehavior>();
    [SerializeField] List<PreparationButton> m_linkedPrepButton = new List<PreparationButton>();
    [Space(5)]
    [SerializeField] List<int> m_ustensilSeatList;
    [SerializeField] List<int> m_ustensilRecipeIndexList;
    List<Coroutine> m_ustensilCoroutine = new List<Coroutine>();
    //index 0 to 7 -> number = dishIndex
    public List<DishBehavior> DishQueue { get => m_dishQueue; set => m_dishQueue = value; }
    public List<PreparationButton> LinkedPrepButton { get => m_linkedPrepButton; set => m_linkedPrepButton = value; }

    [SerializeField] AudioClip m_dishReady;

    public delegate void OnSwitchToCook(int index);
    public static event OnSwitchToCook OnGoingToCook;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        CustomerBehaviour.onIsDoneWaiting += DestroyDish;

        //Initialize Capacity
        for (int i = 0; i < m_machineUsed.Count; i++)
            m_ustensilCoroutine.Add(null);
    }

    private void FixedUpdate()
    {
        int m_iteration = 0;
        if(m_dishQueue.Count != 0) foreach(DishBehavior dish in m_dishQueue)
        {
            int dishUstensils = (int)dish.ustensils;
            if (!m_machineUsed[dishUstensils])
            {
                int linkedSeat = m_linkedPrepButton[m_iteration].LinkedSeat;
                m_ustensilSeatList[linkedSeat] = dishUstensils;
                m_ustensilRecipeIndexList[linkedSeat] = m_iteration;
                LinkedPrepButton[m_iteration].currentState = GameManagerStatic.DishStates.Cook;
                m_ustensilCoroutine[dishUstensils] = StartCoroutine( UseMachine(dish, m_iteration, linkedSeat, m_fillgradient) );
            }
            m_iteration++;
        }
    }

    IEnumerator UseMachine(DishBehavior dish, int recepieIndex, int seat, Gradient fillGradient)
    {
        int progress = 0;
        int m_machineIndex = (int)dish.ustensils;
        m_machineUsed[m_machineIndex] = true;
        m_cookerImage[m_machineIndex].color = new Color(.5f, .5f, .5f, 1f);

        m_currentDishIndex[m_machineIndex] = recepieIndex;
        m_dishIndexNeedDecrement[m_machineIndex] = false;

        float waitTime = dish.cookTime / 100f;
        while (progress < 100)
        {
            yield return new WaitForSeconds(waitTime);

            if (m_dishIndexNeedDecrement[m_machineIndex])
            {
                recepieIndex--;
                m_dishIndexNeedDecrement[m_machineIndex] = false;
            }
            progress++;
            m_progressionSliders[m_machineIndex].value = progress;
            m_progressionSlidersFill[m_machineIndex].color = fillGradient.Evaluate(progress / 100.0f);
        }

        ServiceManager.instance.DishReady.Add(dish);
        GameManager.soundManager.SpawnSound(m_dishReady);
        ServiceManager.instance.SpawnWaiter(dish);

        for(int i = 0; i < m_currentDishIndex.Count; i++)
        {
            if (m_currentDishIndex[i] > recepieIndex)
                m_dishIndexNeedDecrement[i] = true;
        }

        m_dishQueue.RemoveAt(recepieIndex);
        LinkedPrepButton.RemoveAt(recepieIndex);
        m_ustensilSeatList[seat] = -1;

        m_machineUsed[m_machineIndex] = false;
        m_progressionSliders[m_machineIndex].value = 0;
        m_cookerImage[m_machineIndex].color = new Color(1f, 1f, 1f, 1f);
    }

    private void DestroyDish(int seat)
    {
        if (m_ustensilSeatList[seat] == -1)
            return;

        int machineIndex = m_ustensilSeatList[seat];
        int recepieIndex = m_ustensilRecipeIndexList[seat];

        if(m_ustensilCoroutine[machineIndex] != null)
            StopCoroutine(m_ustensilCoroutine[machineIndex]);

        m_dishQueue.RemoveAt(recepieIndex);
        LinkedPrepButton.RemoveAt(recepieIndex);
        m_ustensilSeatList[seat] = -1;

        m_machineUsed[machineIndex] = false;
        m_progressionSliders[machineIndex].value = 0;
        m_cookerImage[machineIndex].color = new Color(1f, 1f, 1f, 1f);
    }
}
