using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HouseType  
{
    [SerializeField]
    private GameObject[] prefabs;
    public int sizeRequired;
    public int quant;
    public int quantityAlreadyPlaced;

    public GameObject getPrefab()
    {
        quantityAlreadyPlaced++;
        if (prefabs.Length > 1)
        {
            var random = UnityEngine.Random.Range(0, prefabs.Length);
            return prefabs[random];
        }
        return prefabs[0];
    }

    public bool isBuildAvail()
    {
        return quantityAlreadyPlaced < quant;
    }

    public void Reset()
    {
        quantityAlreadyPlaced = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
