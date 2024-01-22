using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace GameManagerSpace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static RessourceManager ressourceManager;
        public static SoundManager soundManager;
        public static DishManager dishManager;

        [SerializeField] RessourceManager m_ressourceManager;
        [SerializeField] DishManager m_dishManager;
        [SerializeField] SoundManager m_soundManager;
        [SerializeField] TMP_Text m_moneyText;

        [SerializeField] float m_multiplierAdd;
        [SerializeField] float m_multiplier = 1.0f;
        private uint m_money;
        private int m_currentPanel; // 0 = service // 1 = cooking // 2 = production //
        public uint Money
        {
            get => m_money; 
            set
            {
                m_money = value;
                PlayerPrefs.SetInt("money", (int)m_money);
                PlayerPrefs.Save();

                m_moneyText.text = value.ToString();
            }
        }
        public float Multiplier
        {
            get => m_multiplier;
            set => m_multiplier = value;
        }
        public float MultiplierAdd
        {
            get => m_multiplierAdd;
        }
        public int CurrentPanel
        {
            get => m_currentPanel;
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            ressourceManager = m_ressourceManager;
            dishManager = m_dishManager;
            soundManager = m_soundManager;

            
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_ressourceManager.LoadIngredients();
            m_dishManager.CheckForNewDish();
            LoadMoney();
        }

        public DishBehavior GetRandomDish()
        {
            return m_dishManager.ReturnRandomDish();
        }

        private void LoadMoney()
        {
            if (PlayerPrefs.HasKey("money"))
            {
                Money = (uint)PlayerPrefs.GetInt("money");
            }
            else
            {
                Money = 0;
            }
        }

        [Button]
        public void GiveMoney() => Money += 100;

        [Button]
        public void ResetPlayerPrefs() => PlayerPrefs.DeleteAll();

        public void ChangePanel(int newPannel)
        {
            m_currentPanel = newPannel;
        }
    }

    public static class GameManagerStatic
    {
        public enum RessourcesNames
        {
            Tomato, Carrot, Potato, Pea, Bean, Veal, Beef, Chicken, Duck, Sheep,
            Milk, Butter, Rice, Pasta, Egg
        }

        public enum CookingMachines
        {
            Furnace, Pan, Cooking_pot, Blender, Fryer, Plancha, Grill, Microwave, fridge
        }

        public enum DishStates
        {
            Wait, Prep, QueueCook, Cook, Serve
        }
    }
}