using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmCanvasScript : MonoBehaviour
{
    GameObject player;
	
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	// Update is called once per frame
	void Update ()
    {
        float objectiveScale = Vector3.Distance(transform.position, player.transform.position) / 40;

        transform.LookAt(player.transform);
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
        transform.Rotate(new Vector3(0, 180, 0));
        transform.localScale = new Vector3(objectiveScale, objectiveScale, objectiveScale);
	}
}
