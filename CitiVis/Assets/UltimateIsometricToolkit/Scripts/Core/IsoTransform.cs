using System.Collections.Generic;
using Assets.UltimateIsometricToolkit.Scripts.External;
using Assets.UltimateIsometricToolkit.Scripts.Utils;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Core {


	/// <summary>
	/// Isometric transform component
	/// </summary>
	[ExecuteInEditMode, DisallowMultipleComponent,AddComponentMenu("UIT/IsoTransform")]
	public class IsoTransform : MonoBehaviour {
		/// <summary>
		/// Flag to toggle isometric bounds gizmos in scene gui
		/// </summary>
		public bool ShowBounds = true; 

		/// <summary>
		/// Child IsoTransform components
		/// </summary>
		private readonly List<IsoTransform> _children = new List<IsoTransform>();
		private int _lastChildCount;

		[SerializeField, HideInInspector] private Vector3 _position = Vector3.zero;
		[SerializeField, HideInInspector] private Vector3 _size = Vector3.one;

		/// <summary>
		/// Isometric position of this GameObject in worldspace
		/// </summary>
		[ExposeProperty]
		public Vector3 Position {
			get { return _position; }
			set {
			
				var delta = value - _position;
				_position = value;
				isoSorting.Resolve(this);
//#if UNITY_EDITOR
//				EditorUtility.SetDirty(this);
//#endif 
				//apply delta to each child
				if (transform.childCount != _lastChildCount) //indicates hirarchy for this isoObj changed
					UpdateChildren();

				if (transform.childCount <= 0)
					return;
				for (var i = 0; i < _children.Count; i++) {
					_children[i]._position += delta;
					isoSorting.Resolve(this);
				}
			}
		}
		/// <summary>
		/// Isometric size of this GameObject
		/// </summary>
		[ExposeProperty]
		public Vector3 Size {
			get { return _size; }
			set {
				_size = new Vector3(Mathf.Clamp(value.x, 0, Mathf.Infinity), Mathf.Clamp(value.y, 0, Mathf.Infinity), Mathf.Clamp(value.z, 0, Mathf.Infinity));

//#if UNITY_EDITOR
//				EditorUtility.SetDirty(this);
//#endif 
			}
		}

		public Vector3 Min {
			get { return Position - Size/2; }
		}

		public Vector3 Max {
			get { return Position + Size/2; }
		}

		/// <summary>
		/// Isometric size of this GameObject
		/// </summary>
		public float Depth {
			get { return transform.position.z; }
			set {
				transform.position = new Vector3(transform.position.x, transform.position.y, value);
			}
		}
		IsoSorting _isoSorting;
		[SerializeField, HideInInspector] private IsoTransform _parent;

		private IsoSorting isoSorting {
			get {
				if (!_isoSorting && gameObject.activeInHierarchy) {
					_isoSorting = IsoSorting.Instance;
				}
				return _isoSorting;
			}
		}

		/// <summary>
		/// Updates the internal list of child isoTransforms to be updated when this instance changes
		/// </summary>
		private void UpdateChildren() {
			_children.Clear();
			for (var i = 0; i < GetComponentsInChildren<IsoTransform>().Length; i++) {
				var child = GetComponentsInChildren<IsoTransform>()[i];
				if (child != this)
					_children.Add(child);
			}
			_lastChildCount = _children.Count;
		}

		#region  Unity Events
		void OnEnable() {
			if (isoSorting != null)
				isoSorting.Resolve(this);
		}

		void OnDisable() {
			if (isoSorting != null)
				isoSorting.Remove(this);
			transform.hideFlags = HideFlags.None;
		}

		void Start() {
			if (isoSorting != null)
				isoSorting.Resolve(this);
			UpdateChildren();
		}




		#endregion
		#region Gizmos

		public void Draw() {
			Gizmos.color = Color.white;
			GizmosExtension.DrawIsoWireCube(Position, Size);
	    }

		

		void OnDrawGizmos() {
			if (!ShowBounds)
				return;
			Draw();
		}
#endregion
		/// <summary>
		/// Translates isotransform by a delta
		/// </summary>
		/// <param name="delta"></param>
		public void Translate(Vector3 delta) {
			if (delta == Vector3.zero)
				return;
			Position += delta;
		}

		public IsoTransform Parent
		{
			get { return _parent; }
			set
			{
				_parent = value;
				transform.parent = value.transform;
			}
		}
	}
}