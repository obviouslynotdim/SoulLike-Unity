using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private int baseDamage = 20;
    [SerializeField] private Collider weaponCollider;

    private bool isActive = false;
    private float activationTimer;
    private int currentDamage;

    private void Start()
    {
        if (weaponCollider == null)
            weaponCollider = GetComponent<Collider>();

        // Ensure it is a trigger and starts off
        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            activationTimer -= Time.deltaTime;
            if (activationTimer <= 0) DeactivateWeapon();
        }
    }

    public void ActivateWeapon(float duration, int damageOverride = 0)
    {
        isActive = true;
        activationTimer = duration;
        currentDamage = damageOverride > 0 ? damageOverride : baseDamage;

        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    public void DeactivateWeapon()
    {
        isActive = false;
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 1. Send damage to the player
                player.TakeDamage(currentDamage);

                // 2. Deactivate immediately so the boss doesn't hit multiple times in one swing
                DeactivateWeapon();
            }
        }
    }
}