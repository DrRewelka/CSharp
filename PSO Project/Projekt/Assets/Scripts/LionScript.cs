using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionScript : ParticleScript
{
    [SerializeField] float distanceToKill = 1.5f;

    public LionScript() : base() { }

	protected override void Awake ()
    {
        base.Awake();
        LocalBestPosition = transform.position.PositionToVector2();
	}

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if(distanceToClosestEnemy < distanceToKill)
        {
            if (closestEnemy.GetComponent<ParticleScript>() != null)
            {
                closestEnemy.GetComponent<ParticleScript>().Die();
                GameManagerScript.Instance.LoseHP();
            }
            else if (closestEnemy.GetComponent<CharacterController>() != null)
            {
                GameManagerScript.Instance.GameOver();
            }
            else if (closestEnemy.tag == "Meat")
            {
                Destroy(closestEnemy.gameObject);
            }
        }
    }
}
