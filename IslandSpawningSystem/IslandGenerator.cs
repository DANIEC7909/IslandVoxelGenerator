using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public List<Vector3> FreePositions;
    public List<Vector3> UsedPositions;
    public List<IslandTile> IslandTiles = new List<IslandTile>();

    [SerializeField] GameObject tileGameObject;

    private void Start()
    {
       FreePositions.Add(Vector3.zero);
        //TEMP
        //{
        StartCoroutine(DeleyedSpawn());
    }
    IEnumerator DeleyedSpawn()
    {

        for (int i = 0; i < 200; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnTile();
        }
        Debug.Log("<color=green>ISLAND GENERATION FINISHED!</color>");
    }
    //}
    public IslandTile SpawnTile()
    {
        int id = Random.Range(0, FreePositions.Count);
        IslandTile it = Instantiate(tileGameObject, FreePositions[id], Quaternion.identity).GetComponent<IslandTile>();
        FreePositions.Remove(FreePositions[id]);
        it.SetGeneratorReference(this);
        IslandTiles.Add(it);
        return it;
    }

}
