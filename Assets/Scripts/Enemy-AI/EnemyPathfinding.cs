using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    // Refrences
    [Header("Refrences")]
    public UnityEngine.AI.NavMeshAgent agent;
    private GameObject player;
    [Space]
    public LayerMask playerMask;
    public LayerMask ground;

    // Enemy States
    public enum EnemyState { idling, patrolling, chasing };
    [Header("Enemy State")]
    public EnemyState state;

    // Patrolling
    [Header("Patrolling")]
    public Vector3 patrolPoint;
    public float patrolRadius;
    [Space]
    public bool patrolPointSet;
    [Space]
    public float maxMagnitude;

    // Chasing
    [Header("Chasing")]
    public float playerChaseRadius;
    [Space]
    public bool playerInChaseRange;

    // Idling
    [Header("Idling")]
    public float idleStateChance;
    [Space]
    public float minIdleTime;
    public float maxIdleTime;
    [Space]
    public bool allowIdle;

    // Movement Speed
    [Header("Movement Speed")]
    public float walkSpeed;
    public float runSpeed;

    // Movement Acceleration
    [Header("Movement Acceleration")]
    public float walkAcceleration;
    public float runAcceleration;

    // Gizmo Config
    private float gizmosPatrolRadius;
    private float gizmosChaseRadius;

    // Start is called before the first frame update
    public void Start()
    {
        // Setting variables to default values
        patrolPointSet = false;
        playerInChaseRange = false;
        allowIdle = true;

        // Setting enemy state to default
        state = EnemyState.patrolling;

        // Setting gizmos values
        gizmosPatrolRadius = patrolRadius;
        gizmosChaseRadius = playerChaseRadius;

        // Get player gameobject
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    public void Update()
    {
        UpdateStates();

        if (playerInChaseRange) { state = EnemyState.chasing; Chasing(); }
    }

    public void UpdateStates()
    {
        // Checks if enemy is idle, if it is, return as the enemy doesn't need to do anything if it's idle
        if (!allowIdle) { return; }

        // Checks if the player is within the player chase range with radius playerChaseRadius
        playerInChaseRange = Physics.CheckSphere(transform.position, playerChaseRadius, playerMask);

        // If the player is in the chase range, start chasing. Else, start/continue patrolling
        if (playerInChaseRange) { Chasing(); }
        else { Patrolling(); }
    }

    public void Patrolling()
    {
        // Reset the patrolPointing the walkSpeed and Acceleration in state patrolling
        agent.speed = walkSpeed;
        agent.acceleration = walkAcceleration;

        // Setting enemy state to patrolling
        state = EnemyState.patrolling;

        // Setting playerInChaseRange to false
        playerInChaseRange = false;

        // If enemy has not set a patrolPoint and can idle, call function IsleState()
        if (!patrolPointSet && allowIdle) { StartCoroutine(IdleState()); }

        // If the patrolPoint is set, set the desination to said patrolPoint
        if (patrolPointSet) { agent.SetDestination(patrolPoint); }

        // Find the distance between current enemy position and patrolPoint position
        Vector3 distanceToPatrolPoint = transform.position - patrolPoint;

        // If the distance to the patrol point is smaller than a certain magn
        if (distanceToPatrolPoint.magnitude < maxMagnitude) { patrolPointSet = false; }
    }

    public IEnumerator IdleState()
    {
        // Disallows the enemy to idle
        allowIdle = false;

        // Picks random number, if the number is <= idleStateChance then it idles, if not it sets the patrol point
        if (Random.Range(1, idleStateChance) <= (idleStateChance - 1)) { SetPatrolPoint(); yield break; }

        state = EnemyState.idling;

        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));

        SetPatrolPoint();
    }

    public Vector3 SetPatrolPoint()
    {
        // Allowing the enemy to idle
        allowIdle = true;

        // Gets a random direction inside of a sphere of radius patrolRadius and gets the position of the enemy and sets that to origin
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        Vector3 origin = transform.position;

        // Adds origin to randomDirection
        randomDirection += origin;

        // Creates a object NavMeshHit
        UnityEngine.AI.NavMeshHit navHit;

        // Finds nearest point based on the NavMesh in a range of patrolRadius and we get that value as variable navHit
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, UnityEngine.AI.NavMesh.AllAreas);

        // Sets the patrol point to the random position inside of patrolRadius
        patrolPoint = navHit.position;

        // If the patrolPoint position is suitable, set patrolPointSet to true
        if (Physics.Raycast(patrolPoint, -transform.up, 2f, ground)) { patrolPointSet = true; }

        // Gets distance between patrolPoint and the enemy position
        Vector3 distanceToPatrolPoint = transform.position - patrolPoint;

        // If the patrolPoint is too close to the enemy, we pick a new position that is further away
        if (distanceToPatrolPoint.magnitude < 2.5f) { patrolPointSet = false; }
        else { patrolPointSet = true; }

        // Returns patrolPoint
        return patrolPoint;
    }

    public void Chasing()
    {
        // Setting the runSpeed and Acceleration in state chasing
        agent.speed = runSpeed;
        agent.acceleration = runAcceleration;

        // Setting enemy state to chasing
        state = EnemyState.patrolling;

        // Setting playerInChaseRange to true
        playerInChaseRange = true;

        // Set the destination of the Enemy to the player's position
        agent.SetDestination(player.transform.position);
    }

    void OnDrawGizmos()
    {
        // Draw Patrol Radius in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmosPatrolRadius);

        // Draw Chase Radius in red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, gizmosChaseRadius);

        // Draw Rotation Direction in magenta
        Gizmos.color = Color.magenta;
        Vector3 endpoint = transform.position + transform.forward * 2f;
        Gizmos.DrawLine(transform.position, endpoint);
    }
}
