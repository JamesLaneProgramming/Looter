using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;



public class Material_Manager : MonoBehaviour {
    public Material material;
    public List<string> connectors;

    public void update_Material() {
        List<Renderer> renderers = new List<Renderer>();
        Looter.LooterChests.FindTypesInChildrenRecursive<Renderer>(ref renderers, gameObject.GetComponent<Transform>().transform);
        foreach (Renderer _r in renderers) {
            if (_r != null) {
                for (int i = 0; i < _r.sharedMaterials.Length; i++) {
                    if (_r.sharedMaterials[i] == null) {
                            Material[] selectionMaterials = _r.sharedMaterials;
                            selectionMaterials[i] = material;
                            _r.sharedMaterials = selectionMaterials;
                    }

                    string matName = material.name.ToString().Split(char.Parse("_"))[0];
                    if (_r.sharedMaterials[i].name.Contains(matName)) {
                        Material[] selectionMaterials = _r.sharedMaterials;
                        selectionMaterials[i] = material;
                        _r.sharedMaterials = selectionMaterials;
                    }
                }
            }
        }
    }
}


[CustomEditor(typeof(Material_Manager))]
public class Material_Manager_Editor : Editor {
    private Material_Manager manager;
    public override void OnInspectorGUI() {
        manager = (Material_Manager)target;
        Rect rect = GUILayoutUtility.GetRect(new GUIContent("Add Material"), GUIStyle.none, GUILayout.Height(16));
        GUILayout.BeginVertical();
        GUILayout.Label(new GUIContent("Add Material"));
        GUILayout.Space(10);
        manager.material = (Material)EditorGUI.ObjectField(rect, manager.material, typeof(Material), false);
        GUILayout.EndVertical();
        if (manager.material != null) {
            manager.update_Material();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
            manager.material = null;
        }
    }
}


