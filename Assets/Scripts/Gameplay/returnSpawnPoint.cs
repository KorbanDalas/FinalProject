using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform[] spawnPoints;
    
    
    public Transform ReturnPoint()
    {
        if (spawnPoints.Length != 0)
        {
            var rand = Random.Range(0, spawnPoints.Length);
            return spawnPoints[rand];
            
        }
        else
        {
            return null;
        }
    }

}
