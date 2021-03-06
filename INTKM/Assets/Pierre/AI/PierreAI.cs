using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PierreAI : PierreActor
{
    // Enable / disable AI brain
    public NavMeshAgent agent;

    protected override void Start()
    {
        base.Start();
    }

    public override void Die()
    {
        base.Die();
        agent.isStopped = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {


        // no brain -> retarded
        if (!activated)
        {
            AnimPlayIdle();
            return;
        }

        // Get it's new target if it can
        if (!IsDead())
            target = MAI.FindTarget(this);
        // Movement Force
        Vector3 moveForce = Vector3.zero;


        // We have a target to attack ! YEAY !
        if (target != null)
        {
            agent.isStopped = false;
            agent.speed = speed;

            // Get Normalized direction
            Vector3 direction = (target.transform.position - transform.position);
            direction.y = 0;
            direction.Normalize();


            // Flocking mechanisim

            /*
            List<PierreActor> actors = MAI.GetAllActors();
            Vector3 flock = new Vector3(0, 0, 0);

            foreach (PierreActor a in actors)
            {
                // Ignores the target and him self
                if(a != target && a != this)
                {
                    Vector3 dir = (a.transform.position - transform.position);
                    dir.y = 0;
                    float force = 0.7f / dir.magnitude;
                    dir.Normalize();
                    flock -= dir * force;
                }
            }
            */


            // Check if the target is in the attack range
            float dd = (target.transform.position - transform.position).sqrMagnitude;
            if (dd <= 1.5 * 1.5)
            {
                // Move a little bit
                if (dd <= 1.2 * 1.2)
                    agent.speed = speed * 0.25f;

                agent.SetDestination(target.transform.position);

                // Rotate actor toward the target
                RotateTowards(direction);
                AnimPlayAttack();

                // Attack but move a little bit to stay in focus
                //moveForce = direction * speed * 0.5f + flock;
                //if (dd <= 1.2 * 1.2)
                //    moveForce = flock;

            }
            else
            {
                // move
                agent.SetDestination(target.transform.position);
                // Rotate actor toward the destination
                RotateTowards(agent.velocity.normalized);
                AnimPlayMoving();

                //moveForce = direction * speed + flock;
            }
        }
        else if (!IsDead())
        {
            agent.isStopped = true;
            AnimPlayIdle();
        }
    }
}
