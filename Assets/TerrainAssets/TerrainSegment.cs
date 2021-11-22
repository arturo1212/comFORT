using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSegment
{
    public Mesh mesh;
    int xResolution;
    int yResolution;
    Vector3 origin;
    Vector3 localUp;
    Vector3 director1;
    Vector3 director2;
    Vector3[] corners;
    ShapeGenerator shapeGenerator;
    bool invisiWall;
     
    public TerrainSegment(ShapeGenerator shapeGenerator, Mesh mesh, int xResolution, int yResolution, Vector3 localUp, Vector3 origin, Vector3[] corners, bool invisiWall = false)
    {
        this.mesh = mesh;
        this.xResolution = xResolution;
        this.yResolution = yResolution;
        this.localUp = localUp;
        this.origin = origin;
        this.shapeGenerator = shapeGenerator;
        this.corners = corners;
        this.invisiWall = invisiWall;

        director1 = corners[1] - corners[0]; //x
        director2 = corners[3] - corners[0]; //y
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[xResolution * yResolution];
        int[] triangles = new int[(xResolution - 1) * (yResolution - 1) * 6]; 
        int triIndex = 0;

        //Debug.Log("Creating Vertices. Should be " + xResolution * yResolution);
        for (int i = 0; i < xResolution; i++)
        {
            for (int j = 0; j < yResolution; j++) {

                int vertexIndex = i * yResolution + j;
                //Debug.Log("vertexIndex " + vertexIndex);
                Vector2 percentage = new Vector2(i / (xResolution - 1f), j / (yResolution - 1f));
                // (j != 0 && i != 0 && j < resolution - 1 && i < resolution - 1? localUp : Vector3.zero) +
                Vector3 pointOnUnitSegmentFace = /*(invisiWall ? new Vector3(0,0,0) : localUp ) + */ corners[0] + (percentage.x) * director1 + (percentage.y) * director2;
                //Debug.Log("Percentage " + percentage.x + " " + percentage.y);
                // Vector3 pointOnUnitSphere = j !=0 && i != 0 && j < resolution - 1 && i < resolution - 1 ? pointOnUnitSegmentFace.normalized : pointOnUnitSegmentFace;
                Vector3 pointOnUnitSphere = pointOnUnitSegmentFace.normalized;
                vertices[vertexIndex] = origin + (invisiWall ? pointOnUnitSegmentFace : shapeGenerator.CalculatePointOnTerrain(pointOnUnitSegmentFace));
        
                
                if (j < yResolution - 1 && i < xResolution - 1)
                {
                    // El arreglo de traingulos es, cada 3 numeros representan las esquinas de 1 triangulo, siendo cada uno de esos 3 numeros el indice del vertice que lo compone en el 
                    // arreglo de vertices
                    triangles[triIndex] = vertexIndex + 0;
                    triangles[triIndex+1] = vertexIndex + yResolution + 1;
                    triangles[triIndex+2] = vertexIndex + yResolution;

                    triangles[triIndex+3] = vertexIndex;
                    triangles[triIndex+4] = vertexIndex + 1;
                    triangles[triIndex+5] = vertexIndex + yResolution + 1;
                    triIndex += 6;
                }
            }
        }
        //Debug.Log("number of vertices " + vertices.Length);
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    
}
