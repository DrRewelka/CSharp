using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntelopeScript : ParticleScript
{
    protected override void Awake()
    {
        base.Awake();
        LocalBestPosition = transform.position.PositionToVector2();
    }

    public override void Move()
    {
        base.Move();
    }
}
