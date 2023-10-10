using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookerManager : MonoBehaviour
{
    public static CookerManager instance;

    public enum CookingMachines
    {
        FURNACE = 0,
    }
    [SerializeField] List<Slider> m_progressionSliders;
    [SerializeField] List<Image> m_cookerImage;
    [SerializeField] List<bool> m_machineUsed;

    List<DishBehavior> m_dishQueue = new List<DishBehavior>();
    public List<DishBehavior> DishQueue { get => m_dishQueue; set => m_dishQueue = value; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        int m_iteration = 0;
        if(m_dishQueue.Count != 0) foreach(DishBehavior dish in m_dishQueue)
        {
            if (!m_machineUsed[(int)dish.ustensils])
            {
                StartCoroutine( UseMachine(dish, m_iteration) );
            }
            m_iteration++;
        }
    }

    IEnumerator UseMachine(DishBehavior dish, int recepieIndex)
    {
        int progress = 0;
        int m_machineIndex = (int)dish.ustensils;
        m_machineUsed[m_machineIndex] = true;
        m_cookerImage[m_machineIndex].color = new Color(.5f, .5f, .5f, 1f);

        float waitTime = dish.cookTime / 100f;
        while (progress < 100)
        {
            yield return new WaitForSeconds(waitTime);
            progress++;
            m_progressionSliders[m_machineIndex].value = progress;
        }
        
        //INTEGRER CODE POUR SERVIR
        /*
            Passer la recette au ServiceManager
         */

        m_dishQueue.RemoveAt(recepieIndex);
        m_machineUsed[m_machineIndex] = false;
        m_progressionSliders[m_machineIndex].value = 0;
        m_cookerImage[m_machineIndex].color = new Color(1f, 1f, 1f, 1f);

        print(m_dishQueue.Count);
    }
}
