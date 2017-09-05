using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IInteractable {
    void interact();
}
public interface IAnimatable {
    void play();
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Chest_Behaviour : MonoBehaviour, IInteractable, IAnimatable {

    private Animator animator;
    public float chest_Value;
    private GameObject inventoryUI;
    private GameObject packUI;
    public GameObject ItemUIPrefab;
    [HideInInspector]
    public List<Item_Component> loot;

    private bool openState;
    private bool hasLoot = false;

    private bool interacting;
    public KeyCode interact_Key;

    [HideInInspector]
    public AudioSource audioSource;

    private void Awake() {
        animator = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        inventoryUI = GameObject.Find("Inventory_UI");
        packUI = GameObject.Find("Pack_UI");
    }

    private void Update() {
        interacting = Input.GetKeyDown(interact_Key);
    }
    private void FixedUpdate() {
        if(interacting) {
            interact();
        }
    }

    public void interact() {
        play();
        if (hasLoot == false) {
            generateLoot();
            spawnLoot();
            hasLoot = true;
        }
    }

    public void open() {
        animator.SetBool("Open", true);
        try {
            inventoryUI.GetComponent<Canvas>().enabled = true;
            packUI.GetComponent<Canvas>().enabled = true;
        } catch (Exception) {
            Debug.LogWarning("Make sure that you have the UI prefabs in the scene");
            Debug.LogWarning("Items will not be retrieved without the UI prefabs");
        }
        openState = true;
    }
    public void close() {
        animator.SetBool("Open", false);
        try {
            inventoryUI.GetComponent<Canvas>().enabled = false;
            packUI.GetComponent<Canvas>().enabled = false;
        } catch (Exception) {
            Debug.LogWarning("Could not find UI prefabs to close, please make sure that they are not being deleted");
        }
        openState = false;
    }
    public void play() {
        if(openState == false) {
            open();
        } else {
            close();
        }
    }

    private void generateLoot() {
        Item_Component item;
        float gold_Remaining = chest_Value;
        if(chest_Value == 0) {
            Debug.LogWarning("Make sure that you want " + gameObject.name.ToString() + "'s value to be zero, No items will be generated");
        }
        while (gold_Remaining > 0) {
            item = (Item_Database.Instance.findItem(gold_Remaining));
            if(item == null) {
                gold_Remaining = 0;
                continue;
            }
            loot.Add(item);
            gold_Remaining = gold_Remaining - item.myJsonData.Item_Value;
        }

        GameObject items_List = GameObject.Find("Item_List");
        if(loot != null) {
            foreach (Item_Component _item in loot) {
                try {
                    GameObject itemUIElement = Instantiate(ItemUIPrefab, items_List.transform, false);
                    itemUIElement.GetComponentInChildren<Text>().text = _item.myJsonData.Item_Name.ToString().Split(char.Parse("_"))[1];
                    foreach (Image icon in itemUIElement.GetComponentsInChildren<Image>()) {
                        if (icon.gameObject.name == "Icon") {
                            icon.sprite = _item.icon;
                        }
                    }
                    itemUIElement.GetComponent<Item_Component>().myJsonData.Item_ID = _item.myJsonData.Item_ID;
                    itemUIElement.GetComponent<Item_Component>().myJsonData.Item_Name = _item.myJsonData.Item_Name;
                    itemUIElement.GetComponent<Item_Component>().myJsonData.Item_Value = _item.myJsonData.Item_Value;
                    itemUIElement.GetComponent<Item_Component>().myJsonData.Item_Rarity = _item.myJsonData.Item_Rarity;
                    itemUIElement.GetComponent<Item_Component>().model = _item.model;
                    itemUIElement.GetComponent<Item_Component>().icon = _item.icon;
                } catch (Exception) {
                    Debug.LogWarning("Cannot add Items to chest without the UI prefabs, please add Pack_UI and Inventory_UI to the scene");
                }
            }
        }
    }
    private void spawnLoot() {
        if(loot != null) {
            foreach (Item_Component item in loot) {
                item.spawn(gameObject.transform.position, gameObject.transform.rotation);
            }
        }
    }

    
}
