using UnityEngine;
using UnityEngine.UI;

public class ParticleBehavior : MonoBehaviour
{
     [SerializeField, Range(0f, 200f)] float m_force = 5f;

    [SerializeField] Rigidbody2D m_rb2D;

    public delegate void OnOnParticleRemoveDelegate();
    public static event OnOnParticleRemoveDelegate OnParticleRemove;

    private void Start()
    {
        Spawning();
    }

    private void Update()
    {
        if (transform.position.y <= -50f)
        {
            OnParticleRemove.Invoke();
            Destroy(gameObject);
        }
    }

    void Spawning()
    {
        m_rb2D.velocity = new Vector2(Random.value - 0.5f, Random.value) * m_force;
    }
}
