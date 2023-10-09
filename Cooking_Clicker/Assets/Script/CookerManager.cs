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

    List<Recipes> m_recipesQueue = new List<Recipes>();
    public List<Recipes> RecipesQueue { get => m_recipesQueue; set => m_recipesQueue = value; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        int m_iteration = 0;
        if(m_recipesQueue.Count != 0) foreach(Recipes recipe in m_recipesQueue)
        {
            if (!m_machineUsed[(int)recipe.MachineNeeded])
            {
                StartCoroutine( UseMachine(recipe, m_iteration) );
            }
            m_iteration++;
        }
    }

    IEnumerator UseMachine(Recipes recipe, int recepieIndex)
    {
        recipe.progress = 0;
        int m_machineIndex = (int)recipe.MachineNeeded;
        m_machineUsed[m_machineIndex] = true;
        m_cookerImage[m_machineIndex].color = new Color(.5f, .5f, .5f, 1f);

        while (recipe.progress < 100)
        {
            yield return new WaitForSeconds(recipe.cookingTime / 100);
            recipe.progress++;
            m_progressionSliders[m_machineIndex].value = recipe.progress;
        }
        
        recipe.currentState = Recipes.States.SERVE;

        //INTEGRER CODE POUR SERVIR
        /*
            Passer la recette au ServiceManager
         */

        RecipesQueue.RemoveAt(recepieIndex);
        m_machineUsed[m_machineIndex] = false;
        m_progressionSliders[m_machineIndex].value = 0;
        m_cookerImage[m_machineIndex].color = new Color(.5f, .5f, .5f, 1f);

        print(RecipesQueue.Count);
    }
}
