using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opponent : MonoBehaviour
{
    public int health = 5; // Health of the opponent
    private bool isInvincible = false; // Invincibility status during attack
    private bool isStunned = false; // Status of being stunned
    private Animator animator; 
    
    // Reference to the player's controller
    
    // Timer interval for attacks
    public float attackInterval = 3f;

    public AudioSource parrySound; 

    public Controller controller;
    
    // Possible attack types
    private enum AttackType { RightAttack, LeftAttack, Overhead }
    
    void Start()
    {
        controller = GameManager.Instance.getController(); 
        animator = GetComponent<Animator>();
        GameManager.Instance.SetOpponent(this);
        // Start repeating the attack routine
        //parrySound = GetComponent<AudioSource>();   
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        
    }

    private IEnumerator AttackRoutine()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(attackInterval);
            if (!isStunned)
            {
                PerformAttack();
            }
        }
    }

    private void PerformAttack()
    {
        isInvincible = true;

        // attack type
        AttackType attackType = (AttackType)Random.Range(0, 3);
        
        // attack animation
        switch (attackType)
        {
            case AttackType.RightAttack:
                animator.SetTrigger("RightAttack");
                break;
            case AttackType.LeftAttack:
                animator.SetTrigger("LeftAttack");
                break;
            case AttackType.Overhead:
                animator.SetTrigger("OverheadAttack");
                break;
        }

        // wind up phase
        StartCoroutine(CheckPlayerStateAfterWindup());
    }
// damage phase
    private IEnumerator CheckPlayerStateAfterWindup()
    {
        // Wait for the wind-up duration before checking player state
        yield return new WaitForSeconds(1f);
        
        if (controller != null)
        {
            if (controller.isParrying)
            {
                // If the player is parrying, stun the opponent
                parrySound.Play();

                Stun();
            }
            else if (controller.isRolling)
            {
                // If the player is rolling, do nothing (attack misses)
                Debug.Log("rolled!");
            }
            else
            {
                // Otherwise, the attack hits and kills the player
                //controller.Die();
                Debug.Log("died!");
                SceneManager.LoadScene( "startScene" );
            }
        } else {
            Debug.Log("no controller!");
        }

        // End the attack after checking the player's state
        StartCoroutine(EndAttack());
    }

    private IEnumerator EndAttack()
    {
        // Wait for the duration of the attack animation
        yield return new WaitForSeconds(0.5f);
        
        isInvincible = false; // End invincibility after attack
    }

    // applies damage on hit
    public void TakeDamage(int damage)
    {
        if (isStunned) // death  if hit while stunned
        {
            Die();
        }
        else if (!isInvincible) // else reduce health
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    private void Stun()
    {
        isStunned = true;
        animator.SetTrigger("Stunned");
        
        //Stun routine
        StartCoroutine(EndStun());
    }

    private IEnumerator EndStun()
    {
        yield return new WaitForSeconds(3f); // 3 second stun
        isStunned = false;
    }

    private void Die()
    {
        // death animation and disable the opponent
        animator.SetTrigger("Death");

        Controller.inCombat = false;
        
        
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
