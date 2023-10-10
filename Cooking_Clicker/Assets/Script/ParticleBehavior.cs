using UnityEngine;

public class ParticleBehavior : MonoBehaviour
{
     float m_force = 5f;

    [SerializeField] Rigidbody2D m_rb2D;

    public delegate void OnOnParticleRemoveDelegate();
    public static event OnOnParticleRemoveDelegate OnParticleRemove;

    private void OnEnable()
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
        float sideForce = (Random.value) * 0.5f;
        m_rb2D.velocity = new Vector2(sideForce, 1f) * m_force;
    }
}
