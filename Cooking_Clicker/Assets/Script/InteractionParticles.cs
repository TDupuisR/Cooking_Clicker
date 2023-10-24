using UnityEngine;
using UnityEngine.UI;
using GameManagerSpace;
using NaughtyAttributes;

public class InteractionParticles : MonoBehaviour
{
    short m_maxAmount = 50;
    short m_amount = 0;

    [SerializeField] GameObject m_particle;
    [SerializeField] GameObject m_parentFolder;

    private void OnEnable()
    {
        ParticleBehavior.OnParticleRemove += RemoveCount;

        ProductionButton.OnTouch += LaunchParticles;
        PreparationButton.OnTouch += LaunchParticles;
    }
    private void OnDisable()
    {
        ParticleBehavior.OnParticleRemove -= RemoveCount;

        ProductionButton.OnTouch -= LaunchParticles;
        PreparationButton.OnTouch -= LaunchParticles;
    }

    private void LaunchParticles(Vector2 launchPosition, int[] id)
    {
        int lenId = id.Length;
        for ( int i = 0; i < 5; i++ )
        {
            if (m_amount < m_maxAmount)
            {
                GameObject currentParticle = Instantiate(m_particle, m_parentFolder.transform);
                currentParticle.transform.position = launchPosition;

                int chosenId = Random.Range(0, lenId);
                currentParticle.GetComponent<Image>().sprite = GameManager.ressourceManager.ReturnRessourceSprite(id[chosenId]);

                //Debug.Log("Name: " + currentParticle.transform.name + " |ID: " + id[chosenId] + "|Index: " + chosenId);

                m_amount++;
            }
        }
    }

    private void RemoveCount()
    {
        m_amount--;
    }

    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            LaunchParticles(new(125, 250), -1);
        }
    }
    */
}
