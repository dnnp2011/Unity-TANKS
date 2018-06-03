using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
	// Identifies the layer the tanks are on, eliminating the need to worry about damage values to the environment etc.
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
	public float m_MaxDamage = 100f, m_ExplosionRadius = 5f, m_MaxLifeTime = 2f, m_ExplosionForce = 1000f;                             

    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }

	private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_TankMask);

		for (int i = 0; i < colliders.Length; i++)
		{
			Rigidbody targetRigidbody = colliders [i].GetComponent <Rigidbody> ();

			if (!targetRigidbody)
				continue;

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			// Can call the script class of the script to reference, and can reference a component using another component of that object
			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

			if (!targetHealth)
				continue;

			float damage = CalculateDamage (targetRigidbody.position);

			targetHealth.TakeDamage (damage);
		}

		// Decouples the explosion particles from its parent so the parent can be deleted without removing the particle system itself
		m_ExplosionParticles.transform.parent = null;

		m_ExplosionParticles.Play ();

		m_ExplosionAudio.Play ();

		// Destroy these particles after the DURATION of this particle system
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }

	private float CalculateDamage(Vector3 targetPosition)
	{
		// Calculate the amount of damage a target should take based on it's position.
		// Get a vector from where the explosion is, to the target within its damage radius
		Vector3 explosionToTarget = targetPosition - transform.position;

		// Get the magnitude of that vector, the 'length' from the explosion to the target
		float explosionDistance = explosionToTarget.magnitude;

		// Ratio of the targets distance from the total explosion radius. The percentage of the explosion radius that the target is from the epicenter.
		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		// Multiply the damage ratio by the max damage, so the closer the target is, the more damage it shall incur.
		float damage = relativeDistance * m_MaxDamage;

		damage = Mathf.Max (0f, damage);

		return damage;
    }
}