using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;


[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class Chest_Factory : MonoBehaviour {

    public bool randomiseOnLoad;
    [HideInInspector]
    public GameObject[] chest_Lids;
    [HideInInspector]
    public GameObject[] chest_Bases;
    [HideInInspector]
    public GameObject[] chest_Latches;

    private Transform baseJoint;
    private Transform lidJoint;
    private Transform latchJoint;

    [HideInInspector]
    public int baseIndex;
    [HideInInspector]
    public int lidIndex;
    [HideInInspector]
    public int latchIndex;

    [SerializeField]
    public GameObject currentBasePiece;
    [SerializeField]
    public GameObject currentLidPiece;
    [SerializeField]
    public GameObject currentLatchPiece;

    private void Init() {
        chest_Lids = Resources.LoadAll<GameObject>("Prefabs/Parts/Lids");
        chest_Bases = Resources.LoadAll<GameObject>("Prefabs/Parts/Bases");
        chest_Latches = Resources.LoadAll<GameObject>("Prefabs/Parts/Latches");

        baseJoint = gameObject.transform.GetChild(0);
        lidJoint = baseJoint.transform.GetChild(0);
        latchJoint = lidJoint.transform.GetChild(0);
    }

    private void OnEnable() {
        Init();
        //currentBasePiece = baseJoint.transform.GetChild(1).gameObject;
        //currentLidPiece = lidJoint.transform.GetChild(1).gameObject;
        //currentLatchPiece = latchJoint.transform.GetChild(1).gameObject;
        if (randomiseOnLoad) {
            randomise();
        }
    }

    public void setBase(int baseIndex) {
        Init();
        Material[] savedMaterials = currentBasePiece.GetComponent<Renderer>().materials;
        DestroyImmediate(currentBasePiece, true);
        currentBasePiece = Instantiate(chest_Bases[baseIndex], transform.position, transform.rotation, baseJoint.transform);
        currentBasePiece.transform.localPosition = new Vector3(0, 0, 0);

        setMaterials(currentBasePiece, savedMaterials);
    }
    public void setLid(int lidIndex) {
        Init();
        Material[] savedMaterials = currentLidPiece.GetComponent<Renderer>().materials;
        DestroyImmediate(currentLidPiece, true);
        currentLidPiece = Instantiate(chest_Lids[lidIndex], transform.position, transform.rotation, lidJoint.transform);
        currentLidPiece.transform.localPosition = new Vector3(0, 0, 0);

        setMaterials(currentLidPiece, savedMaterials);
    }
    public void setLatch(int latchIndex) {
        Init();
        Material[] savedMaterials = currentLatchPiece.GetComponent<Renderer>().materials;
        DestroyImmediate(currentLatchPiece, true);
        currentLatchPiece = Instantiate(chest_Latches[latchIndex], transform.position, transform.rotation, latchJoint.transform);
        currentLatchPiece.transform.localPosition = new Vector3(0, 0, 0);

        setMaterials(currentLatchPiece, savedMaterials);
    }

    public void randomise() {
        setBase(Random.Range(0, chest_Bases.Length));
        setLid(Random.Range(0, chest_Lids.Length));
        setLatch(Random.Range(0, chest_Latches.Length));
    }
    public void setMaterials(GameObject target, Material[] materialArray)
    {
        target.GetComponent<Renderer>().materials = materialArray;
    }
}

[CustomEditor(typeof(Chest_Factory))]
public class chest_Factory_Inspector : Editor {
    override public void OnInspectorGUI() {
        Chest_Factory myFactory = (Chest_Factory)target;

        GUILayout.BeginHorizontal();
        myFactory.randomiseOnLoad = GUILayout.Toggle(myFactory.randomiseOnLoad, "Randomise on Load");
        GUILayout.EndHorizontal();

        GUILayout.Label("Base: ");
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("\u25C1", "Previous Base"))) {
            myFactory.baseIndex--;
            if (myFactory.baseIndex < 0) {
                myFactory.baseIndex = myFactory.chest_Bases.Length - 1;
            }
            myFactory.setBase(myFactory.baseIndex);
        }

        if (GUILayout.Button(new GUIContent("\u25B7", "Next Base"))) {
            myFactory.baseIndex++;
            if (myFactory.baseIndex > myFactory.chest_Bases.Length - 1) {
                myFactory.baseIndex = 0;
            }
            myFactory.setBase(myFactory.baseIndex);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.Label("Lid: ");
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("\u25C1", "Previous Lid"))) {
            myFactory.lidIndex--;
            if (myFactory.lidIndex < 0) {
                myFactory.lidIndex = myFactory.chest_Lids.Length - 1;
            }
            myFactory.setLid(myFactory.lidIndex);
        }
        if (GUILayout.Button(new GUIContent("\u25B7", "Next Lid"))) {
            myFactory.lidIndex++;
            if (myFactory.lidIndex > myFactory.chest_Lids.Length - 1) {
                myFactory.lidIndex = 0;
            }
            myFactory.setLid(myFactory.lidIndex);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.Label("Latch: ");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("\u25C1", "Previous Latch"))) {
            myFactory.latchIndex--;
            if (myFactory.latchIndex < 0) {
                myFactory.latchIndex = myFactory.chest_Latches.Length - 1;
            }
            myFactory.setLatch(myFactory.latchIndex);
        }
        if (GUILayout.Button(new GUIContent("\u25B7", "Next Latch"))) {
            myFactory.latchIndex++;
            if (myFactory.latchIndex > myFactory.chest_Latches.Length - 1) {
                myFactory.latchIndex = 0;
            }
            myFactory.setLatch(myFactory.latchIndex);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (GUILayout.Button("Create Prefab")) {
            MeshFilter[] meshFilters = myFactory.gameObject.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            myFactory.gameObject.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            myFactory.gameObject.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

            AssetDatabase.CreateAsset(myFactory.gameObject.GetComponent<MeshFilter>().sharedMesh, "Assets/" + myFactory.gameObject.name + "_M" + ".asset");
            PrefabUtility.CreatePrefab("Assets/" + myFactory.gameObject.name + ".prefab", myFactory.gameObject);
            Debug.Log("Finished creating prefab in 'Assets' folder. You are free to move these into subfolders at your discretion");
        }
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        
    }
}


namespace Looter {
    public static class LooterChests {
        public static T[] FindTypesInChildren<T>(Transform _transform) {
            System.Collections.Generic.List<T> result = new List<T>();
            foreach (Transform child in _transform) {
                if (child.GetComponent<T>() != null) {
                    result.Add(child.GetComponent<T>());
                }
            }
            return result.ToArray();
        }

        public static void FindTypesInChildrenRecursive<T>(ref List<T> result, Transform _transform) {
            foreach (Transform child in _transform) {
                T component = child.GetComponent<T>();
                if (component != null) {
                    result.Add(component);
                }
                FindTypesInChildrenRecursive<T>(ref result, child);
            }
        }

        public static T FindTypeInParent<T>(Transform _transform) {
            return _transform.parent.GetComponent<T>();
        }

        public static T[] FindTypesInParentRecursive<T>(Transform _transform) {
            System.Collections.Generic.List<T> result = new List<T>();
            if (_transform.parent.GetComponent<T>() != null) {
                result.Add(_transform.parent.GetComponent<T>());
            }
            FindTypesInParentRecursive<T>(_transform.parent);
            return result.ToArray();
        }

        public static Transform FindRootParent(Transform _transform) {
            Transform currentTransform = _transform;
            while(currentTransform.parent != null) {
                currentTransform = currentTransform.parent;
            }
            return currentTransform;
        }

        public static Transform FindParent(Transform _transform, string Parent_Name) {
            Transform currentTransform = _transform;

            while (currentTransform.parent != null) {
                if(currentTransform.parent.name == Parent_Name) {
                    currentTransform = currentTransform.parent;
                    return currentTransform;
                }
                else {
                    currentTransform = currentTransform.parent;
                }
            }
            Debug.LogError("Could not find parent, ensure that their their is a parent transform with the name: " + Parent_Name, _transform);
            return null;
        }
    }
}

