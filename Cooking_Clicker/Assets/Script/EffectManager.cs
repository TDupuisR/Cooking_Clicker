using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField] Transform m_customerParent;
    [SerializeField] GameObject m_effectPrefab;
    [SerializeField] List<Sprite> m_effectSprite;
    public enum _effectImg
    {
        NOTHAPPY,
        CASH
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }

    public void CreateEffect(Vector3 position, _effectImg image)
    {
       GameObject newEffect = Instantiate(m_effectPrefab, position, Quaternion.identity);
       newEffect.transform.SetParent(m_customerParent);
       newEffect.GetComponent<Image>().sprite = m_effectSprite[(int) image];
    }
}
