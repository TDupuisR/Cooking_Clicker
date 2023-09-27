using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private RectTransform ServiceGameObject;
    [SerializeField] private RectTransform CookingGameObject;
    [SerializeField] private RectTransform ProductionGameObject;

    [Space(5)]
    [SerializeField] private float m_transitionSpeed;

    private void Start()
    {
        print(ServiceGameObject.rect);
    }

    IEnumerator SwitchScreen()
    {
        float timeElapsed = 0;


        while (timeElapsed < m_transitionSpeed)
        {

            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }
}
