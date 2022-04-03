using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class IslandTile : MonoBehaviour
{
    int biomeID;
    IslandGenerator Generator;
    Vector3 cashedPos;
    [SerializeField] List<Vector3> TileNearPositions = new List<Vector3>();
    public MeshRenderer mr;
    [SerializeField] Material[] IslandMaterials;
    public enum BlockType { Grass, Dirt, Rock };
    BlockType blockType;
    private void Start()
    {
        cashedPos = transform.position;
        mr = GetComponent<MeshRenderer>();
        //calculate TNPosition
        /*  Thread Calclations = new Thread(CalculateAndInit);
          Calclations.Start();*/
        CalculateAndInit();
    }
  
    private void CalculateAndInit()
    {

        TileNearPositions.Add(cashedPos - Vector3.forward);
        TileNearPositions.Add(cashedPos + Vector3.forward);
        TileNearPositions.Add(cashedPos + Vector3.right);
        TileNearPositions.Add(cashedPos - Vector3.right);
        TileNearPositions.Add(cashedPos - Vector3.up);
       // TileNearPositions.Add(cashedPos + Vector3.up);
        //check if main freePos list in generator does not have one of this positions. 
        foreach (Vector3 tnpPos in TileNearPositions)
        {
            //add postion to the main freePos list 
            if (!Generator.UsedPositions.Contains(tnpPos) && !Generator.FreePositions.Contains(tnpPos)) Generator.FreePositions.Add(tnpPos);
        }
        //add tile pos to usedPos list 
        //done in IslandGenerator.cs

        //TODO:: Delegate which is going to be called when tile is initialised// Done
        Generator.CanSpawn = true;
    } 
    public IslandGenerator SetGeneratorReference(IslandGenerator generator) => Generator = generator;
    public void SetBlockType(BlockType bt)
    {
        blockType = bt;   
        mr.material = IslandMaterials[(int)bt];
    }
    public BlockType GetBlockType() => blockType;
   
}
