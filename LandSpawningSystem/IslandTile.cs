using System.Collections.Generic;

using UnityEngine;
public class IslandTile : MonoBehaviour
{
    int biomeID;
    IslandGenerator Generator;

    public Vector3[] NearPositionMatrix = new Vector3[5];

    [SerializeField] MeshRenderer mr;
    [SerializeField] Mesh mesh;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Material[] GrassIslandMaterials;
    [SerializeField] Material[] DirtIslandMaterials;
    [SerializeField] Material[] RockIslandMaterials;
    public enum BlockType { Grass, Dirt, Rock };
    BlockType blockType;
    [SerializeField] float length = 1f;
    [SerializeField] float width = 1f;
    [SerializeField] float height = 1f;
    [SerializeField]BoxCollider TileCollider;
    GameObject _cube;


    void Start()
    {

        GenerateCube();
        mr = GetComponent<MeshRenderer>();
        _cube.GetComponent<MeshRenderer>();
        meshFilter = _cube.GetComponent<MeshFilter>();
        gameObject.isStatic = true;
       // TileCollider.GetComponent<BoxCollider>();
    }

    public void GenerateCube()
    {
        _cube = gameObject;
        mesh = meshFilter.mesh;

        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-length * .5f, -width * .5f, height * .5f);
        c[1] = new Vector3(length * .5f, -width * .5f, height * .5f);
        c[2] = new Vector3(length * .5f, -width * .5f, -height * .5f);
        c[3] = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        c[4] = new Vector3(-length * .5f, width * .5f, height * .5f);
        c[5] = new Vector3(length * .5f, width * .5f, height * .5f);
        c[6] = new Vector3(length * .5f, width * .5f, -height * .5f);
        c[7] = new Vector3(-length * .5f, width * .5f, -height * .5f);
 
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };

        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normals = new Vector3[]
        {
            down, down, down, down,             // Bottom
	        left, left, left, left,             // Left
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
	        right, right, right, right,         // Right
	        up, up, up, up	                    // Top
        };

        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            uv11, uv01, uv00, uv10, // Bottom
	        uv11, uv01, uv00, uv10, // Left
	        uv11, uv01, uv00, uv10, // Front
	        uv11, uv01, uv00, uv10, // Back	        
	        uv11, uv01, uv00, uv10, // Right 
	        uv11, uv01, uv00, uv10  // Top
        };


        mesh.Clear();
        mesh.vertices = vertices;
        // mesh.triangles = triangles;
        RecalculateFaces(ref mesh);
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.Optimize();
        //   mesh.RecalculateNormals();


    }

    void RecalculateFaces(ref Mesh mesh)
    {
        List<Vector3> gUsedPos = Generator.AlocatedPositions;
        if (Generator.AlocatedPositions.Contains(transform.position + Vector3.down) && Generator.AlocatedPositions.Contains(transform.position + Vector3.left) && Generator.AlocatedPositions.Contains(transform.position + Vector3.forward) && Generator.AlocatedPositions.Contains(transform.position + Vector3.back) && Generator.AlocatedPositions.Contains(transform.position + Vector3.right) && Generator.AlocatedPositions.Contains(transform.position + Vector3.up))
        {
            name = "nowalls";
            TileCollider.enabled = false;
            mesh.triangles = new int[6];
        }
        else
        {
            List<int> Triangles = new List<int>();
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.down))//bottom
            { }
            else
            {

                Triangles.Add(3);
                Triangles.Add(1);
                Triangles.Add(0);
                Triangles.Add(3);
                Triangles.Add(2);
                Triangles.Add(1);
                name = name + " bottom";
                TileCollider.enabled = true;
            }
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.left))//left
            { }
            else
            {
                Triangles.Add(7);
                Triangles.Add(5);
                Triangles.Add(4);
                Triangles.Add(7);
                Triangles.Add(6);
                Triangles.Add(5);
                name = name + " left";
                TileCollider.enabled = true;

            }
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.forward))//front  
            { }
            else
            {
                Triangles.Add(11);
                Triangles.Add(9);
                Triangles.Add(8);
                Triangles.Add(11);
                Triangles.Add(10);
                Triangles.Add(9);
                name = name + " front";
                TileCollider.enabled = true;
            }
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.back))//back
            { }
            else
            {
                Triangles.Add(15);
                Triangles.Add(13);
                Triangles.Add(12);
                Triangles.Add(15);
                Triangles.Add(14);
                Triangles.Add(13);
                name = name + " back";
                TileCollider.enabled = true;
            }
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.right))//right
            { }
            else
            {
                Triangles.Add(19);
                Triangles.Add(17);
                Triangles.Add(16);
                Triangles.Add(19);
                Triangles.Add(18);
                Triangles.Add(17);
                name = name + " right";
                TileCollider.enabled = true;
            }
            if (Generator.AlocatedPositions.Contains(transform.position + Vector3.up))//top
            { }
            else
            {
                Triangles.Add(23);
                Triangles.Add(21);
                Triangles.Add(20);
                Triangles.Add(23);
                Triangles.Add(22);
                Triangles.Add(21);
                name = name + " top";
                TileCollider.enabled = true;
            }
            mesh.triangles = Triangles.ToArray();
        }

    }

    public void SetBlockType(BlockType bt)
    {
        blockType = bt;
        switch (bt)
        {
            case BlockType.Grass:
                mr.material = GrassIslandMaterials[Random.Range(0, GrassIslandMaterials.Length)];
                break;
            case BlockType.Dirt:
                mr.material = DirtIslandMaterials[Random.Range(0, DirtIslandMaterials.Length)];
                break;
            case BlockType.Rock:
                mr.material = RockIslandMaterials[Random.Range(0, RockIslandMaterials.Length)];
                break;
        }
    }
  
    public BlockType GetBlockType() => blockType;
    public void SetPositionMatrix(Vector3[] npm) => NearPositionMatrix = npm;
    public void SetGeneratorInstance(IslandGenerator ig) => Generator = ig;
}
