using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyTerrain))]
public class TerrainEditor : Editor
{
    MyTerrain terrain;
    Editor colorEditor;
    Editor shapeEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                terrain.GenerateTerrain();
            }

        }
        if (GUILayout.Button("Generate Terrain"))
        {
            terrain.GenerateTerrain();
        }
        DrawSettngsEditor(terrain.shapeSettings, terrain.OnShapeSettingsUpdated, ref terrain.shapeSettingsFolded, ref shapeEditor);
        DrawSettngsEditor(terrain.colorSettings, terrain.OnColorSettingsUpdated, ref terrain.colorSettingsFolded, ref colorEditor);
    }

    void DrawSettngsEditor(Object settings, System.Action onSettingsUpdated, ref bool folded, ref Editor editor)
    {
        if (settings != null)
        {
            folded = EditorGUILayout.InspectorTitlebar(folded, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (folded)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            //onSettingsUpdated();
                        }
                    }
                }

            }
        }
        
    }

    private void OnEnable()
    {
        terrain = (MyTerrain) target; 
    }
}
