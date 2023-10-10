using UnityEngine;
using UnityEngine.UIElements;
using GameManagerSpace;
using NaughtyAttributes;

public class InteractionParticles : MonoBehaviour
{
    short m_maxAmount = 150;
    short m_amount = 0;

    [SerializeField] GameObject m_particle;

    private void OnEnable()
    {
        ParticleBehavior.OnParticleRemove += RemoveCount;
    }
    private void OnDisable()
    {
        ParticleBehavior.OnParticleRemove -= RemoveCount;
    }

    private void LaunchParticles(Vector2 launchPosition, int id)
    {
        for ( int i = 0; i < 5; i++ )
        {
            if (m_amount < m_maxAmount)
            {
                GameObject currentParticle = Instantiate(m_particle);
                currentParticle.transform.position = launchPosition;
                currentParticle.GetComponent<Image>().sprite = GameManager.ressourceManager.ReturnRessourceSprite(id);
                m_amount++;
            }
        }
    }

    private void RemoveCount()
    {
        m_amount--;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            LaunchParticles(new(125, 250), 0);
        }
    }
}
