using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Collider swordCollider;

    void Start() => swordCollider = GetComponent<Collider>();

    // Call these from Animation Events
    public void EnableWeapon() => swordCollider.enabled = true;
    public void DisableWeapon() => swordCollider.enabled = false;

    private void OnTriggerEnter(Collider other)
    {
        // Step 1: Does the physics engine see the touch?
        Debug.Log("1. Physics: Touched " + other.name);

        if (other.CompareTag("Enemy"))
        {
            // Step 2: Is the Tag set correctly?
            Debug.Log("2. Tag: Confirmed object is an Enemy");

            HealthSystem enemy = other.GetComponent<HealthSystem>();
            if (enemy != null)
            {
                // Step 3: Is the HealthSystem script found?
                Debug.Log("3. Script: Found HealthSystem, sending damage");
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.Log("3. ERROR: No HealthSystem found on " + other.name);
            }
        }
    }
}