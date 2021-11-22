using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator 
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i=0; i< noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    bool IsInsideSubregion(Vector3 point)
    {
        // Yerba fumada de https://codereview.stackexchange.com/questions/108857/point-inside-polygon-check
        int j = settings.subregion.Length - 1;
        bool c = false;
        for (int i = 0; i < settings.subregion.Length; j = i++) c ^= settings.subregion[i].z > point.z ^ settings.subregion[j].z > point.z && point.x < (settings.subregion[j].x - settings.subregion[i].x) * (point.z - settings.subregion[i].z) / (settings.subregion[j].z - settings.subregion[i].z) + settings.subregion[i].x;
        return c;
    }

    bool IsPointInPolygon4(Vector3 testPoint)
    {
        bool result = false;
        int j = settings.subregion.Length - 1;
        for (int i = 0; i < settings.subregion.Length; i++)
        {
            if (settings.subregion[i].z < testPoint.z && settings.subregion[j].z >= testPoint.z || settings.subregion[j].z < testPoint.z && settings.subregion[i].z >= testPoint.z)
            {
                if (settings.subregion[i].x + (testPoint.z - settings.subregion[i].z) / (settings.subregion[j].z - settings.subregion[i].z) * (settings.subregion[j].x - settings.subregion[i].x) < testPoint.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    public Vector3 CalculatePointOnTerrain(Vector3 pointOnUnitSegmentFace)
    {
        float firstLayerValue = 0;
        float elevation = 0;
        float subregionMask = 1;
        if (IsInsideSubregion(pointOnUnitSegmentFace))
        {
            subregionMask = 0;
        }
        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSegmentFace);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }
        // Arrancamos desde el segundo porque ya el primero lo usamos
        for (int i=1; i<noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSegmentFace) * mask;
            }
        }
        return pointOnUnitSegmentFace  + (Vector3.up * elevation) * (settings.maskSubregion ? subregionMask : 1);
    }
}
