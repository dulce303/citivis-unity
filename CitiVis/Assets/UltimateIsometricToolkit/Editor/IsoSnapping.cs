using UnityEngine;
using UnityEditor;
using System.Linq;
using Assets.UltimateIsometricToolkit.Scripts.Core;

namespace UltimateIsometricToolkit {
	public class IsoSnapping : EditorWindow {
		//Snap IsoTransform to multiple of this vector 
		public static Vector3 SnappingVector = Vector3.one;

		//enable - disable
		public static bool DoSnap;

		[MenuItem("Tools/UIT/IsometricSnapping %_l")]
		static void Init() {
			var window = (IsoSnapping)GetWindow(typeof(IsoSnapping));
			window.maxSize = new Vector2(400, 200);
			
		}

		void OnEnable() {
			var values = EditorPrefs.GetString("snappingVector").Split(';');
			SnappingVector.x = float.Parse(values[0]);
			SnappingVector.y = float.Parse(values[1]);
			SnappingVector.z = float.Parse(values[2]);

			DoSnap = EditorPrefs.GetBool("doSnap");
		}

		void OnDisable() {
			EditorPrefs.SetString("snappingVector", SnappingVector.x + ";" + SnappingVector.y + ";" + SnappingVector.z + ";");
			EditorPrefs.SetBool("doSnap", DoSnap);
		}

		public void OnGUI() {
			DoSnap = EditorGUILayout.Toggle(new GUIContent("Auto Snap", (DoSnap ? "Disable" : "Enable") + " automatic snapping for IsoTransforms"), DoSnap);
			EditorUtility.SetDirty(this);
			SnappingVector = EditorGUILayout.Vector3Field(new GUIContent("Snap Value", "Selection will snap to a closest multilpe in each direction"), SnappingVector);
			if (SnappingVector.x == 0 || SnappingVector.y == 0 || SnappingVector.z == 0) {
				DoSnap = false;
				EditorGUILayout.HelpBox("Snapping to a multiple of zero not allowed", MessageType.Warning);
			}

			if (GUILayout.Button(new GUIContent("Snap selection", "Snap the current selection in Scene view to the  \n closest multiple of the snapping Vector"))) {
				foreach (var isoObj in Selection.gameObjects.Where(c => c.GetComponent<IsoTransform>() != null).ToList().Select(obj => obj.GetComponent<IsoTransform>())) {
					isoObj.Position = Round(isoObj.Position);
				}
			}

			GUILayout.Space(10);
		}

		/// <summary>
		/// Ceils input vector to next multiple of SnappingVector
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>

		public static Vector3 Ceil(Vector3 input) {
			var x = SnappingVector.x * Mathf.Ceil((input.x / SnappingVector.x));
			var y = SnappingVector.y * Mathf.Ceil((input.y / SnappingVector.y));
			var z = SnappingVector.z * Mathf.Ceil((input.z / SnappingVector.z));

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Rounds input vector to the next multiple SnappingVector 
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Vector3 Round(Vector3 input) {
			var x = SnappingVector.x * Mathf.Round((input.x / SnappingVector.x));
			var y = SnappingVector.y * Mathf.Round((input.y / SnappingVector.y));
			var z = SnappingVector.z * Mathf.Round((input.z / SnappingVector.z));

			return new Vector3(x, y, z);
		}

	}
}