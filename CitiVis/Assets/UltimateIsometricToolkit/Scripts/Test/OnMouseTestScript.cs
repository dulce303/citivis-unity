using UnityEngine;

/// <summary>
/// Tests if OnMouse callbacks get invoked properly
/// </summary>
public class OnMouseTestScript : MonoBehaviour {

	void OnMouseOver() {
		Debug.Log("OnMouseOver " + name);
	}

	void OnMouseEnter() {
		Debug.Log("OnMouseEnter " + name);
	}
	void OnMouseExit() {
		Debug.Log("OnMouseExit " + name);
	}

	void OnMouseDown() {
		Debug.Log("OnMousedown " + name);
	}

	void OnMouseUp() {
		Debug.Log("OnMouseUp " + name);
	}
	void OnMouseDrag() {
		Debug.Log("OnMouseDrag " + name);
	}
}
