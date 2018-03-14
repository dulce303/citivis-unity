using System;
using Assets.UltimateIsometricToolkit.Scripts.Core;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
using UnityEditor;

namespace UltimateIsometricToolkit {
	[CustomEditor(typeof(IsoSorting)), CanEditMultipleObjects]
	public class IsoSortingEditor : Editor {

		IsoSorting instance;
		PropertyField[] instance_Fields;
		void OnEnable() {
			instance = target as IsoSorting;
			instance_Fields = ExposeProperties.GetProperties(instance);
		}

		public override void OnInspectorGUI() {
			if (instance == null)
				return;
			DrawDefaultInspector();
			switch (instance.Projection) {
				case Isometric.Projection.Isometric:
				case Isometric.Projection.Dimetric1To2:
				case Isometric.Projection.Military:
				case Isometric.Projection.Dimetric42To7:
					break;
				case Isometric.Projection.DimetricCustom:
					instance.XRot = EditorGUILayout.FloatField("XRotation", instance.XRot);
					break;
				case Isometric.Projection.OrthographicCustom:
					instance.XRot = EditorGUILayout.FloatField("XRotation", instance.XRot);
					instance.YRot = EditorGUILayout.FloatField("YRotation", instance.YRot);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			ExposeProperties.Expose(instance_Fields);
		}
	}
}