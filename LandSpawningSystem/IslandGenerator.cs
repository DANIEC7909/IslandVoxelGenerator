
using System.Collections.Generic;

using System.Linq;
using TMPro;
using UnityEngine;
public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> FreePositions;
    public List<Vector3> UsedPositions;
    public List<Vector3> DestroyedPositions;
    public Transform PivotTransform;
    public List<IslandTile> IslandTiles = new List<IslandTile>();
    public Dictionary<Vector3, IslandTile> IslandTilesDictionary = new Dictionary<Vector3, IslandTile>();
    [SerializeField] int howMuchSpawn;
    [SerializeField] GameObject tileGameObject;
    public bool CanSpawn = true;
    public bool IsMold = true;
    int iterations;
    [SerializeField] TextMeshProUGUI counter;
    string testcontent;
    public enum CalculateMatrixSide { left, right, forward, backward, up, down, DownForwardBackwardLeftRight, all }


    [SerializeField] List<Vector3> Duplicates = new List<Vector3>();


    [SerializeField] bool fixing;
    public bool Done;
    private void Start()
    {
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        //create pivot 
        UsedPositions.Add(PivotTransform.position);
        Vector3 InitcachedPos = UsedPositions[0];

        CalculateNearVectorMatrix(InitcachedPos, CalculateMatrixSide.DownForwardBackwardLeftRight, FreePositions);

        while (iterations < howMuchSpawn)
        {
            int posID = UnityEngine.Random.Range(0, FreePositions.Count);
            Vector3 cachedFreePos = FreePositions[posID];
            if (!UsedPositions.Contains(cachedFreePos))
            {
                UsedPositions.Add(cachedFreePos);
                FreePositions.Remove(cachedFreePos);
                CalculateNearVectorMatrix(cachedFreePos, CalculateMatrixSide.DownForwardBackwardLeftRight, FreePositions);
            }
            else
            {
                FreePositions.Remove(cachedFreePos);
                iterations--;
            }
            //calculate near pos's
            iterations++;
        }
        foreach (Vector3 dup in UsedPositions)
        {
            for (int i = 0; i < FreePositions.Count; i++)
            {
                if (dup == FreePositions[i])
                {
                    FreePositions.Remove(dup);
                }
            }
        }

        foreach (Vector3 pos in UsedPositions)
        {
            SpawnTileCalc(pos);
        }
        if (IsMold) MoldIsland();

        timer.Stop();
        System.TimeSpan timeTaken = timer.Elapsed;
        UnityEngine.Debug.Log("Island Generation taken: " + timeTaken.TotalSeconds + "s");
        testcontent = "Island Generation taken: " + timeTaken.TotalSeconds + "s" + " " + "Initial amount of the voxel " + howMuchSpawn.ToString() + " " + "Amount After Molding " + IslandTiles.Count.ToString();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnTileRandom();
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (IslandTile it in IslandTiles)
            {
                it.GenerateCube();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RegenerateIsland();
        }
        counter.text = testcontent + " " + " How much voxels to spawn " + howMuchSpawn.ToString();
    }
    /// <summary>
    /// Regenerates Island
    /// </summary>
    public void RegenerateIsland()
    {
        Clear();
        Start();
    }
    void Clear()
    {
        foreach (IslandTile it in IslandTiles)
        {
            Destroy(it.gameObject);
        }
        IslandTiles.Clear();
        IslandTilesDictionary.Clear();
        FreePositions.Clear();
        UsedPositions.Clear();
        Duplicates.Clear();
        Done = false;
        fixing = false;
        iterations = 0;
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
        else if (pos.y <= -5)
        {
            it.SetBlockType(IslandTile.BlockType.Rock);
        }
    }
    /// <summary>
    /// Calculates adjacent blocks positions.
    /// Returned Vector[] description
    /// 0-backward
    /// 1-forward
    /// 2-right
    /// 3-left
    /// 4-up
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cms"></param>
    /// <returns></returns>
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
            case CalculateMatrixSide.DownForwardBackwardLeftRight:
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
    /// <summary>
    /// Calculates adjacent blocks positions and Removes or Adds it to desired lsits
    /// Returned Vector[] description
    /// 0-backward
    /// 1-forward
    /// 2-right
    /// 3-left
    /// 4-up
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cms"></param>
    /// <returns></returns>
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
            case CalculateMatrixSide.DownForwardBackwardLeftRight:
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
    /// <summary>
    /// This function makes islands looks pretier(Fills empty cubes).
    /// </summary>
    public void MoldIsland()
    {
        fixing = true;

        Duplicates = FreePositions.Distinct().ToList();

        foreach (Vector3 p in Duplicates)
        {
            SpawnTile(p);

        }
        fixing = false;
        Done = true;
    }
    /// <summary>
    /// This function fixes island based on DestroyedPostions Matrix.
    /// </summary>
    public void FixIsland()
    {
        List<IslandTile> ToRegenerate = new List<IslandTile>();
        foreach (Vector3 pos in DestroyedPositions)
        {
            SpawnTile(pos);

            RecalculateFaces(pos);
        }

        DestroyedPositions.Clear();

    }
    void RecalculateFaces(Vector3 pos)
    {

        Vector3[] PositionsToRegenerate = CalculateNearVectorMatrix(pos, CalculateMatrixSide.all);

        List<Vector3> ToRegenerateMatrix = new List<Vector3>();

        foreach (Vector3 PtRpos in PositionsToRegenerate)
        {
            if (UsedPositions.Contains(PtRpos))
            {
                ToRegenerateMatrix.Add(PtRpos);
            }
        }
        foreach (Vector3 trm in ToRegenerateMatrix)
        {
            IslandTile itg;
            bool hasValue = IslandTilesDictionary.TryGetValue(trm, out itg); // IslandTilesDictionary[trm];
            if (hasValue) { itg.GenerateCube(); }
        }

    }
    /// <summary>
    /// This function destroys tiles and recalculaters faces 
    /// </summary>
    /// <param name="itt"></param>
    public void DestroyTileRecalculateFaces(IslandTile itt)
    {
        DestroyTile(itt, false);
        Vector3 pos = itt.transform.position;
        RecalculateFaces(pos);
        IslandTilesDictionary.Remove(pos);
    }
    /// <summary>
    /// This function only destroys tile and do nothing to face recalculation 
    /// </summary>
    /// <param name="itt"></param>
    public void DestroyTile(IslandTile itt, bool RemoveInDictionary = true)
    {
        IslandTile it = itt;
        Vector3 pos = it.transform.position;
        DestroyedPositions.Add(pos);
        UsedPositions.Remove(pos);
        IslandTiles.Remove(itt);
        Destroy(it.gameObject);
        CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight, FreePositions, false);
        if (RemoveInDictionary)
        {
            IslandTilesDictionary.Remove(pos);
        }
    }
    /// <summary>
    /// This functions spawn block in current position by UsedPostions var on the START
    /// IMPORTANT: This function didn't adds object to usedPosMatrix and didn't removes block from FreePositonsMatrix
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private IslandTile SpawnTileCalc(Vector3 pos)
    {
        if (!IslandTilesDictionary.ContainsKey(pos))
        {
            IslandTile it = Instantiate(tileGameObject, pos, Quaternion.identity).GetComponent<IslandTile>();
            //  it.gameObject.transform.SetParent(gameObject.transform);
            IslandTiles.Add(it);
            IslandTilesDictionary.Add(pos, it);

            setMaterial(it, pos);
            it.SetPositionMatrix(CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight));
            it.SetGeneratorInstance(this);
            return it;
        }
        else
        {
            Debug.LogWarning("Cannot spawn block in existing  position");
            return null;
        }
    }
    /// <summary>
    /// This function spawn block in desired position by pos position 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public IslandTile SpawnTile(Vector3 pos)
    {
        if (!IslandTilesDictionary.ContainsKey(pos))
        {
            IslandTile it = Instantiate(tileGameObject, pos, Quaternion.identity).GetComponent<IslandTile>();
            FreePositions.Remove(pos);
            UsedPositions.Add(it.transform.position);
            IslandTiles.Add(it);
            IslandTilesDictionary.Add(pos, it);

            setMaterial(it, pos);
            it.SetPositionMatrix(CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight));
            it.SetGeneratorInstance(this);
            return it;
        }
        else
        {
            Debug.LogWarning("Cannot spawn block in existing  position");
            return null;
        }
    }
    public IslandTile SpawnTileRandom()
    {
        int id = UnityEngine.Random.Range(0, FreePositions.Count);
        while (FreePositions[id].y > 0)
        {
            id = UnityEngine.Random.Range(0, FreePositions.Count);
        }
        if (!IslandTilesDictionary.ContainsKey(FreePositions[id]))
        {
            IslandTile it = Instantiate(tileGameObject, FreePositions[id], Quaternion.identity).GetComponent<IslandTile>();
            setMaterial(it, FreePositions[id]);
            FreePositions.Remove(FreePositions[id]);
            UsedPositions.Add(it.transform.position);
            it.SetGeneratorInstance(this);
            IslandTiles.Add(it);
            Vector3 pos = it.transform.position;
            IslandTilesDictionary.Add(pos, it);
            it.SetPositionMatrix(CalculateNearVectorMatrix(it.transform.position, CalculateMatrixSide.DownForwardBackwardLeftRight));
            return it;
        }
        else
        {
            Debug.LogWarning("Cannot spawn block in existing  position");
            return null;
        }
    }
    public List<IslandTile> GetIslandTiles() => IslandTiles;
}