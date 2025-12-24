using UnityEngine;
using UnityEngine.AI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private Animator enemyAnim;
    private bool isDead = false;

    void Start()
    {
        enemyAnim = GetComponent<Animator>();
    }

    // Public function so TigerBossAI can check status
    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("---------- HIT DETECTED ----------");
        Debug.Log($"<color=red>HIT!</color> {gameObject.name} lost {damage} HP. Remaining Health: {health}");

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (enemyAnim != null)
            {
                enemyAnim.SetTrigger("Hit");
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("!!!!!!!!!! ENEMY HAS DIED !!!!!!!!!!");

        // 1. STOP NAVIGATION & ROTATION
        if (TryGetComponent(out NavMeshAgent agent))
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        // 2. TRIGGER ANIMATION
        if (enemyAnim != null)
        {
            enemyAnim.SetTrigger("Die");
        }

        // 3. DISABLE PHYSICS
        if (TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }

        Invoke("DisableAgent", 0.1f);
        Destroy(gameObject, 5f);
    }

    private void DisableAgent()
    {
        if (TryGetComponent(out NavMeshAgent agent)) agent.enabled = false;
    }
}