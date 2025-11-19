using UnityEngine;
using UnityEngine.AI;

public class BaseNPC : MonoBehaviour
{
    public Stats myStats;
    public int currentHealth;

    public bool isDead = false;    
    
    public MovementState myMoveState;
    public bool isAggro = false;

    [Header("Player Detection")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] float timeBetweenChecks = 1f;
    float checkTimer = 0f;

    [Header("Movement")]
    [SerializeField] NavMeshAgent navAgent;
    public LayerMask groundLayer;
    [SerializeField] float currentSpeed = 0;
    public float walkSpeed = 4f;
    public float runSpeed = 7.5f;
    public float maxSpeed = 7.5f; // change both of these when using navmesh

    public Animator myAnimator;
    public string animSpeedString = "speed";
    [SerializeField] float currentAnimSpeed = 0f; // for lerping
    [SerializeField] float animSpeedDamp = 20f;
    [SerializeField] float rotDamp = 20f;

    Vector3 walkPoint;
    bool walkPointSet = false;
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float wanderIdleTime = 2f; // add random offset
    float wanderIdleTimer = 0f;

    [Header("Aggro")]
    public AttackType attackType;
    public Transform targetT;
    [SerializeField] float pursueRadius = 10f; // change to a cone of vision
    [SerializeField] float attackRadius = 5f; // change to a cone of vision
    [SerializeField] float stopRadius = 2f;
    [SerializeField] float aggroTime = 5f; // use once player is out of range
    float aggroTimer = 0f;

    [Header("Attacking")]
    public SpellManager spellManager;
    public Transform attackPoint;
    float attackTimer = 0f;
    public GameObject projectileGo;
    public float projForce = 100f;

    [Header("Health")]
    HealthManager healthManager;

    [Header("Visuals")]
    [SerializeField] Transform rbRoot;
    [SerializeField] GameObject hurtGo;
    [SerializeField] GameObject deadGo;

    public enum MovementState
    {
        IDLE,
        WANDER,
        PURSUE,
        ATTACK,
        FLEE
    }

    public void initHealth()
    {
        healthManager = GetComponent<HealthManager>();
        healthManager.InitNpcHealth(this);
    }

    public virtual void PlayerDetector()
    {
        if (myMoveState == MovementState.PURSUE || !isAggro)
        {
            checkTimer = 0f;
            return;
        }
        checkTimer += Time.deltaTime;
        if (checkTimer > timeBetweenChecks)
        {
            if (targetT != null)
            {
                if (Vector3.Distance(targetT.position, transform.position) <= pursueRadius)
                {
                    myMoveState = MovementState.PURSUE;
                }
            }
            else
            {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, pursueRadius, transform.forward, 0f);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject.CompareTag(playerTag))
                    {
                        targetT = hit.transform;
                        myMoveState = MovementState.PURSUE;
                    }
                }
            }
                

            checkTimer = 0f;
        }
    }

    public virtual void MovementManager()
    {
        if (isDead)
        {
            if (navAgent.isActiveAndEnabled) navAgent.enabled = false;
            return;
        } // dont move if dead

        switch (myMoveState) 
        {
            case MovementState.IDLE:
                //
            break;

            case MovementState.WANDER:
                Wander();
            break;

            case MovementState.PURSUE:
                PursuePlayer();
            break;

            case MovementState.ATTACK:
                Attack();
            break;

            case MovementState.FLEE:
                //
            break;

        }
    }

    void Wander()
    {
        // find point to wander to
        float distToPos = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(walkPoint.x, walkPoint.z));
        if (!walkPointSet)
        {
            currentSpeed = 0f;
            navAgent.speed = 0f;

            if (wanderIdleTimer < Random.Range(wanderIdleTime * 0.8f, wanderIdleTime * 1.2f))
            {
                wanderIdleTimer += Time.deltaTime;
                UpdateAnimator();
                return;
            }            

            navAgent.destination = FindWalkPoint();

            walkPointSet = true;
        }
        else
        {
            currentSpeed = walkSpeed;
            navAgent.speed = walkSpeed;

            if (distToPos < 0.3f)
            {
                currentSpeed = 0f;
                wanderIdleTimer = 0f;
                walkPointSet = false;
            }

            //rotate to walkPoint smoothly
            //Vector3 dir = (walkPoint - transform.position).normalized;
            //transform.rotation = SmoothRotateToTarget(dir);
        }
        UpdateAnimator();
    }

    Vector3 FindWalkPoint()
    {
        walkPoint = transform.position;

        Vector3 randomPos = transform.position + Random.insideUnitSphere * wanderRadius;
        randomPos.y = 20f;

        RaycastHit hit;
        if (Physics.Raycast(randomPos, Vector3.down, out hit, 40f, groundLayer))
        {
            walkPoint = hit.point;
        }

        return walkPoint;
    }

    void PursuePlayer()
    {
        float distToPos = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetT.position.x, targetT.position.z));

        if (distToPos > pursueRadius)
        {
            aggroTimer += Time.deltaTime;
            if (aggroTimer > aggroTime)
            {
                myMoveState = MovementState.WANDER;
                aggroTimer = 0f;
            }
        }
        else
        {
            aggroTimer = 0f;
        }

            //move to new point
        navAgent.SetDestination(targetT.position);

        Vector3 dir = (walkPoint - transform.position).normalized;

        if (distToPos < attackRadius)
        {
            currentSpeed = 0f;
            navAgent.speed = 0f;

            myMoveState = MovementState.ATTACK;
        }
        else
        {
            currentSpeed = runSpeed;
            navAgent.speed = runSpeed;
        }

        UpdateAnimator();
        //rotate to walkPoint smoothly
        //transform.rotation = SmoothRotateToTarget(dir);
    }

    void Attack()
    {
        UpdateAnimator();

        navAgent.SetDestination(targetT.position);

        float distToPos = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetT.position.x, targetT.position.z));
        Vector3 dir = (targetT.position - attackPoint.position).normalized;

        //move controller
        if (distToPos <= stopRadius) // very close
        {
            navAgent.speed = 0f;
            currentSpeed = 0f;
        }
        else if (distToPos < attackRadius) // run and gun
        {
            currentSpeed = runSpeed;
            navAgent.speed = runSpeed;
        }
        else
        {
            myMoveState = MovementState.PURSUE;
        }

        //transform.rotation = SmoothRotateToTarget(dir);

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0) // attack
        {
            Fire(dir);
            attackTimer = spellManager.currentSpell.spellFireRate;
        }

    }

    void Fire(Vector3 dir)
    {
        spellManager.Cast();
    }

    Quaternion SmoothRotateToTarget(Vector3 dir)
    {
        Quaternion desiredRot = Quaternion.LookRotation(dir);
        Quaternion slerpRot = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * rotDamp);
        return slerpRot;
    }

    void UpdateAnimator()
    {
        if (myAnimator == null) return;

        float speedPercent = currentSpeed / maxSpeed;
        speedPercent = Mathf.Clamp(speedPercent, 0f, 1f);

        currentAnimSpeed = Mathf.Lerp(currentAnimSpeed, speedPercent, Time.deltaTime * animSpeedDamp);

        myAnimator.SetFloat(animSpeedString, currentAnimSpeed);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(walkPoint, 1f);
        Gizmos.DrawLine(walkPoint, transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pursueRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
[System.Serializable]
public class Stats
{
    public string mercName;
    public int powerLevel;
    public int rank;

    public int currentMoney;

    public int maxHealth;
    public int damage;
    public int accuracy;
    public int speed;

}

public enum AttackType //add more in future
{
    MELEE,
    RANGED
}