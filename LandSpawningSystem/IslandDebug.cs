using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandDebug : MonoBehaviour
{
    [SerializeField] GameObject DebugCube;
    [SerializeField] IslandGenerator ig;
    [SerializeField] int ElementIndex;
    [SerializeField] KeyCode Pkey;
    [SerializeField] KeyCode Nkey;
    [SerializeField] KeyCode Tkey;
    void Update()
    {
        if (Input.GetKeyDown(Pkey))
        {
            ElementIndex++;
        } 
        if (Input.GetKeyDown(Nkey))
        {
            ElementIndex--;
        }
        DebugCube.transform.position = ig.NonAlocatedPositions[ElementIndex];
        if (Input.GetKeyDown(Tkey))
        {
            StartCoroutine(turn());
        }
        if(ElementIndex+1> ig.NonAlocatedPositions.Count)
        {
            ElementIndex = 0;
        }
    }
    IEnumerator turn()
    {
        for (int i = 0; i < ig.NonAlocatedPositions.Count-1; i++)
        {
            ElementIndex++;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
