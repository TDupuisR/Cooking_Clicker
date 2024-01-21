using System;
using System.Collections;
using UnityEngine;
using GameManagerSpace;

public class SwitchScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] Transform ServiceGameObject;
    [SerializeField] Transform CookingGameObject;
    [SerializeField] Transform ProductionGameObject;

    [Space(5)]
    [SerializeField] float m_transitionSpeed;
    bool IsBusy;

    public static event Action<int> OnSwitchScreen;

    public void StartSwitchScreen(int Screen) { if (!IsBusy) StartCoroutine(SwitchScreen(Screen)); }

    IEnumerator SwitchScreen(int Offset)
    {
        IsBusy = true;
        OnSwitchScreen?.Invoke(Offset);

        //Definition de PLEIN de variables importantes
        float timeElapsed = 0;
        float m_universalYPosition = CookingGameObject.position.y; //Determine la position y des écrans
        float m_startingOffset = Camera.main.pixelWidth / 2;

        //Determine start and ending position of each Screens
        Vector3 startServicePosition = ServiceGameObject.position;
        Vector3 endServicePosition = new Vector3(-Camera.main.pixelWidth * Offset + m_startingOffset, m_universalYPosition, 0f);

        Vector3 startCookingPosition = CookingGameObject.position;
        Vector3 endCookingPosition = new Vector3(-Camera.main.pixelWidth * Offset + m_startingOffset * 3, m_universalYPosition, 0f);

        Vector3 startProductionPosition = ProductionGameObject.position;
        Vector3 endProductionPosition = new Vector3(-Camera.main.pixelWidth * Offset + m_startingOffset * 5, m_universalYPosition, 0f);

        

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

        GameManager.Instance.ChangePanel(Offset);

        IsBusy = false;
    }
}
