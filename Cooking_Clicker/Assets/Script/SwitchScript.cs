using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private Transform ServiceGameObject;
    [SerializeField] private Transform CookingGameObject;
    [SerializeField] private Transform ProductionGameObject;

    [Space(5)]
    [SerializeField] private float m_transitionSpeed;
    bool IsBusy;

    float m_universalYPosition;
    
    
    /*
     PAS ENCORE FINI !
     */

    float i = 1;
    //-360
    //360
    //720

    private void Start()
    {
       print(Camera.main.scaledPixelWidth);
       m_universalYPosition = CookingGameObject.position.y;
    }
    public void StartSwitchScreen(int Screen) { if (!IsBusy) StartCoroutine(SwitchScreen(Screen)); }

    IEnumerator SwitchScreen(int Offset)
    {
        IsBusy = true;
        float timeElapsed = 0;

        //Determine start and ending position of each Screens
        Vector3 startServicePosition = ServiceGameObject.position;
        Vector3 endServicePosition = new Vector3(-720 * Offset + 360, m_universalYPosition, 0f);

        Vector3 startCookingPosition = CookingGameObject.position;
        Vector3 endCookingPosition = new Vector3(-720 * Offset + 1080, m_universalYPosition, 0f);

        Vector3 startProductionPosition = ProductionGameObject.position;
        Vector3 endProductionPosition = new Vector3(-720 * Offset + 1800, m_universalYPosition, 0f);

        print(endServicePosition);

        while (timeElapsed < m_transitionSpeed)
        {
            ServiceGameObject.position = Vector3.Lerp(startServicePosition, endServicePosition, timeElapsed / m_transitionSpeed);
            CookingGameObject.position = Vector3.Lerp(startCookingPosition, endCookingPosition, timeElapsed / m_transitionSpeed);
            ProductionGameObject.position = Vector3.Lerp(startProductionPosition, endProductionPosition, timeElapsed / m_transitionSpeed);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        ServiceGameObject.position = endServicePosition;
        CookingGameObject.position = endCookingPosition;
        ProductionGameObject.position = endProductionPosition;

        IsBusy = false;
    }
}
