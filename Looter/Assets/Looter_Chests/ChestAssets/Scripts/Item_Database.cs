using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;

using UnityEditor;



public class Item_Database : GenericSingletonClass<Item_Database> {
    public List<Item_Component> items;
    public JsonData item_Data;
    public string jsonPath;

    [HideInInspector]
    public bool updateDatabase;
    [HideInInspector]
    public bool addItems;

    public GameObject newItem;

    public float getTotalDropChance(float gold_left) {
        float total = 0f;
        if (items.Count != 0) {
            foreach (Item_Component item in items) {
                if (item.myJsonData.Item_Value <= gold_left) {
                    total += item.myJsonData.Item_Rarity;
                }
            }
        }
        //print(total);
        return total;
    }
    public Item_Component findItem(float gold_left) {
        loadItems();
        float remaining_chance = getTotalDropChance(gold_left);
        float random_Chance = UnityEngine.Random.Range(0, remaining_chance);
        if (items.Count != 0 && random_Chance != 0) {
            foreach (Item_Component item in items) {
                if (item.myJsonData.Item_Value <= gold_left) {
                    remaining_chance = remaining_chance - item.myJsonData.Item_Rarity;
                    if (remaining_chance <= random_Chance) {
                        return item;
                    }
                }
            }
        }
        return null;
    }
    public int getFreeID() {
        for (int i = 0; i < item_Data.Count; i++) {
            //(n(n + 1))/2
            //(i(i + 1))/2
            if ((i * (i + 1) / 2) != ((int)item_Data[i]["Item_ID"] * ((int)item_Data[i]["Item_ID"] + 1) / 2)) {
                return i;
            }
            /*if ((int)item_Data[i]["Item_ID"] - lastID > 1) {
                return (i);
            } else {
                lastID = (int)item_Data[i]["Item_ID"];
            }*/
        }
        return item_Data.Count;
    }

    public void initialiseJsonFile() {
        jsonPath = Application.dataPath + "/Looter_Chests/Resources/items.json";
    }
    private void loadItems() {
        items = new List<Item_Component>();
        foreach (GameObject prefab in Resources.LoadAll<GameObject>("Prefabs/Items")) {
            if(prefab.GetComponent<Item_Component>()) {
                items.Add(prefab.GetComponent<Item_Component>());
            } else {
                Debug.LogError("The Item: " + prefab.name + " does not have an 'Item_Component' script attatched");
            }
        }
    } 

    public void updateLocalJsonData() {
        initialiseJsonFile();
        retrieveJsonData();
        /*foreach (JsonData item in item_Data) {
            Debug.Log(item);
            if((int)item["Item_ID"] != 0) {
                Debug.Log((int)item["Item_ID"]);
                items.ToArray()[(int)item["Item_ID"] - 1].myJsonData.Item_Name = (string)item[(int)item["Item_ID"] - 1]["Item_Name"];
                items.ToArray()[(int)item["Item_ID"] - 1].myJsonData.Item_Value = (int)item[(int)item["Item_ID"] - 1]["Item_Value"];
                items.ToArray()[(int)item["Item_ID"] - 1].myJsonData.Item_Rarity = (int)item[(int)item["Item_ID"] - 1]["Item_Rarity"];
            }
        }*/
        for (int i = 1; i < items.Count; i++) {
            try {
                items.ToArray()[i - 1].myJsonData.Item_ID = (int)item_Data[i]["Item_ID"];
                items.ToArray()[i - 1].myJsonData.Item_Name = (string)item_Data[i]["Item_Name"];
                items.ToArray()[i - 1].myJsonData.Item_Value = (int)item_Data[i]["Item_Value"];
                items.ToArray()[i - 1].myJsonData.Item_Rarity = (int)item_Data[i]["Item_Rarity"];
            } catch (Exception) {
                Debug.LogError("There is json data waiting to update an item");
                Debug.LogError("Please ensure that number of items matches the amount of json information stored in items.json");
            }
        }
    }

    public void retrieveJsonData() {
        initialiseJsonFile();
        loadItems();
        try {
            item_Data = JsonMapper.ToObject(File.ReadAllText(jsonPath));
        } catch (Exception e) {
            Debug.Log(e);
            resetJsonFile();
            item_Data = JsonMapper.ToObject(File.ReadAllText(jsonPath));
        }
        if (JsonMapper.ToObject(File.ReadAllText(jsonPath)).ToString() == "Uninitialized JsonData") {
            print("un");
            resetJsonFile();
            item_Data = JsonMapper.ToObject(File.ReadAllText(jsonPath));
        }
    }

    public void resetJsonFile() {
        Debug.LogWarning("Item Database was corrupt, reseting json file");
        using (StreamWriter sw = new StreamWriter(jsonPath)) {
            sw.WriteLine("[\n" + "{\"Item_ID\":0,\"Item_Name\":\"DO_NOT_TOUCH / DELETE\",\"Item_Value\":100000000,\"Item_Rarity\":0}" + "\n]");
        }
    }
    public void updateItemData() {
        initialiseJsonFile();
        JsonData new_Item_data = JsonMapper.ToJson(newItem.GetComponent<Item_Component>().myJsonData);
        string tempFile = Path.GetTempFileName();

        using (var sr = new StreamReader(jsonPath))
        using (var sw = new StreamWriter(tempFile)) {
            string line;

            while ((line = sr.ReadLine()) != null) {
                if (line != "]")
                    sw.WriteLine(line);
            }
        }

        File.Delete(jsonPath);
        File.Move(tempFile, jsonPath);

        StreamWriter writer = new StreamWriter(jsonPath, true);
        writer.WriteLine("," + new_Item_data.ToString() + "\n" + "]");
        writer.Close();
        updateLocalJsonData();
    }

    public void syncItemFromJson(GameObject item) {
        EditorUtility.SetDirty(item);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void createItem(GameObject newItem) {
        initialiseJsonFile();
        JsonData new_Item_data = JsonMapper.ToJson(newItem.GetComponent<Item_Component>().myJsonData);
        string tempFile = Path.GetTempFileName();

        using (var sr = new StreamReader(jsonPath))
        using (var sw = new StreamWriter(tempFile)) {
            string line;

            while ((line = sr.ReadLine()) != null) {
                if (line != "]")
                    sw.WriteLine(line);
            }
        }

        File.Delete(jsonPath);
        File.Move(tempFile, jsonPath);

        StreamWriter writer = new StreamWriter(jsonPath, true);
        writer.WriteLine("," + new_Item_data.ToString() + "\n" + "]");
        writer.Close();
        updateLocalJsonData();
    }
}


[CustomEditor(typeof(Item_Database))]
public class DatabaseEditor : Editor {
    override public void OnInspectorGUI() {
        serializedObject.Update();
        Item_Database myDB = (Item_Database)target;

        EditorGUI.BeginChangeCheck();
        GUILayout.BeginVertical();
        GUIContent addButtonContent = new GUIContent("Add Items", "Search for and add Items to database");
        if (GUILayout.Button(addButtonContent)) {
            myDB.addItems = true;
        }
        GUIContent updateButtonContent = new GUIContent("Update Items", "Updates prefabs in /Prefabs/Items to match json data");
        if(GUILayout.Button(updateButtonContent))
        {
            myDB.updateDatabase = true;
        }
        GUILayout.EndVertical();
        if(EditorGUI.EndChangeCheck()) {
            Console.WriteLine("Editor Change Detected");
        }
#region EditorActions
        if (myDB.updateDatabase) {
            myDB.initialiseJsonFile();
            Debug.Log("Checking for updates");
            myDB.updateLocalJsonData();
            Debug.Log("local items updated");
            myDB.updateDatabase = false;
        }
        if(myDB.addItems) {
            myDB.initialiseJsonFile();
            GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Items");
            foreach (GameObject item in prefabs) {
                Item_Component itemComponent = item.GetComponent<Item_Component>();
                if (itemComponent.exists() == false) {
                    if(itemComponent.needsUpdating()) {
                        Debug.Log("needs updating");
                        myDB.syncItemFromJson(item);
                    } else {
                        Debug.Log("Creating new Json data for item found");
                        itemComponent.Init();
                        myDB.createItem(item);
                    }
                }
                else {
                    Debug.Log("Item Found, Didn't need to update");
                }
            }
            myDB.addItems = false;
        }
#endregion
    }
}
