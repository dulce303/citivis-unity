using UnityEngine;

///from http://wiki.unity3d.com/index.php/GetOrAddComponent
///Author: Caue Rego (cawas)
/// Creative Commons Attribution Share Alike
public static class MethodExtensionForMonoBehaviourTransform {

    static public T GetOrAddComponent<T>(this Component child) where T : Component {
        T result = child.GetComponent<T>();
        if (result == null) {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }
}
