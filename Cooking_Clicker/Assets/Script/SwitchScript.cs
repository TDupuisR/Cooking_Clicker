using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Transform ServiceGameObject;
    [SerializeField] private Transform CookingGameObject;
    [SerializeField] private Transform ProductionGameObject;

    [Space(5)]
    [SerializeField] private float m_transitionSpeed;

    private void Start()
    {
        print(ProductionGameObject.localPosition);
    }

    /*
     PAS ENCORE FINI !
     */

    public void StartSwitchScreen(int Screen) => StartCoroutine(SwitchScreen(Screen));

    IEnumerator SwitchScreen(int Offset)
    {
        float timeElapsed = 0;

        //Determine start and ending position of each Screens
        Vector2 startServicePosition = ServiceGameObject.localPosition;
        Vector2 endServicePosition = new Vector2(-1280 * Offset, 0f);
        
        Vector2 startCookingPosition = ServiceGameObject.localPosition;
        Vector2 endCookingPosition = new Vector2(1280 - 1280 * Offset, 0f);
        
        Vector2 startProductionPosition = ServiceGameObject.localPosition;
        Vector2 endProductionPosition = new Vector2(2560 - 1280 * Offset, 0f);


        while (timeElapsed < m_transitionSpeed)
        {
            ServiceGameObject.localPosition = Vector3.Lerp(startServicePosition, endServicePosition, timeElapsed / m_transitionSpeed);
            CookingGameObject.localPosition = Vector3.Lerp(startCookingPosition, endCookingPosition, timeElapsed / m_transitionSpeed);
            ProductionGameObject.localPosition = Vector3.Lerp(startProductionPosition, endProductionPosition, timeElapsed / m_transitionSpeed);
            
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ServiceGameObject.localPosition = endServicePosition;
        CookingGameObject.localPosition = endCookingPosition;
        ProductionGameObject.localPosition = endProductionPosition;
    }
}
