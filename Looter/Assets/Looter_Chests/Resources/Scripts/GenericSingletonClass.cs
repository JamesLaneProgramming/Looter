using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSingletonClass<T> : MonoBehaviour where T : Component {
    private static T instance;
    public static T Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<T>();
                if (instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                    Debug.LogWarning("There was no Singleton of type: " + typeof(T).Name + " in the scene, we have added one for you.");
                    Debug.LogWarning("This is a temporary fix, please ensure that you keep a " + typeof(T).Name + " in the scene at all times");
                }
            }
            return instance;
        }
    }
    public virtual void Awake() {
        if (instance == null) {
            instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
