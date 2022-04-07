using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> FreePositions;
    public List<Vector3> UsedPositions;
    public List<Vector2> DestroyedPositions;
    public List<IslandTile> IslandTiles = new List<IslandTile>();
    [SerializeField] int howMuchSpawn;
    [SerializeField] GameObject tileGameObject;
    public bool CanSpawn = true;
    int iterations;
    [SerializeField] TextMeshProUGUI counter;
    [SerializeField] TextMeshProUGUI counterHow;

    [SerializeField] List<Vector3> Duplicates = new List<Vector3>();


    [SerializeField] bool fixing;
    public bool Done;
    private void Start()
    {
        FreePositions.Add(Vector3.zero);
    }
    private void Update()
    {
  
    }
    public void FixIsland()
    {
        fixing = true;
        /*    foreach (Vector3 p in FreePositions)
            {
                if (p.y == 0)
                {
                    Duplicates.Add(p);
                    //  SpawnTileByPos(p);
                }
            }
            for(int i =0;i<Duplicates.Count;i++)
            {
                SpawnTileByPos(Duplicates[i]);
            }*/
        Duplicates = FreePositions.Distinct().ToList();
        foreach (Vector3 p in Duplicates)
        {
            SpawnTile(p);
        }
        Done = true;
    }
    void setMaterial(IslandTile it, Vector3 pos)
    {
        if (pos.y <= 0&&pos.y>-2)
        {
            it.SetBlockType(IslandTile.BlockType.Grass);
        }
        else if (pos.y <= -2&&pos.y>-5)
        {
            it.SetBlockType(IslandTile.BlockType.Dirt);
        }
        else if (pos.y < -5)
        {
            it.SetBlockType(IslandTile.BlockType.Rock);
        }
    }
    public IslandTile SpawnTile(Vector3 pos)
    {
        IslandTile it = Instantiate(tileGameObject, pos, Quaternion.identity).GetComponent<IslandTile>();
        FreePositions.Remove(pos);
        UsedPositions.Add(it.transform.position);
        it.SetGeneratorReference(this);
        IslandTiles.Add(it);
        setMaterial(it,pos);
        return it;
    }

    public IslandTile SpawnTile()
    {
        int id = Random.Range(0, FreePositions.Count);
        while (FreePositions[id].y > 0)
        {
            id = Random.Range(0, FreePositions.Count);
        }
        IslandTile it = Instantiate(tileGameObject, FreePositions[id], Quaternion.identity).GetComponent<IslandTile>();
        setMaterial(it, FreePositions[id]);
        FreePositions.Remove(FreePositions[id]);
        UsedPositions.Add(it.transform.position);
        it.SetGeneratorReference(this);
        IslandTiles.Add(it);
        return it;
    }

}
