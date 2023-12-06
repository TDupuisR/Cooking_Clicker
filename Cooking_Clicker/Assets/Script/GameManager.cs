using TMPro;
using UnityEngine;

namespace GameManagerSpace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static RessourceManager ressourceManager;
        public static DishManager dishManager;

        [SerializeField] RessourceManager m_ressourceManager;
        [SerializeField] DishManager m_dishManager;
        [SerializeField] TMP_Text m_moneyText;

        private uint m_money;
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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            ressourceManager = m_ressourceManager;
            dishManager = m_dishManager;

            m_ressourceManager.LoadIngredients();
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
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
    }

    public static class GameManagerStatic
    {
        //Trouver une solution pour récupérer les infos de manière sécuriser via une method!!!!!
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