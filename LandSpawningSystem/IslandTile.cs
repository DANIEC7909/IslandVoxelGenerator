using System.Collections.Generic;
using UnityEngine;
using System.Threading;
public class IslandTile : MonoBehaviour
{
    int biomeID;
    IslandGenerator Generator;
    Vector3 cashedPos;
    public Vector3[] NearPositionMatrix = new Vector3[6];
    public MeshRenderer mr;
    [SerializeField] Material[] IslandMaterials;
    public enum BlockType { Grass, Dirt, Rock };
    BlockType blockType;
    private void Start()
    {
        cashedPos = transform.position;
        mr = GetComponent<MeshRenderer>();

    }

    public void SetBlockType(BlockType bt)
    {
        blockType = bt;
        mr.material = IslandMaterials[(int)bt];
    }
    public BlockType GetBlockType() => blockType;
    public void SetPositionMatrix(Vector3[] npm)=> NearPositionMatrix = npm;
}
