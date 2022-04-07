
using System.Collections.Generic;

using System.Linq;
using System.Threading;
using TMPro;
using Unity.Jobs;
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
    string testcontent;
    

    [SerializeField] List<Vector3> Duplicates = new List<Vector3>();


    [SerializeField] bool fixing;
    public bool Done;
    private void Start()
    {
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        //create pivot 
        UsedPositions.Add(Vector3.zero);
        Vector3 InitcachedPos = UsedPositions[0];
        FreePositions.Add(InitcachedPos - Vector3.forward);
        FreePositions.Add(InitcachedPos + Vector3.forward);
        FreePositions.Add(InitcachedPos + Vector3.right);
        FreePositions.Add(InitcachedPos - Vector3.right);
        FreePositions.Add(InitcachedPos - Vector3.up);


        while (iterations < howMuchSpawn)
        {
            int posID = Random.Range(0, FreePositions.Count);
            Vector3 cachedFreePos = FreePositions[posID];
            UsedPositions.Add(cachedFreePos);
            FreePositions.Remove(cachedFreePos);
            //calculate near pos's
            FreePositions.Add(cachedFreePos - Vector3.forward);
            FreePositions.Add(cachedFreePos + Vector3.forward);
            FreePositions.Add(cachedFreePos + Vector3.right);
            FreePositions.Add(cachedFreePos - Vector3.right);
            FreePositions.Add(cachedFreePos - Vector3.up);
            iterations++;
        }
        foreach (Vector3 pos in UsedPositions)
        {
            SpawnTileCalc(pos);
        }
            MoldIsland();
        timer.Stop();
        System.TimeSpan timeTaken = timer.Elapsed;
        UnityEngine.Debug.Log("Island Generation taken: " + timeTaken.TotalSeconds+"s");
       testcontent = "Island Generation taken: " + timeTaken.TotalSeconds + "s"+" "+ "Initial amount of the voxel "+howMuchSpawn.ToString()+" "+"Amount After Molding "+IslandTiles.Count.ToString();
    }

   


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            howMuchSpawn += 500;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            howMuchSpawn -= 500;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach(IslandTile it in IslandTiles)
            {
                Destroy(it.gameObject);
            }
            IslandTiles.Clear();
            FreePositions.Clear();
            UsedPositions.Clear();
            Duplicates.Clear();
            Done = false;
            fixing = false;
            iterations = 0;
            Start();
        }
        counter.text = testcontent + " " +" How much voxels to spawn"+howMuchSpawn.ToString();
    }
    public void MoldIsland()
    {
        fixing = true;
    
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
    public IslandTile SpawnTileCalc(Vector3 pos)
    {
        IslandTile it = Instantiate(tileGameObject, pos, Quaternion.identity).GetComponent<IslandTile>();
      
        it.SetGeneratorReference(this);
        IslandTiles.Add(it);
        setMaterial(it, pos);
        return it;
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
