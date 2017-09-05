using UnityEngine;
using System.Collections;

public class DataLoader : GenericSingletonClass<DataLoader> {

    public string[] items;
    
    public override void Awake(){
        StartCoroutine("getItems");
	}

    IEnumerator getItems() {
        WWW itemsData = new WWW("http://localhost/openworld_data.php");
        yield return itemsData;
        string data_String = itemsData.text;
        items = data_String.Split(':');
    }

    public bool hasLoaded() {
        if(items != null) {
            return true;
        }
        else {
            return false;
        }
    }
}


























//void Start(){
//	string data = "ID:1|Name:Health Potion|Type:consumables|Cost:50";
//	print(GetDataValue(data, "Cost:"));
//}
//
//void Update(){
//	
//}
//
//string GetDataValue(string data ,string index){
//	string value = data.Substring(data.IndexOf(index)+index.Length);
//	if(value.Contains("|"))value = value.Remove(value.IndexOf("|"));
//	return value;
//}
