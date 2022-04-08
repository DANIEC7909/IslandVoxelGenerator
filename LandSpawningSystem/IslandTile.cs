using System.Collections.Generic;
using UnityEngine;
public class IslandTile : MonoBehaviour
{
    int biomeID;
    IslandGenerator Generator;
    Vector3 cashedPos;
    /// <summary>
    /// 0-backward
    /// 1-forward
    /// 2-right
    /// 3-left
    /// 4-up
    /// </summary>
    public Vector3[] NearPositionMatrix = new Vector3[5];

    public MeshRenderer mr;
    public Mesh mesh;
    public MeshFilter meshFilter;
    [SerializeField] Material[] IslandMaterials;
    public enum BlockType { Grass, Dirt, Rock };
    BlockType blockType;
    [SerializeField] float length = 1f;
    [SerializeField] float width = 1f;
    [SerializeField] float height = 1f;
    GameObject _cube;


    void Start()
    {
        cashedPos = transform.position;

        mr = GetComponent<MeshRenderer>();
        //1) Create an empty GameObject with the required Components
        _cube = gameObject;
        _cube.GetComponent<MeshRenderer>();
        meshFilter = _cube.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;


        //Create a 'Cube' mesh...

        //2) Define the cube's dimensions



        //3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-length * .5f, -width * .5f, height * .5f);
        c[1] = new Vector3(length * .5f, -width * .5f, height * .5f);
        c[2] = new Vector3(length * .5f, -width * .5f, -height * .5f);
        c[3] = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        c[4] = new Vector3(-length * .5f, width * .5f, height * .5f);
        c[5] = new Vector3(length * .5f, width * .5f, height * .5f);
        c[6] = new Vector3(length * .5f, width * .5f, -height * .5f);
        c[7] = new Vector3(-length * .5f, width * .5f, -height * .5f);


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };


        //5) Define each vertex's Normal
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


        //6) Define each vertex's UV co-ordinates
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


        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
     /*   int[] triangles = new int[]
        {
            3, 1, 0,        3, 2, 1,        // Bottom	
	         7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,	    // Top
        };
*/

        //8) Build the Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        // mesh.triangles = triangles;
        RecalculateFaces(mesh);
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.Optimize();
        //   mesh.RecalculateNormals();



    }
    void RecalculateFaces(Mesh mesh)
    {
        List<Vector3> gUsedPos = Generator.UsedPositions;
        foreach (Vector3 pos in gUsedPos)
        {
       
            if (NearPositionMatrix[0] == pos)//backward{
            {
                int[] triangles = new int[]
                 {
            3, 1, 0,        3, 2, 1,        // Bottom	
	         7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	       
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,     // Top
                 };
                mesh.triangles = triangles;
            }
            if (NearPositionMatrix[1] == pos)//forward{
            {
                int[] triangles = new int[]
                {
            3, 1, 0,        3, 2, 1,        // Bottom	
	         7, 5, 4,        7, 6, 5,        // Left
	       
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,     // Top
                };
                mesh.triangles = triangles;
            }
            if (NearPositionMatrix[2] == pos)//right{
            {
                int[] triangles = new int[]
                {
            3, 1, 0,        3, 2, 1,        // Bottom	
	         7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        
	        23, 21, 20,     23, 22, 21,     // Top
                };
                mesh.triangles = triangles;
            }
            if (NearPositionMatrix[3] == pos)//left{
            {
                int[] triangles = new int[]
                {    
            3, 1, 0,        3, 2, 1,        // Bottom	

	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,     // Top
                };
                mesh.triangles = triangles;
            }
            if (NearPositionMatrix[4] == pos)//up{
            {
                int[] triangles = new int[]
                {
            3, 1, 0,        3, 2, 1,        // Bottom	
	         7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	       
                };
                mesh.triangles = triangles;
            }
        }
    
    }
    public void SetBlockType(BlockType bt)
    {
        blockType = bt;
        mr.material = IslandMaterials[(int)bt];
    }
    public BlockType GetBlockType() => blockType;
    public void SetPositionMatrix(Vector3[] npm) => NearPositionMatrix = npm;
    public void SetGeneratorInstance(IslandGenerator ig) => Generator = ig;
}
