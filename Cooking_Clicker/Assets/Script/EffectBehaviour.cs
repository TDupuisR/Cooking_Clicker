using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectBehaviour : MonoBehaviour
{
    [SerializeField] float m_distance;
    [SerializeField] float m_delay;
    Image m_img;

    private void Awake()
    {
        m_img = GetComponent<Image>();
        StartCoroutine(AnimationCoRoutine(transform.position, m_distance, m_delay));
    }

    IEnumerator AnimationCoRoutine(Vector3 startPosition, float distance, float delay)
    {
        transform.position = startPosition;
        float timeElapsed = 0.0f;
        while (timeElapsed < delay)
        {
            transform.Translate(0, distance, 0);

            m_img.color = new Color(1f, 1f, 1f, 1 - timeElapsed / delay);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
