using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ParticleScript : MonoBehaviour
{
    protected SwarmScript swarm;

    [SerializeField] protected Transform target;
    protected Rigidbody rigid;

    public float LocalCost => GetLocalCost();
    public float LocalBestCost { get; protected set; } = Mathf.Infinity;

    public Vector2 LocalBestPosition { get; protected set; }

    //Export to editor
    protected float intertiaWeight = 1f;
    protected float personalCoefficient = 3f;
    protected float socialCoefficient = 1f;

    protected Vector2 velocity2d;
    protected Vector2 targetPos;
    protected Vector2 targetLastPos;

    float checkTargetPosTimeMax = 0.8f;
    float checkTargetPosTimer;

    protected Vector3 velocity;

    protected List<Vector3> positionHistory = new List<Vector3>();
    protected int maxPosHistory = 30;

    protected GameObject player;

    [SerializeField] protected float particleSpeed;

    protected Transform closestEnemy = null;
    protected float distanceToClosestEnemy = Mathf.Infinity;

    public bool isPendingDeath = false;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        velocity2d = Vector2.zero;
        positionHistory.Add(transform.position);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void SetupParticle(SwarmScript _swarm)
    {
        swarm = _swarm;
        target = SwitchTarget();
        targetLastPos = target.position.PositionToVector2();
    }

    public float GetLocalCost()
    {
        float distanceToTarget = Vector2.Distance(targetPos, transform.position.PositionToVector2());

        if (swarm.functionType == FunctionType.Cost || (swarm.functionType == FunctionType.Mixed && target == swarm.safetyTarget))
            return distanceToTarget;
        else return -distanceToTarget;

    }

    public virtual void Move()
    {
        if (target == null)
            target = SwitchTarget();
        if (targetLastPos != target.position.PositionToVector2())
        {
            // To change later probably.
            LocalBestCost = Mathf.Infinity;
        }

        // set personal and social vectors of movement
        var rand1 = (new Vector2(Random.Range(0.9f, 1f), Random.Range(0.9f, 1f)).normalized);
        var xPersonal = personalCoefficient * rand1.x * (LocalBestPosition.x - transform.position.PositionToVector2().x);
        var yPersonal = personalCoefficient * rand1.y * (LocalBestPosition.y - transform.position.PositionToVector2().y);

        var rand2 = (new Vector2(Random.Range(0.9f, 1f), Random.Range(0.9f, 1f)).normalized);
        var xSocial = socialCoefficient * rand2.x * (swarm.GlobalBestPosition.x - transform.position.PositionToVector2().x);
        var ySocial = socialCoefficient * rand2.y * (swarm.GlobalBestPosition.y - transform.position.PositionToVector2().y);

        // set velocity of particle
        velocity2d = (intertiaWeight * velocity2d + new Vector2(xPersonal, yPersonal) + new Vector2(xSocial, ySocial)).normalized * particleSpeed;
        velocity = new Vector3(velocity2d.x, 0, velocity2d.y);

        // Update local bests if current are better.
        
        if (LocalCost < LocalBestCost)
        {
            LocalBestCost = LocalCost;
            LocalBestPosition = transform.position.PositionToVector2();
        }
        targetLastPos = target.position.PositionToVector2();
        

        SurroundingsChecker();
    }

    public SwarmScript GetSwarm()
    {
        return swarm;
    }

    public virtual Transform SwitchTarget() //Switching target for the closest object.
    {
        var enemies = new List<GameObject>();

        enemies.AddRange(GameObject.FindGameObjectsWithTag(swarm.enemyTag));
        enemies.RemoveAll(e => e.GetComponent<ParticleScript>()?.isPendingDeath == true);
        
        distanceToClosestEnemy = Mathf.Infinity;

        var meats = GameObject.FindGameObjectsWithTag("Meat");

        if (swarm.functionType == FunctionType.Cost)
        {
            if(meats.Length > 0 && Vector3.Distance(transform.position, closestEnemy.position) < swarm.visionRadius)
            {
                if (target != meats[0])
                    LocalBestCost = Mathf.Infinity;
                closestEnemy = meats[0].transform;
                distanceToClosestEnemy = Vector3.Distance(transform.position, closestEnemy.position);
                return meats[0].transform;
            }

            closestEnemy = player.transform;
            distanceToClosestEnemy = Vector3.Distance(transform.position, player.transform.position);
        }

        foreach (GameObject enemy in enemies)
        {
            var distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToTarget < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToTarget;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null && Vector3.Distance(transform.position, closestEnemy.position) < swarm.visionRadius)
        {
            if(closestEnemy.GetComponent<CharacterController>() == null)
            {
                var head = enemies.First(e => e.GetComponent<ParticleScript>()?.isPendingDeath == false)
                        .GetComponent<ParticleScript>()
                        .GetSwarm()
                        .GetBest();

                if (target != head)
                    LocalBestCost = Mathf.Infinity;
                if (closestEnemy != target)
                    checkTargetPosTimer = 0;
                return head;
            }
            else
            {
                if (target != closestEnemy)
                {
                    LocalBestCost = Mathf.Infinity;
                    checkTargetPosTimer = 0;
                }
                return closestEnemy;
            }
        }
        else
        {
            if (target != swarm.safetyTarget)
            {
                LocalBestCost = Mathf.Infinity;
                checkTargetPosTimer = 0;
            }

            return swarm.safetyTarget;
        }
    }

    private void SurroundingsChecker()
    {
        float maxDistance = 5.0f;
        float distanceBetweenAllies;

        foreach(var ally in swarm.swarm)
        {
            distanceBetweenAllies = Vector3.Distance(transform.position, ally.transform.position);

            //Keeps antelopes in a distance between each other
            if (distanceBetweenAllies < maxDistance)
            {
                transform.position += (transform.position - ally.transform.position).normalized * (maxDistance - distanceBetweenAllies);
            }
        }
    }

    public void Die()
    {
        isPendingDeath = true;
        swarm.Remove(this);
        Destroy(gameObject, 0.1f);
    }

    public virtual void Update()
    {
        //To switch targets dynamically
        target = SwitchTarget();

        checkTargetPosTimer -= Time.deltaTime;
        if(checkTargetPosTimer <= 0)
        {
            checkTargetPosTimer = Mathf.Lerp(0, checkTargetPosTimeMax, Mathf.Clamp01(((Vector3.Distance(transform.position, target.position) - 5) / swarm.visionRadius)));
            targetPos = target.position.PositionToVector2();
        }
    }

    protected virtual void LateUpdate()
    {
        var smoothPos = Vector3.Lerp(transform.position, 2 * transform.position - positionHistory[positionHistory.Count - 1], 0.14f);
        transform.LookAt(smoothPos);
    }

    public virtual void FixedUpdate()
    {
        if (target != null)
            transform.position += velocity * Time.fixedDeltaTime;

        if (transform.position.x < swarm.mapBoundXMinus.position.x)
            transform.position = new Vector3(swarm.mapBoundXMinus.position.x, transform.position.y, transform.position.z);
        else if (transform.position.x > swarm.mapBoundXPlus.position.x)
            transform.position = new Vector3(swarm.mapBoundXPlus.position.x, transform.position.y, transform.position.z);
        else if (transform.position.z < swarm.mapBoundZMinus.position.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, swarm.mapBoundZMinus.position.z);
        else if (transform.position.z > swarm.mapBoundZPlus.position.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, swarm.mapBoundZPlus.position.z);

        float yPos = swarm.terrain.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);

        positionHistory.Insert(0, transform.position);

        if (positionHistory.Count > maxPosHistory)
            positionHistory.RemoveAt(maxPosHistory);
    }
}
