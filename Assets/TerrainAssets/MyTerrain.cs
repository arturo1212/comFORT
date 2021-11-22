using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Packages;
using Unity.AI.Navigation;

public class MyTerrain : MonoBehaviour
{
    [Range(2,256)]
    public int xResolution;
    [Range(2, 256)]
    public int yResolution;
    public float extendedRatio = 1f;
    public bool autoUpdate = true;
    public Vector3[] corners;
    [HideInInspector]
    public Vector3[] extendedCorners;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainSegment[] terrainsegments;
    TerrainSegment[] invisiWalls;
    [SerializeField, HideInInspector]
    MeshFilter[] invisibleMeshFilters;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;
    [HideInInspector]
    public bool colorSettingsFolded;
    [HideInInspector]
    public bool shapeSettingsFolded;
    public bool navMeshGenerated;

    GameObject skyField;
    NavMeshSurface navi;
    void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);


        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[1];
        }
        if (invisibleMeshFilters == null || invisibleMeshFilters.Length == 0)
        {
            invisibleMeshFilters = new MeshFilter[4];
        }
        if ( corners == null || corners.Length == 0)
        {
            corners = new Vector3[4];
        }
        corners.CopyTo(shapeSettings.subregion, 0);
        terrainsegments = new TerrainSegment[1];
        Vector3[] recorners = ExtendCorners();
        extendedCorners = recorners;
        Vector3[] originss = { new Vector3(0, 0, 0), new Vector3(0, 0, 2), new Vector3(0, 0, 4) , new Vector3(0, 0, 6), new Vector3(0, 0, 8), new Vector3(0, 0, 10) };
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 1; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshObj.AddComponent<MeshCollider>().sharedMesh = meshFilters[i].sharedMesh;
                //meshObj.AddComponent<Rigidbody>().isKinematic = true;
            }
            terrainsegments[i] = new TerrainSegment(shapeGenerator, meshFilters[i].sharedMesh, xResolution, yResolution, directions[0], originss[i], recorners);
        }
    }

    void InitializeEdgeTriggers()
    {
        int[] assignedCorners = { 0, 1, 2, 3};
        int[] assignedCorners1 = { 1, 2, 3, 0};
        Vector3[] directions = { Vector3.left, Vector3.forward, Vector3.right, Vector3.back };
        invisiWalls = new TerrainSegment[4];
        for (int i = 0; i< invisibleMeshFilters.Length; i++)
        {   
            if (invisibleMeshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("ParedInvisible");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                invisibleMeshFilters[i] = meshObj.AddComponent<MeshFilter>();
                invisibleMeshFilters[i].sharedMesh = new Mesh();
                MeshCollider invisiCollider = meshObj.AddComponent<MeshCollider>();
                invisiCollider.convex = true;
                invisiCollider.isTrigger = true;
                //meshObj.AddComponent<Rigidbody>();
            }
            invisiWalls[i] = new TerrainSegment(null, invisibleMeshFilters[i].sharedMesh, 2, 2, directions[i], new Vector3(0, 0, 0), invisiWallCorners(i), true);
        }
    }

    void GenerateInvisiWall(Mesh mesh, int corner1, int corner2)
    {
        
    }

    public void GenerateTerrain()
    {
        Initialize();
        GenerateMesh();
        GenerateSkyField();
        GenerateColors();
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
            GenerateSkyField();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainSegment segment in terrainsegments)
        {
            segment.ConstructMesh();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
        createOrRebakeNavMesh();
    }

    void createOrRebakeNavMesh()
    {
        if (!navMeshGenerated)
        {
            navi = gameObject.AddComponent<NavMeshSurface>();
            navi.BuildNavMesh();
            navMeshGenerated = true;
            return;
        }
        //navi.navMeshData;
        navi.UpdateNavMesh(navi.navMeshData);
    }

    void GenerateSkyField()
    {
        if (skyField != null)
        {
            DestroyImmediate(skyField);
        }
        skyField = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DestroyImmediate(skyField.GetComponent<MeshFilter>());
        Vector3 center = (corners[0] + corners[1] + corners[2] + corners[3])/4f;
        skyField.transform.position = center + Vector3.up;
        skyField.transform.parent = transform;
        float distToExtendedCorners = (center - extendedCorners[0]).magnitude;
        SphereCollider cielito = skyField.GetComponent<SphereCollider>();
        cielito.radius = distToExtendedCorners;
        cielito.isTrigger = true;
    }

    void GenerateColors()
    {
        foreach (MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.terrainColour;
        }
    }

    Vector3[] ExtendCorners()
    {
        // Same as the TerrainSegment director vectors
        Vector3 director1 = corners[1] - corners[0]; //x
        Vector3 director2 = corners[3] - corners[0]; //y
        Vector3[] recorners = {
        (corners[0] - director1 - director2)*extendedRatio,
        (corners[1] + director1 - director2)*extendedRatio,
        (corners[2] + director1 + director2)*extendedRatio,
        (corners[3] - director1 + director2)*extendedRatio, };
        return recorners;

    }

    Vector3[] invisiWallCorners(int wall)
    {
        int[] assignedCorners = { 0, 1, 2, 3 };
        int[] assignedCorners1 = { 1, 2, 3, 0 };
        // Same as the TerrainSegment director vectors
        Vector3 ancho = corners[assignedCorners1[wall]] - corners[assignedCorners[wall]]; //x
        Vector3 director2 = Vector3.up * 10; //y
        Vector3[] recorners = {
        // QUiza invertir + y -
        corners[assignedCorners[wall]] + director2,
        corners[assignedCorners1[wall]] + director2,
        corners[assignedCorners1[wall]] - director2,
        corners[assignedCorners[wall]] - director2};
        return recorners;

    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
