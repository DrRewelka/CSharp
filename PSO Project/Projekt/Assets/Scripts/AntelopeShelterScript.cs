using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AntelopeShelterScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var antelope = other.GetComponent<AntelopeScript>();

        if (antelope != null)
        {
            SceneManager.LoadScene("gameVictory");
        }
    }
}
