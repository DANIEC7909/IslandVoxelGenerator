
using System.Collections.Generic;

using System.Linq;
using TMPro;
using UnityEngine;
public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> FreePositions;
    public List<Vector3> UsedPositions;
    public List<Vector3> DestroyedPositions;

    public List<IslandTile> IslandTiles = new List<IslandTile>();
    [SerializeField] int howMuchSpawn;
    [SerializeField] GameObject tileGameObject;
    public bool CanSpawn = true;
    int iterations;
    [SerializeField] TextMeshProUGUI counter;
    string testcontent;
    public enum CalculateMatrixSide { left, right, forward, backward, up, down, dfblr, all }


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

        CalculateNearVectorMatrix(InitcachedPos, CalculateMatrixSide.dfblr, FreePositions);

        while (iterations < howMuchSpawn)
        {
            int posID = UnityEngine.Random.Range(0, FreePositions.Count);
            Vector3 cachedFreePos = FreePositions[posID];
            UsedPositions.Add(cachedFreePos);
            FreePositions.Remove(cachedFreePos);
            //calculate near pos's
            CalculateNearVectorMatrix(cachedFreePos, CalculateMatrixSide.dfblr, FreePositions);
            iterations++;
        }

        foreach (Vector3 pos in UsedPositions)
        {
            SpawnTileCalc(pos);
        }
        MoldIsland();

        timer.Stop();
        System.TimeSpan timeTaken = timer.Elapsed;
        UnityEngine.Debug.Log("Island Generation taken: " + timeTaken.TotalSeconds + "s");
        testcontent = "Island Generation taken: " + timeTaken.TotalSeconds + "s" + " " + "Initial amount of the voxel " + howMuchSpawn.ToString() + " " + "Amount After Molding " + IslandTiles.Count.ToString();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTile();
            MoldIsland();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            FixIsland();
        }

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
            foreach (IslandTile it in IslandTiles)
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
        counter.text = testcontent + " " + " How much voxels to spawn " + howMuchSpawn.ToString();
    }
    void setMaterial(IslandTile it, Vector3 pos)
    {
        if (pos.y <= 0 && pos.y > -2)
        {
            it.SetBlockType(IslandTile.BlockType.Grass);
        }
        else if (pos.y <= -2 && pos.y > -5)
        {
            it.SetBlockType(IslandTile.BlockType.Dirt);
        }
        else if (pos.y < -5)
        {
            it.SetBlockType(IslandTile.BlockType.Rock);
        }
    }
    public Vector3[] CalculateNearVectorMatrix(Vector3 pos, CalculateMatrixSide cms)
    {
        Vector3[] nPos = null;
        switch (cms)
        {
            case CalculateMatrixSide.backward:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.forward;
                break;
            case CalculateMatrixSide.forward:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.forward;
                break;
            case CalculateMatrixSide.left:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.right;
                break;
            case CalculateMatrixSide.right:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.right;
                break;
            case CalculateMatrixSide.up:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.up;
                break;
            case CalculateMatrixSide.down:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.up;
                break;
            case CalculateMatrixSide.dfblr:
                nPos = new Vector3[5];
                nPos[0] = pos - Vector3.forward;
                nPos[1] = pos + Vector3.forward;
                nPos[2] = pos + Vector3.right;
                nPos[3] = pos - Vector3.right;
                nPos[4] = pos - Vector3.up;
                break;
            case CalculateMatrixSide.all:
                nPos = new Vector3[6];
                nPos[0] = pos - Vector3.forward;
                nPos[1] = pos + Vector3.forward;
                nPos[2] = pos + Vector3.right;
                nPos[3] = pos - Vector3.right;
                nPos[4] = pos - Vector3.up;
                nPos[5] = pos + Vector3.up;
                break;
        }

        return nPos;
    }
    public Vector3[] CalculateNearVectorMatrix(Vector3 pos, CalculateMatrixSide cms, List<Vector3> listToAdd, bool AddRemove = true)
    {
        Vector3[] nPos = null;
        switch (cms)
        {
            case CalculateMatrixSide.backward:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.forward;
                break;
            case CalculateMatrixSide.forward:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.forward;
                break;
            case CalculateMatrixSide.left:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.right;
                break;
            case CalculateMatrixSide.right:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.right;
                break;
            case CalculateMatrixSide.up:
                nPos = new Vector3[1];
                nPos[0] = pos + Vector3.up;
                break;
            case CalculateMatrixSide.down:
                nPos = new Vector3[1];
                nPos[0] = pos - Vector3.up;
                break;
            case CalculateMatrixSide.dfblr:
                nPos = new Vector3[5];
                nPos[0] = pos - Vector3.forward;
                nPos[1] = pos + Vector3.forward;
                nPos[2] = pos + Vector3.right;
                nPos[3] = pos - Vector3.right;
                nPos[4] = pos - Vector3.up;
                break;
            case CalculateMatrixSide.all:
                nPos = new Vector3[6];
                nPos[0] = pos - Vector3.forward;
                nPos[1] = pos + Vector3.forward;
                nPos[2] = pos + Vector3.right;
                nPos[3] = pos - Vector3.right;
                nPos[4] = pos - Vector3.up;
                nPos[5] = pos + Vector3.up;
                break;
        }
        if (AddRemove)
        {
            foreach (Vector3 p in nPos)
            {
                listToAdd.Add(p);
            }
        }
        else
        {
            foreach (Vector3 p in nPos)
            {
                listToAdd.Remove(p);
            }
        }
        return nPos;
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
    public void FixIsland()
    {
        foreach (Vector3 pos in DestroyedPositions)
        {
            SpawnTileCalc(pos);
            UsedPositions.Add(pos);
        }
        DestroyedPositions.Clear();
    }
    public IslandTile DestroyTile(IslandTile itt)
    {
        IslandTile it = itt;
        Vector3 pos = it.transform.position;
        DestroyedPositions.Add(pos);
        IslandTiles.Remove(itt);
        Destroy(it.gameObject);
        CalculateNearVectorMatrix(pos, CalculateMatrixSide.dfblr, FreePositions, false);
        return it;
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
        setMaterial(it, pos);
        return it;
    }
    public IslandTile SpawnTile()
    {
        int id = UnityEngine.Random.Range(0, FreePositions.Count);
        while (FreePositions[id].y > 0)
        {
            id = UnityEngine.Random.Range(0, FreePositions.Count);
        }
        IslandTile it = Instantiate(tileGameObject, FreePositions[id], Quaternion.identity).GetComponent<IslandTile>();
        setMaterial(it, FreePositions[id]);
        FreePositions.Remove(FreePositions[id]);
        UsedPositions.Add(it.transform.position);
        it.SetGeneratorReference(this);
        IslandTiles.Add(it);
        return it;
    }
    public List<IslandTile> GetIslandTiles() => IslandTiles;

}


