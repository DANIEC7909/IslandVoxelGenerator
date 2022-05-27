using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> NonAlocatedPositions;
    /// <summary>
    /// Matrix/Scheme of positions to spawn blocks
    /// </summary>
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
    //Testing
    [SerializeField] Material Grass, Dirt, Rock;
    [SerializeField] GameObject GrassCombine, DirtCombine, RockCombine;
    [SerializeField] MeshFilter m_GrassCombine, m_DirtCombine, m_RockCombine;
    [SerializeField] bool isCombineMeshes = false;
    Thread CleanDupes_T;
    Thread CalculateNearVectorAfterSpawn;
    private void cleanDupes()
    {
        foreach (Vector3 dup in AlocatedPositions)
        {
            for (int i = 0; i < NonAlocatedPositions.Count; i++)
            {
                if (dup == NonAlocatedPositions[i])
                {
                    NonAlocatedPositions.Remove(dup);
                    Debug.LogWarning("Removing Dupes" + dup);
                    
                }
            }
        }
      
    }

    void calculateAllPositionsAfterSpawn()
    {
        foreach(Vector3 pos in AlocatedPositions)
        {
            NonAlocatedPositions.AddRange(CalculateNearVectorMatrix(pos, CalculateMatrixSide.all));
        }
        CleanDupes();
    }
    private void Start()
    {
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
       /*     else
            {
                NonAlocatedPositions.Remove(cachedFreePos);
                iterations--;
            }*/
            iterations++;
        }
        
  
      
       if (IsMold) MoldIsland();
       
     CleanDupes_T=  new Thread(CleanDupes);
        CleanDupes_T.Start();

        foreach (Vector3 pos in AlocatedPositions)
        {
        SpawnTileCalc(pos);
           
        }

     

        #region PrepareGo's for Mesh Combineing 
        if (GrassCombine == null)
        {
            GrassCombine = new GameObject("Grass Combine");

            m_GrassCombine = GrassCombine.AddComponent<MeshFilter>();
            GrassCombine.AddComponent<MeshRenderer>();
         
        }
        if (DirtCombine == null)
        {
            DirtCombine = new GameObject("Dirt Combine");
            m_DirtCombine = DirtCombine.AddComponent<MeshFilter>();
            DirtCombine.AddComponent<MeshRenderer>();
         
        }
        if (RockCombine == null)
        {
            RockCombine = new GameObject("Rock Combine");
            m_RockCombine = RockCombine.AddComponent<MeshFilter>();
            RockCombine.AddComponent<MeshRenderer>();
    
        }
        #endregion
 CalculateNearVectorAfterSpawn = new Thread(calculateAllPositionsAfterSpawn);
       CalculateNearVectorAfterSpawn.Start();

    }
    #region MeshCombineing


    /// <summary>
    ///Use this function to optimise island mesh. Gives much performance boost. 
    ///But be carefull because this function takes a lot of time so call it only once when you finish terafforming terrain
    /// </summary>
    public void CombineMeshes()
    {
        #region old
        if (isCombineMeshes == true)
        {
            List<MeshFilter> d_meshFilterss = new List<MeshFilter>();
            List<MeshFilter> g_meshFilterss = new List<MeshFilter>();
            List<MeshFilter> r_meshFilterss = new List<MeshFilter>();
            foreach (IslandTile it in IslandTiles)
            {
                switch (it.GetBlockType())
                {
                    case IslandTile.BlockType.Dirt:
                        d_meshFilterss.Add(it.GetMeshFilter());
                        break;

                    case IslandTile.BlockType.Grass:
                        g_meshFilterss.Add(it.GetMeshFilter());
                        break;

                    case IslandTile.BlockType.Rock:
                        r_meshFilterss.Add(it.GetMeshFilter());
                        break;
                }

            }
            CombineInstance[] g_combine = new CombineInstance[g_meshFilterss.Count];
            CombineInstance[] d_combine = new CombineInstance[d_meshFilterss.Count];
            CombineInstance[] r_combine = new CombineInstance[r_meshFilterss.Count];

            for (int d = 0; d < d_meshFilterss.Count; d++)
            {
                d_combine[d].mesh = d_meshFilterss[d].sharedMesh;
                d_combine[d].transform = d_meshFilterss[d].transform.localToWorldMatrix;
            }

            for (int g = 0; g < g_meshFilterss.Count; g++)
            {
                g_combine[g].mesh = g_meshFilterss[g].sharedMesh;
                g_combine[g].transform = g_meshFilterss[g].transform.localToWorldMatrix;
            }

            for (int r = 0; r < r_meshFilterss.Count; r++)
            {
                r_combine[r].mesh = r_meshFilterss[r].sharedMesh;
                r_combine[r].transform = r_meshFilterss[r].transform.localToWorldMatrix;
            }



            m_GrassCombine.mesh = new Mesh();

            m_GrassCombine.mesh.CombineMeshes(g_combine);

            GrassCombine.GetComponent<MeshRenderer>().material = Grass;

            m_DirtCombine.mesh = new Mesh();
            m_DirtCombine.mesh.CombineMeshes(d_combine);
            DirtCombine.GetComponent<MeshRenderer>().material = Dirt;
            m_RockCombine.mesh = new Mesh();
            m_RockCombine.mesh.CombineMeshes(r_combine);
            RockCombine.GetComponent<MeshRenderer>().material = Rock;

        }
        isCombineMeshes = false;
        #endregion
    
    }

    #endregion
    private void LateUpdate()
    {
        //TESTING 
        CombineMeshes();
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
        isCombineMeshes = true;
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
    public Vector3[] CalculateNearVectorMatrix(Vector3 pos, CalculateMatrixSide cms, List<Vector3> listToAdd, bool Add = true, bool checkIsContains = true)
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
        if (Add)
        {
          /*  foreach (Vector3 p in nPos)
            {
                if (checkIsContains)
                {
                    if (!listToAdd.Contains(p))
                    {
                        listToAdd.Add(p);
                    }
                }
                else
                {
                    listToAdd.Add(p);
                }
            }*/
            listToAdd.AddRange(nPos);
        }
        else
        {
             foreach (Vector3 p in nPos)
              {
                  if (checkIsContains)
                  {
                      if (listToAdd.Contains(p))
                      { listToAdd.Remove(p); }
                  }
                  else
                  {
                      listToAdd.Remove(p);
                  }
              }
          
        }
        return nPos;
    }
    /// <summary>
    /// This function makes islands looks pretier(Fills empty cubes).
    /// </summary>
    public void MoldIsland(bool IsStart = false)
    {
        Fixing = true;

        Duplicates = NonAlocatedPositions.Distinct().ToList();
        AlocatedPositions.AddRange(Duplicates);

        if (IsStart)
        {
            foreach (Vector3 p in Duplicates)
            {
                SpawnTile(p);

            }
        }
        Fixing = false;
        Done = true;
    }
    /// <summary>
    /// This function fixes island based on DestroyedPostions Matrix.
    /// </summary>
    public void FixIsland()
    {
     
        foreach (Vector3 pos in DestroyedPositions)
        {
            SpawnTile(pos);

            RecalculateFaces(pos);
        }

        DestroyedPositions.Clear();

    }

    public void RecalculateFaces(Vector3[] pos)
    {
        foreach(Vector3 p in pos) { 
        Vector3[] PositionsToRegenerate = CalculateNearVectorMatrix(p, CalculateMatrixSide.all);

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
    }
    /// <summary>
    /// More light than FFR(). Because only goes through obects in closest position.
    /// </summary>
    /// <param name="pos"></param>
    public void RecalculateFaces(Vector3 pos)
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
    /// Very tough.
    /// </summary>
    public void ForceFaceRecalculation()
    {
        foreach(IslandTile it in IslandTiles)
        {
            it.GenerateCube();
        }
    }
    /// <summary>
    /// This function destroys tiles and recalculaters faces 
    /// </summary>
    /// <param name="itt"></param>
    public void DestroyTileRecalculateFaces(IslandTile itt, bool _isCombineMeshes = false)
    {
        DestroyTile(itt, false);
        Vector3 pos = itt.transform.position;
        RecalculateFaces(pos);
        IslandTilesDictionary.Remove(pos);
        if (_isCombineMeshes) { isCombineMeshes = true; }
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
        isCombineMeshes = true;
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
           IslandTiles.Add(it);
            IslandTilesDictionary.Add(pos, it);

            setMaterial(it, pos);
            it.SetPositionMatrix(CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight));
            it.SetGeneratorInstance(this);
      
            return it;
        }
        else
        {
            Debug.LogWarning("Cannot spawn block in existing  position"+pos);
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
            Vector3[] NearMatrix = CalculateNearVectorMatrix(pos, CalculateMatrixSide.DownForwardBackwardLeftRight);
            it.SetPositionMatrix(NearMatrix);
            foreach (Vector3 p in NearMatrix) { 
                if(!NonAlocatedPositions.Contains(p))
                {
                    NonAlocatedPositions.Add(p);
                }
            }
            it.SetGeneratorInstance(this);
            CleanDupes_T = new Thread(CleanDupes);
            CleanDupes_T.Start();
            return it;
        }
        else
        {
            Debug.LogWarning("Cannot spawn block in existing  position" + pos);
            return null;
        }
    }
   
    public IslandTile SpawnTileRandom()
    {
        int id = UnityEngine.Random.Range(0, NonAlocatedPositions.Count);
        IslandTile it= SpawnTile(NonAlocatedPositions[id]);
       // it.MeshRenderer.enabled = true;
        return it;
    }
    public List<Vector3> GetAlocatedPositions() => AlocatedPositions;
    public List<Vector3> GetNonAlocatedPositions() => NonAlocatedPositions;
    public bool CanSpawnBlocks(bool can) => CanSpawn = can;
    public bool IsMoldIsland(bool im) => IsMold = im;
    public int SetBlockAmount(int amount) => howMuchSpawn = amount;
    public int GetBlockAmount() => howMuchSpawn;
    public List<IslandTile> GetIslandTiles() => IslandTiles;
}