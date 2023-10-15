using UnityEngine;

namespace GameManagerSpace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static RessourceManager ressourceManager;

        [SerializeField] RessourceManager m_ressourceManager;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            ressourceManager = m_ressourceManager;
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public static class GameManagerStatic
    {
        //Trouver une solution pour sécuriser via une method!!!!!
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
            Wait, Prep, Cook, Serve
        }
    }
}