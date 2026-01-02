using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private HealthBar healthBar;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;

    [Header("Weapon")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject swordOnShoulder;

    [Header("Potion Settings")]
    [SerializeField] private int potionCount = 3;
    [SerializeField] private float healAmount = 30f;

    public bool isEquipping;
    public bool isEquipped;
    public bool isBlocking;
    public bool isAttacking;
    public bool isRolling;

    private float timeSinceAttack;
    public int currentAttack = 0;

    [Header("Roll")]
    public float rollCooldown = 1.2f;
    private float lastRollTime;
    private float equipTimeout = 1.2f;

    void Start()
    {
        if (healthBar == null) healthBar = FindObjectOfType<HealthBar>();

        // Update the UI immediately at the start
        healthBar?.UpdatePotionUI(potionCount);

        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    void Update()
    {
        // MASTER SWITCH: If dead, stop all local input logic
        if (healthBar != null && healthBar.CurrentHealth <= 0) return;

        timeSinceAttack += Time.deltaTime;

        Attack();
        Equip();
        Block();
        Roll();
        Heal();

        playerAnim.applyRootMotion = isRolling || isAttacking || isBlocking;
    }

    public void TakeDamage(int damage)
    {
        if (healthBar != null && healthBar.CurrentHealth > 0)
        {
            healthBar.TakeDamage(damage);

            if (healthBar.CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                playerAnim.SetTrigger("TakeDamage");
            }
        }
    }

    private void Die()
    {
        // 1. Play Death Animation
        if (playerAnim != null) playerAnim.SetTrigger("Die");

        // 2. DO NOT disable CharacterController here.
        // Instead, we let the MASTER SWITCH in Update() stop the movements.
        // This allows the Starter Assets gravity to keep pulling you down to the floor.

        // 3. UI Logic: Show "You Died" and unlock cursor
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Reset combat states
        isAttacking = false;
        isRolling = false;
        isBlocking = false;

        Debug.Log("Player has died!");
    }
    // --- UI BUTTON FUNCTIONS ---
    public void TryAgain()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    // --- EXISTING COMBAT LOGIC ---
    private void Equip()
    {
        if (isAttacking || isRolling) return;
        if (Input.GetKeyDown(KeyCode.R) && playerAnim.GetBool("Grounded"))
        {
            isEquipping = true;
            Invoke(nameof(ForceEndEquip), equipTimeout);
            playerAnim.SetTrigger("Equip");
        }
    }

    private void ForceEndEquip() => isEquipping = false;

    public void ActiveWeapon()
    {
        bool equip = !isEquipped;
        sword.SetActive(equip);
        swordOnShoulder.SetActive(!equip);
        isEquipped = equip;
    }

    public void Equipped() => isEquipping = false;

    private void Block()
    {
        if (Input.GetKey(KeyCode.Mouse1) && playerAnim.GetBool("Grounded") && !isRolling)
        {
            isBlocking = true;
            playerAnim.SetBool("Block", true);
        }
        else
        {
            isBlocking = false;
            playerAnim.SetBool("Block", false);
        }
    }

    private void Roll()
    {
        if (Input.GetKeyDown(KeyCode.Q) && playerAnim.GetBool("Grounded") && !isRolling)
        {
            if (healthBar != null && !healthBar.HasStamina(20f)) return;
            if (Time.time <= lastRollTime + rollCooldown) return;

            Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
            Vector3 inputDir = (camForward * Input.GetAxisRaw("Vertical") + camRight * Input.GetAxisRaw("Horizontal")).normalized;

            if (inputDir.magnitude > 0.1f) transform.rotation = Quaternion.LookRotation(inputDir);

            isRolling = true;
            lastRollTime = Time.time;
            playerAnim.SetTrigger("Roll");
            healthBar?.DeductStamina(20f);
            isBlocking = false;
            playerAnim.SetBool("Block", false);
        }
    }

    public void EndRoll() => isRolling = false;

    private void Heal()
    {
        // Check for H key, potions remaining, and if player is hurt
        if (Input.GetKeyDown(KeyCode.H) && potionCount > 0 && healthBar.CurrentHealth < 100)
        {
            potionCount--;
            healthBar.Heal(healAmount);

            // Update the UI text after using a potion
            healthBar.UpdatePotionUI(potionCount);

            // Optional: playerAnim.SetTrigger("Heal");
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && playerAnim.GetBool("Grounded") && timeSinceAttack > 0.8f && isEquipped && !isRolling)
        {
            if (healthBar != null && !healthBar.HasStamina(10f)) return;
            currentAttack = (timeSinceAttack > 1.2f || currentAttack >= 3) ? 1 : currentAttack + 1;
            playerAnim.SetTrigger("Attack" + currentAttack);
            timeSinceAttack = 0f;
            isAttacking = true;
            healthBar?.DeductStamina(10f);
        }
    }

    public void ResetAttack() => isAttacking = false;
}