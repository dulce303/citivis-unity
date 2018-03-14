using UnityEngine;
using Assets.UltimateIsometricToolkit.Scripts.Core;

namespace UltimateIsometricToolkit.controller { 
/// <summary>
/// Simple continuous movement with WSAD/Arrow Keys movement.
/// Note: This is an exemplary implementation. You may vary inputs, speeds, etc.
/// </summary>
	[AddComponentMenu("UIT/CharacterController/Simple Controller")]
	public class SimpleIsoObjectController : MonoBehaviour {

		public float Speed = 10;
		
		private IsoTransform _isoTransform;
		
		void Awake() {
			_isoTransform = this.GetOrAddComponent<IsoTransform>(); //avoids polling the IsoTransform component per frame
		}

		void Update() {
			//translate on isotransform
			_isoTransform.Translate(new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal")) * Time.deltaTime * Speed);
		}
	}
}
