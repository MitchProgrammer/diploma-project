using UnityEngine;
using UnityEngine.AI;

public class HumanoidAI : MonoBehaviour
{
    public float roamRadius = 20f; // Maximum distance for random waypoints
    public float idleTime = 2f;    // Idle duration before moving
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    public bool isIdle = true;    // Idle state
    public bool isWalking = false; // Walking state
    public bool isRunning = false; // Running state

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(IdleBeforeNextMove());
    }

    void Update()
    {
        if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
        {
            StartCoroutine(IdleBeforeNextMove());
        }

        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.K)) { animator.SetTrigger("death"); }
    }

    void PickRandomWaypoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            agent.destination = hit.position;

            // Randomly decide walking or running
            if (Random.value > 0.5f)
            {
                SetState(false, true, false); // Walking
                agent.speed = walkSpeed;
            }
            else
            {
                SetState(false, false, true); // Running
                agent.speed = runSpeed;
            }
        }
    }

    System.Collections.IEnumerator IdleBeforeNextMove()
    {
        agent.isStopped = true;
        SetState(true, false, false); // Idle
        yield return new WaitForSeconds(idleTime);
        agent.isStopped = false;
        PickRandomWaypoint();
    }

    void UpdateAnimator()
    {
        animator.SetBool("idle", isIdle);
        animator.SetBool("walking", isWalking);
        animator.SetBool("running", isRunning);
    }

    void SetState(bool idle, bool walking, bool running)
    {
        isIdle = idle;
        isWalking = walking;
        isRunning = running;
    }
}