using UnityEngine;

public class PinchZoom : MonoBehaviour {

	public static float orthoZoomSpeed = .05f;

	
	void Update() {
		ApplyZoom();
	}

	public static void ApplyZoom () {
		if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			Camera mainCamera = Camera.main;

			// ... change the orthographic size based on the change in distance between the touches.
			mainCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
		}
	}
}
