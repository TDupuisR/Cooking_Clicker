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
                m_moneyText.text = value.ToString();
            }
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            ressourceManager = m_ressourceManager;
            dishManager = m_dishManager;
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }

        public DishBehavior GetRandomDish()
        {
            return m_dishManager.ReturnRandomDish();
        }
    }

    public static class GameManagerStatic
    {
        //Trouver une solution pour r�cup�rer les infos de mani�re s�curiser via une method!!!!!
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