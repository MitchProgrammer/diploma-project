using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class PetAI : MonoBehaviour
{
    public Transform player;
    public float followDistance = 3f;
    private NavMeshAgent agent;
    public Animator animator;

    public TextMeshProUGUI happyText;

    public enum state { idle, followingPlayer}
    public state aiState;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        happyText.text = "100% Happiness";
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > followDistance)
        {
            aiState = state.followingPlayer;
            agent.SetDestination(player.position);
            animator.SetBool("idle", false);
            animator.SetBool("running", true);
        }
        else
        {
            aiState = state.idle;
            agent.ResetPath();
            animator.SetBool("idle", true);
            animator.SetBool("running", false);
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            animator.SetTrigger("lying down");
            happyText.text = "0% Happiness";
            agent.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animator.SetTrigger("backflip");
        }
    }
}