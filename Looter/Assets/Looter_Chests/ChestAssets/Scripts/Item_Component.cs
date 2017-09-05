using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface ISpawnable {
    void spawn();
    void spawn(Vector3 _pos, Quaternion _rot);
    void spawn(Transform _parent);
}

public class Item_Component : MonoBehaviour, ISpawnable {
    public item_Json_Data myJsonData;
    [HideInInspector]
    public GameObject model;
    public Sprite icon;
    public Image infoUI;

    private bool inPack;


    public void Init() {
        myJsonData.Item_ID = Item_Database.Instance.getFreeID();
        myJsonData.Item_Name = myJsonData.Item_ID + "_" + gameObject.name;
        string assetPath = AssetDatabase.GetAssetPath(gameObject);
        AssetDatabase.RenameAsset(assetPath, myJsonData.Item_Name);
        model = gameObject;
    }
    public bool needsUpdating() {
        Item_Database DB_Ref = Item_Database.Instance;
        for (int i = 1; i < DB_Ref.item_Data.Count; i++) {
            Debug.Log("This ID" + myJsonData.Item_ID);
            Debug.Log("Matched JSON ID" + (int)DB_Ref.item_Data[i]["Item_ID"]);
            if (myJsonData.Item_ID == (int)DB_Ref.item_Data[i]["Item_ID"]) {
                if (myJsonData.Item_Name != (string)DB_Ref.item_Data[i]["Item_Name"]) {
                    return true;
                }

                if (myJsonData.Item_Value != (int)DB_Ref.item_Data[i]["Item_Value"]) {
                    return true;
                }

                if (myJsonData.Item_Rarity != (int)DB_Ref.item_Data[i]["Item_Rarity"]) {
                    return true;
                }
            }
        }
        return false;
    }
    public bool exists() {
        Item_Database DB_Ref = Item_Database.Instance;
        DB_Ref.retrieveJsonData();
        for (int i = 1; i < DB_Ref.item_Data.Count; i++) {
            Debug.Log((string)DB_Ref.item_Data[i]["Item_Name"]);
            if (myJsonData.Item_ID == (int)DB_Ref.item_Data[i]["Item_ID"]) {
                return true;
            }
        }
        return false;
    }
    public void updateItemData() {

    }


    public void showInfo() {
        infoUI.gameObject.SetActive(true);
    }
    public void hideInfo() {
        infoUI.gameObject.SetActive(false);
    }

    public void spawn() {
        GameObject item_Instance = Instantiate(model);
        item_Instance.transform.parent = item_Instance.transform;
    }
    public void spawn(Vector3 _pos, Quaternion _rot) {
        GameObject item_Instance = Instantiate(model, _pos, _rot);
        item_Instance.transform.parent = item_Instance.transform;
    }
    public void spawn(Transform _parent) {
        Instantiate(model, _parent);
    }

    public void gainLoot() {
        if(inPack) {
            GameObject chest_List = GameObject.Find("Item_List");
            GameObject itemUIElement = Instantiate(gameObject, chest_List.transform, false);
            itemUIElement.GetComponent<Item_Component>().inPack = false;
            Destroy(gameObject);
        }
        else {
            GameObject pack_List = GameObject.Find("Pack_List");
            GameObject itemUIElement = Instantiate(gameObject, pack_List.transform, false);
            itemUIElement.GetComponent<Item_Component>().inPack = true;
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class item_Json_Data {
    [HideInInspector]
    public int Item_ID;
    [HideInInspector]
    public string Item_Name;
    public int Item_Value;
    public int Item_Rarity;

    public item_Json_Data(int _ID, string _name, int _value, int _rarity) {
        this.Item_Name = _name;
        this.Item_Value = _value;
        this.Item_Rarity = _rarity;
    }
}
