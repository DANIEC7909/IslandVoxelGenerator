using System.Collections.Generic;
using UnityEngine;

public class IslandTile : MonoBehaviour
{
    int biomeID;
    IslandGenerator Generator;
   
    [SerializeField] List<Vector3> TileNearPositions = new List<Vector3>();


    private void Start()
    {
        //calculate TNPosition

        TileNearPositions.Add(transform.position - Vector3.forward);
        TileNearPositions.Add(transform.position + Vector3.forward);
        TileNearPositions.Add(transform.position + Vector3.right);
        TileNearPositions.Add(transform.position - Vector3.right);

        //check if main freePos list in generator does not have one of this positions. 
        foreach (Vector3 tnpPos in TileNearPositions)
        {
            //add postion to the main freePos list 
            if (!Generator.UsedPositions.Contains(tnpPos) && !Generator.FreePositions.Contains(tnpPos)) Generator.FreePositions.Add(tnpPos);
        }
        //add tile pos to usedPos list 
        Generator.UsedPositions.Add(transform.position);
        //TODO:: Delegate which is going to be called when tile is initialised 
    }
    public IslandGenerator SetGeneratorReference(IslandGenerator generator) => Generator = generator;
    public int GetTileBiome() => biomeID;
}
