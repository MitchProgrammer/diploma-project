using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    private NavMeshAgent agent;

    public enum state { idle, followingPlayer}
    public state aiState;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > followDistance)
        {
            aiState = state.followingPlayer;
            agent.SetDestination(player.position);
        }
        else
        {
            aiState = state.idle;
            agent.ResetPath();
        }
    }
}