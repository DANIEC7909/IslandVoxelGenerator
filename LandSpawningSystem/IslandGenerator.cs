using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> NonAlocatedPositions;
    public List<Vector3> AlocatedPositions;
    public List<Vector3> DestroyedPositions;
    [SerializeField] List<Vector3> Duplicates = new List<Vector3>();
    public Transform PivotTransform;
    public List<IslandTile> IslandTiles = new List<IslandTile>();
    public Dictionary<Vector3, IslandTile> IslandTilesDictionary = new Dictionary<Vector3, IslandTile>();
    [SerializeField] int howMuchSpawn;
    [SerializeField] GameObject tileGameObject;
    [SerializeField] bool CanSpawn = true;
    [SerializeField] bool IsMold = true;
    public bool CanSpawn_g => CanSpawn;
    public bool IsMold_g => IsMold;
    public bool Fixing { get; private set; }
    public bool Done { get; private set; }
    int iterations;
    public enum CalculateMatrixSide { left, right, forward, backward, up, down, DownForwardBackwardLeftRight, all }
    public System.TimeSpan timeTaken { get; private set; }

    private void Start()
    {
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        //create pivot 
        AlocatedPositions.Add(PivotTransform.position);
        Vector3 InitcachedPos = AlocatedPositions[0];

        CalculateNearVectorMatrix(InitcachedPos, CalculateMatrixSide.DownForwardBackwardLeftRight, NonAlocatedPositions);

        while (iterations < howMuchSpawn)
        {
            int posID = UnityEngine.Random.Range(0, NonAlocatedPositions.Count);
            Vector3 cachedFreePos = NonAlocatedPositions[posID];
            if (!AlocatedPositions.Contains(cachedFreePos))
            {
                AlocatedPositions.Add(cachedFreePos);
                NonAlocatedPositions.Remove(cachedFreePos);
                CalculateNearVectorMatrix(cachedFreePos, CalculateMatrixSide.DownForwardBackwardLeftRight, NonAlocatedPositions);
            }
            else
            {
                NonAlocatedPositions.Remove(cachedFreePos);
                iterations--;
            }
            iterations++;
        }
        //cleanup dupes
        foreach (Vector3 dup in AlocatedPositions)
        {
            for (int i = 0; i < NonAlocatedPositions.Count; i++)
            {
                if (dup == NonAlocatedPositions[i])
                {
                    NonAlocatedPositions.Remove(dup);
                }
            }
        }
        //spawn cubes by generated pos Matrix
        foreach (Vector3 pos in AlocatedPositions)
        {
            SpawnTileCalc(pos);
        }
        if (IsMold) MoldIsland();

        timer.Stop();
        timeTaken = timer.Elapsed;
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
        NonAlocatedPositions.Clear();
        AlocatedPositions.Clear();
        Duplicates.Clear();
        Done = false;
        Fixing = false;
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
        Fixing = true;

        Duplicates = NonAlocatedPositions.Distinct().ToList();

        foreach (Vector3 p in Duplicates)
        {
            SpawnTile(p);

        }
        Fixing = false;
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
            if (AlocatedPositions.Contains(PtRpos))
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
        AlocatedPositions.Remove(pos);
        IslandTiles.Remove(itt);
        Destroy(it.gameObject);
        CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight, NonAlocatedPositions, false);
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
            NonAlocatedPositions.Remove(pos);
            AlocatedPositions.Add(it.transform.position);
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
        int id = UnityEngine.Random.Range(0, NonAlocatedPositions.Count);
        while (NonAlocatedPositions[id].y > 0)
        {
            id = UnityEngine.Random.Range(0, NonAlocatedPositions.Count);
        }
        if (!IslandTilesDictionary.ContainsKey(NonAlocatedPositions[id]))
        {
            IslandTile it = Instantiate(tileGameObject, NonAlocatedPositions[id], Quaternion.identity).GetComponent<IslandTile>();
            setMaterial(it, NonAlocatedPositions[id]);
            NonAlocatedPositions.Remove(NonAlocatedPositions[id]);
            AlocatedPositions.Add(it.transform.position);
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

    public List<Vector3> GetAlocatedPositions() => AlocatedPositions;
    public List<Vector3> GetNonAlocatedPositions() => NonAlocatedPositions;
    public bool CanSpawnBlocks(bool can) => CanSpawn = can;
    public bool IsMoldIsland(bool im) => IsMold = im;
    public int SetBlockAmount(int amount) => howMuchSpawn = amount;
    public int GetBlockAmount() => howMuchSpawn;
    public List<IslandTile> GetIslandTiles() => IslandTiles;
}