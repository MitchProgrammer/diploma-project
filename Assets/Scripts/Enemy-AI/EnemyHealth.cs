using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public Rigidbody rb;

    public double health;

    public bool dead;

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Damage(50);
        }
    }

    public void Damage(double damage)
    {
        health -= damage;
        KillCheck();
    }

    public void KillCheck()
    {
        if (health <= 0) { Die(); }
    }

    public void Die()
    {
        dead = true;

        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;

        MonoBehaviour[] components = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components) { component.enabled = false; }

        if (TryGetComponent(out navMeshAgent)) { navMeshAgent.enabled = false; }
    }
}
