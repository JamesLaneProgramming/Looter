using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Material))]
public class Custom_Material : MaterialEditor {

    private int lastGroupID = 0;

    /*public override void OnEnable()
    {
        lastGroupID = Undo.GetCurrentGroup();
    }*/

    /*
     * 1. Record the amount of instances found when creating a new material
     * 2. IF instancesFound > 0
     * 3.   create new material with name "(name of material)" + instancesFound++ + ".mat"
     * 
     */

    public override void OnInspectorGUI() {
        
        if(lastGroupID == 0)
        {
            lastGroupID = Undo.GetCurrentGroup();
        }
        Material myMat = (Material)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Save"))
        {
            bool changeDesision = EditorUtility.DisplayDialog("Material Prefab Breach", "Changing this material will affect all chests using the same material, do you want to continue to change or create a new instance of the material?", "Continue", "Create new instance");
            if (changeDesision)
            {
                lastGroupID = Undo.GetCurrentGroup();
                Debug.Log("Applying to affected...");
            }
            else
            {
                Material newMat = new Material(myMat);
                AssetDatabase.CreateAsset(newMat, "Assets/Looter_Chests/ChestAssets/Models/Chest/Materials/" + newMat.name + Random.Range(0, 100000).ToString() + ".mat");
                if (Selection.activeGameObject != null)
                {
                    Transform parent = Looter.LooterChests.FindRootParent(Selection.activeGameObject.transform);
                    parent.GetComponent<Material_Manager>().material = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(newMat), typeof(Material)) as Material;
                    parent.GetComponent<Material_Manager>().update_Material();
                }
                else
                {
                    Debug.Log("Editing material in project view, Material was created but was not applied to any chest. Use the Material_Manager to apply material to any chest");
                }
                
            }
            Undo.RevertAllDownToGroup(lastGroupID);
        }
    }

    public override void OnDisable()
    {
        if(lastGroupID != Undo.GetCurrentGroup())
        {
            Debug.LogWarning("Still using");
        }
    }
}


