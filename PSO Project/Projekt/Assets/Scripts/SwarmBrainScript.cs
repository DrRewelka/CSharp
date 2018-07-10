using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class SwarmScript : MonoBehaviour
{
    public float GlobalBestCost { get; private set; } = Mathf.Infinity;
    public Vector3 GlobalBestPosition { get; private set; } = Vector3.zero;
    public float GlobalBestFitness { get; private set; } = Mathf.Infinity;
    public Vector3 GlobalWorstPosition { get; private set; } = Vector3.zero;

    private void Update()
    {
        DoStuff();
    }

    public void Remove(ParticleScript particle)
    {
        swarm.Remove(particle);
    }

    public Transform GetBest()
    {
        return swarm.First(p => p.LocalCost == swarm.Min(d => d.LocalCost) && p.isPendingDeath == false).transform;
    }

    public void DoStuff()
    {
        // search for particle with best local cost and set it's parameters as global bests.
        // First() returns first element on the list that matches the condition.
        // Min() returns lowest value from specified parameter, in this case the LocalCost
        if(swarm.Any(p => p.LocalCost == swarm.Min(d => d.LocalCost) && p.isPendingDeath == false))
        {
            var bestParticle = swarm.First(p => p.LocalCost == swarm.Min(d => d.LocalCost) && p.isPendingDeath == false);

            GlobalBestCost = bestParticle.LocalCost;
            GlobalBestPosition = bestParticle.LocalBestPosition;

            for (int i = swarm.Count - 1; i >= 0; i--)
            {
                swarm[i].Move();
            }
        }
    }
}
