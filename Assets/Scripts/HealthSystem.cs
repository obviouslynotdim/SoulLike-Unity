using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f; // Added maxHealth for calculations
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float stunDuration = 0.5f;

    private Animator enemyAnim;
    private NavMeshAgent agent;
    private bool isDead = false;

    void Start()
    {
        maxHealth = health; // Set maxHealth to starting health
        enemyAnim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public bool IsDead() => isDead;

    // Helper for the Boss UI to get a 0-1 value for the slider
    public float GetHealthPercentage() => health / maxHealth;

    public void TakeDamage(float damage, Vector3 attackerPosition)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log($"<color=red>HIT!</color> {gameObject.name} lost {damage} HP. Remaining Health: {health}");

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (enemyAnim != null) enemyAnim.SetTrigger("Hit");

            StopAllCoroutines();
            StartCoroutine(ApplyHitEffects(attackerPosition));
        }
    }

    private IEnumerator ApplyHitEffects(Vector3 attackerPosition)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            Vector3 knockbackDir = (transform.position - attackerPosition).normalized;

            float timer = 0;
            while (timer < 0.2f)
            {
                agent.Move(knockbackDir * knockbackForce * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(stunDuration);
            if (!isDead) agent.isStopped = false;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        if (enemyAnim != null) enemyAnim.SetTrigger("Die");
        if (TryGetComponent(out Collider col)) col.enabled = false;

        Invoke("DisableAgent", 0.1f);
        Destroy(gameObject, 5f);
    }

    private void DisableAgent()
    {
        if (agent != null) agent.enabled = false;
    }
}