using System.Collections;
using UnityEngine;



public class Controller : MonoBehaviour
{
    public Animator animator;
    public float rollDistance = 10f; // Distance the player moves with each tap
    public float rollTime = 0.25f;   // Duration of the roll (in seconds)
    public bool isRolling = false; // Tracks if a roll is already in progress
    public bool isAttacking = false;  //attacking and parrying checks
    public bool isParrying = false;
    public AudioSource parrySound; 

    //public GameObject lightBlock; 
   // public UnityEngine.Rendering.Universal.Light2D Light;

    public static bool inCombat = false;

    public GameObject enemyPrefab;  // enemy prefab
    public Transform spawnPoint;    // unused, ran out of time
    private float runTimer = 0f;

    // combo attack ( right now just tracks second animation, no damage difference)
    private bool canComboAttack = false; // Tracks if a combo attack can be triggered
    private float comboWindow = 0.5f; // Time window to trigger the combo

    public Vector3 centerPosition = new Vector3(0, -2.0f, 0);

    public int attackDamage = 1;         

    //public Light2D parryLight;     

    void Start () {
        //lightBlock = this.transform.Find("parryLight").gameObject;
       // light = lightBlock.GetComponent<Light>();
        GameManager.Instance.setController(this);
    }
    void Update()
    {
        //GetComponent<Light>().enabled = true;
        if (!inCombat)
        {
            animator.SetBool("inCombat", false);
            // Update run timer
            runTimer += Time.deltaTime;


            transform.position = Vector3.Lerp(transform.position, centerPosition, 1 * Time.deltaTime);

            // runs into enemy after 5 seconds of running
            if (runTimer >= 5f)
            {
                
                animator.SetBool("inCombat", true);
                inCombat = true;
                GameObject clone = Instantiate<GameObject>(enemyPrefab);
                runTimer = 0f;
            }
        }

        // left roll
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !isRolling && inCombat)
        {
            
            animator.SetInteger("Direction", -1);
            StartCoroutine(Roll(Vector3.left));
        }

        // right roll
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isRolling && inCombat)
        {
            
            animator.SetInteger("Direction", 1);
            StartCoroutine(Roll(Vector3.right));
        }

        // attacking
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isRolling && inCombat)
        {
            
            if (canComboAttack)
            {
                StartCoroutine(ComboAttack()); // Combo attack 
                
                parrySound.Play();
            } else if (!isAttacking) {
                StartCoroutine(Attack()); //
                
                parrySound.Play();
            }
        }

        // parrying
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isRolling && !isParrying && inCombat) 
        {
            StartCoroutine(Parry());
        }

        // update anims
        animator.SetBool("Rolling", isRolling);
        animator.SetBool("Attacking", isAttacking);
    }

    //proper rolling mechanics
    private IEnumerator Roll(Vector3 direction)
    {
        isRolling = true; // Set rolling flag
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * rollDistance;
        float elapsedTime = 0f;

        // goes the set distance over time
        while (elapsedTime < rollTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / rollTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isRolling = false;
    }

    // first default attack
    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        Opponent opponent = GameManager.Instance.GetOpponent();

        if (opponent != null)
        {
            opponent.TakeDamage(attackDamage); // Deal damage to the current opponent
            Debug.Log("Dealt " + attackDamage + " damage to opponent!");
        }

        yield return new WaitForSeconds(0.15f); // attack duration

        // combo time
        canComboAttack = true;
        
        isAttacking = false;

        yield return new WaitForSeconds(comboWindow);
        canComboAttack = false; // Reset combo if not used

        
    }

    // second attack
    private IEnumerator ComboAttack()
    {
        canComboAttack = false; // Prevent further combos
        animator.SetTrigger("ComboAttack");
        isAttacking = true;
        yield return new WaitForSeconds(0.15f); 

        isAttacking = false;
        Debug.Log("Combo attack executed!");
    }

    // parrying!!
    private IEnumerator Parry()
    {
        isParrying = true;
        animator.SetBool("Parrying", isParrying);


        
        yield return new WaitForSeconds(.1f);
        animator.SetBool("Parrying", false);
        yield return new WaitForSeconds(1f); // 1 s window

        isParrying = false;
        OnDownArrowPress();
    }

    // unused, ran out of time for more features.
    private void OnUpArrowPress()
    {
        
        Debug.Log("Up arrow pressed - Attack triggered");
    }

    private void OnDownArrowPress()
    {
        Debug.Log("Down arrow pressed - Parry triggered");
    }

    
}
