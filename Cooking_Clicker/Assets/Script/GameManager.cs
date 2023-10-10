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
            Tomate, Carotte, Pomme_de_terre, Petit_pois, Haricot, Veau, Boeuf, Poulet, Canard, Mouton,
            Lait, Beurre, Riz, Pates, Oeufs
        }

        public enum CookingMachines
        {
            Furnace, Pan, Cooking_pot, Blender, Fryer, Plancha, Grill, Microwave, Firewood_furnace
        }
    }
}