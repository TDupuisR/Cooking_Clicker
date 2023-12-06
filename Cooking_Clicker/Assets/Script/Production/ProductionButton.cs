using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameManagerSpace;
using Unity.VisualScripting;

public class ProductionButton : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] Image m_productImage;
    [SerializeField] Button m_productionButton;
    [SerializeField] TMP_Text m_productName;
    [SerializeField] TMP_Text m_productAmount;
    [SerializeField] Slider m_progressionSlider;
    [Space(5)]
    [SerializeField] TMP_Text m_productPriceText;

    [Header("Field")]
    [SerializeField] int m_productType;
    [SerializeField] int m_progression;
    [SerializeField] float m_progressionTime;
    [Space(5)]
    [SerializeField] bool m_isUnlocked;
    [SerializeField] GameObject m_lockedObject;
    [SerializeField] uint m_ingredientPrice;
    Coroutine m_autoCoRoutine;

    [Header("Event")]
    [SerializeField] UnityEvent OnProgression;
    [SerializeField] UnityEvent OnCompletion;

    public delegate void OnTouchdelegate(Vector2 spawnPos, int[] id);
    public static event OnTouchdelegate OnTouch;

    private void OnEnable()
    {
        RessourceManager.OnUnlockButton += RessourceManager_OnUnlockButton;
    }

    private void Start()
    {
        m_productName.text = GameManager.ressourceManager.ReturnRessourceName(m_productType);
        m_productImage.sprite = GameManager.ressourceManager.ReturnRessourceSprite(m_productType);
        m_productPriceText.text = m_ingredientPrice.ToString() + " $";
    }

    private void FixedUpdate()
    {
        m_productAmount.text = GameManager.ressourceManager.ressourcesAmount[m_productType].ToString();
        m_progressionSlider.value = m_progression;

        if(m_progression > 100)
        {
            m_progression = 0;
            GameManager.ressourceManager.ressourcesAmount[m_productType]++;
            OnCompletion.Invoke();
        }
    }

    public void CheckIfCanBuy()
    {
        if(GameManager.Instance.Money >= m_ingredientPrice)
        {
            GameManager.Instance.Money -= m_ingredientPrice;
            UnlockButton();
        }
    }
    private void RessourceManager_OnUnlockButton(int obj)
    {
        if (obj == m_productType)
            UnlockButton();
    }
    void UnlockButton(bool save = false)
    {
        m_autoCoRoutine = StartCoroutine(AutoProgression());
        m_productionButton.interactable = true;
        m_isUnlocked = false;
        m_lockedObject.SetActive(false);

        if (save) 
            GameManager.ressourceManager.SaveIngredients(m_productType);
        GameManager.dishManager.availableIngredients.Add(GameManager.ressourceManager.ReturnRessource(m_productType));
        GameManager.dishManager.CheckForNewDish();
    }

    public void SpeedProgression(int amount)
    {
        m_progression += amount;
        OnProgression.Invoke();

        int[] id = { m_productType };
        OnTouch.Invoke(transform.position, id);
    }

    IEnumerator AutoProgression()
    {
        yield return new WaitForSeconds(m_progressionTime);
        m_progression++;
        StartCoroutine(AutoProgression());
    }
}
