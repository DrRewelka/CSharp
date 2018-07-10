using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class SwarmScript : MonoBehaviour
{
    public Transform mapBoundXPlus;
    public Transform mapBoundXMinus;
    public Transform mapBoundZPlus;
    public Transform mapBoundZMinus;

    [SerializeField] AntelopeShelterScript shelter;

    [SerializeField] ParticleScript swarmUnitObj;
    [SerializeField] int maxQuantity;
    [SerializeField] Transform target;

    public List<ParticleScript> swarm;

    public Terrain terrain;
    public FunctionType functionType;

    public string enemyTag;
    public Transform safetyTarget;
    public float visionRadius;

    private void Start()
    {
        SpawnSwarm(out swarm, swarmUnitObj, maxQuantity);
    }

    void SpawnSwarm(out List<ParticleScript> _swarm, ParticleScript _swarmUnityObject, int _amount)
    {
        _swarm = new List<ParticleScript>();
        for (int i = 0; i < _amount; i++)
        {
            // for each particle in swarm it is trying to spawn it somewhere in specified area.
            // the number of tries is hardcoded to 50 and it could be changed later.
            var tries = 50;
            while (tries-- > 0)
            {
                // randomize spawn position for current particle
                var pos = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0f, transform.localScale.x);

                // if there is any particle in the distance of one meter(hardcoded), loop is continued to try again.
                if (swarm.Any(p => Vector3.Distance(p.transform.position, pos) < 1f))
                    continue;

                // spawn particle in randomized position and break the while loop
                var unit = Instantiate(_swarmUnityObject, pos, Quaternion.identity);
                unit.SetupParticle(this);
                _swarm.Add(unit);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Show spawn area in editor.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
    }
}

public enum FunctionType
{
    Cost,
    Fitness,
    Mixed
}