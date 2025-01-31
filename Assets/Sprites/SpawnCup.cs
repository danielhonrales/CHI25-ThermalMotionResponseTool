using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnCup : MonoBehaviour
{

    public GameObject cupPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn() {
        GameObject existingCup = GameObject.Find("Cup");
        if (existingCup != null) {
            Destroy(existingCup);
        }
        GameObject cup = Instantiate(cupPrefab, new Vector3(2.514f, 0.699f, -1.973f), Quaternion.identity);
        cup.name = "Cup";
    }
}
