using System;
using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Utils {
	/// <summary>
	/// Isometric helper class with utility functions
	/// </summary>
	public class Isometric {
		[SerializeField]
		private static Matrix4x4 _isoMatrix = Matrix4x4.identity;

		public static Matrix4x4 IsoMatrix {
			get {
				
				return _isoMatrix;
			}
			set { _isoMatrix = value; }
		}


		/// <summary>
		///  Rotates a vector from isometric space to unity space
		/// </summary>
		/// <param name="isoVector">Isometric Vector</param>
		/// <returns>Vector in screen coordinates</returns>
		public static Vector3 IsoToUnitySpace(Vector3 isoVector) {
			
			return IsoMatrix.MultiplyPoint(isoVector);
		}

		/// <summary>
		///  Rotates a vector from isometric space to unity space
		/// </summary>
		/// <param name="isoVector">Isometric Vector</param>
		/// <returns>Vector in screen coordinates</returns>
		[Obsolete("renaming, please use IsoToUnitySpace instead")]
		public static Vector3 IsoToScreen(Vector3 isoVector) {

			return IsoMatrix.MultiplyPoint(isoVector);
		}

		/// <summary>
		///  Rotates a vector from unity space to isometric space
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3 UnityToIsoSpace(Vector3 vector) {
			return IsoMatrix.inverse.MultiplyPoint(vector);
		}
		/// <summary>
		///  Rotates a vector from unity's coordinate system to the isometric coordinate system
		/// </summary>
		/// <param name="vector"></param>
		/// <returns>Vector in isometric space</returns>
		 [Obsolete("renaming, please use UnityToIsoSpace instead")]
		public static Vector3 ScreenToIso(Vector3 vector) {
			return IsoMatrix.inverse.MultiplyPoint(vector);
		}


		/// <summary>
		/// returns the matrix that convers from isometric space to unity space
		/// </summary>
		/// <returns></returns>
		[Obsolete("use GetProjectionMatrix instead")]
		public static Matrix4x4 GetIsoMatrix(float isoAngle) {
			var isoAngleInRad = Mathf.Deg2Rad * isoAngle;
			var arcsintan = Mathf.Asin(Mathf.Tan(isoAngleInRad)) * Mathf.Rad2Deg;
			var rot = Quaternion.AngleAxis(-arcsintan, Vector3.right) * Quaternion.AngleAxis(-45, Vector3.up);
			return Matrix4x4.TRS(Vector3.zero, rot, Vector3.one);
		}

		public static Matrix4x4 GetProjectionMatrix(Projection projection, float xRot,float yRot) {
			Matrix4x4 projectionMatrix;
			switch (projection) {
				case Projection.Dimetric1To2:
					projectionMatrix = GetOrthographicProjectionMatrix(30, 45);
					break;
				case Projection.DimetricCustom:
					projectionMatrix = GetOrthographicProjectionMatrix(xRot, 45);
					break;
				case Projection.Isometric:
					projectionMatrix = GetOrthographicProjectionMatrix(35.625f, 45);
					break;
				case Projection.Military:
					projectionMatrix = GetOrthographicProjectionMatrix(45, 45);
					break;
				case Projection.Dimetric42To7:
					projectionMatrix = GetOrthographicProjectionMatrix(20, 70);	
					break;
				case Projection.OrthographicCustom:
					projectionMatrix = GetOrthographicProjectionMatrix(xRot, yRot);
					break;
				default:
					throw new ArgumentOutOfRangeException("projection", projection, null);
			}
			return projectionMatrix;

		}
		/// <summary>
		/// Returns an orthographic projection matrix with a rotation around the global x and y axis
		/// </summary>
		/// <param name="xRot"></param>
		/// <param name="yRot"></param>
		/// <returns></returns>
		private static Matrix4x4 GetOrthographicProjectionMatrix(float xRot, float yRot) {
			return Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-xRot, Vector3.right)*Quaternion.AngleAxis(-yRot, Vector3.up),
				Vector3.one);
		}

		private static Quaternion GetQuaternion(float xRot, float yRot) {
			return Quaternion.AngleAxis(-xRot, Vector3.right)*Quaternion.AngleAxis(-yRot, Vector3.up);
		}

		/// <summary>
		/// Returns the quaternion that rotates from Unity to Iso space given a projection,global xRot and yRot
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="xRot"></param>
		/// <param name="yRot"></param>
		/// <returns></returns>
		public static Quaternion GetProjecitonQuaternion(Projection projection, float xRot, float yRot) {
			Quaternion quaternion;
			switch (projection) {
				case Projection.Dimetric1To2:
					quaternion = GetQuaternion(30, 45);
					break;
				case Projection.DimetricCustom:
					quaternion = GetQuaternion(xRot, 45);
					break;
				case Projection.Isometric:
					quaternion = GetQuaternion(35.625f, 45);
					break;
				case Projection.Military:
					quaternion = GetQuaternion(45, 45);
					break;
				case Projection.Dimetric42To7:
					quaternion = GetQuaternion(20, 70);
					break;
				case Projection.OrthographicCustom:
					quaternion = GetQuaternion(xRot, yRot);
					break;
				default:
					throw new ArgumentOutOfRangeException("projection", projection, null);
			}
			return quaternion;
		}

		/// <summary>
		/// Calculates a position in isometric-space relative to a xy position in unity space and an offset from an arbitrary plane in isometric space
		/// </summary>
		/// <param name="xyPos"></param>
		/// <param name="planeNormalVector"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static Vector3 UnityToIsoSpace(Vector2 xyPos, Vector3 planeNormalVector, float offset) {
			
			var plane = new Plane(planeNormalVector, offset); //isometric plane that goes through Offset

			var matrixInverse = IsoMatrix.inverse;
			var isoRay = new Ray(matrixInverse.MultiplyPoint(xyPos), matrixInverse.MultiplyPoint(new Vector3(0, 0, 1))); //create a ray that points in the camera direction and has 

			float distance;
			if (plane.Raycast(isoRay, out distance)) {
				return isoRay.GetPoint(distance);
			}
			throw new Exception("ray and plane never intersect");
		}

		public Vector3 ScreenToIsoPoint(Vector2 screenSpacePoint, float zOffset = 0f, Camera cam = null) {
			if(cam == null)
				cam = Camera.main;
			var worldPos = cam.ScreenToWorldPoint(new Vector3(screenSpacePoint.x, screenSpacePoint.y, Camera.main.nearClipPlane));
			return UnityToIsoSpace(worldPos,new Vector3(0,0,1),zOffset);
		}


		/// <summary>
		/// Utility function for raycasting from a screenspace point 
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <returns></returns>
		public static Ray ScreenSpaceToIsoRay(Vector2 screenSpacePoint, Camera camera = null) {
			if(camera == null)
				camera = Camera.main;
			var ray = camera.ScreenPointToRay(screenSpacePoint);

			//rotate the origin and direction to the isometric coordinate system 
			var isoRayOrigin = UnityToIsoSpace(ray.origin);
			var isoRayDirection = UnityToIsoSpace(ray.direction);

			return new Ray(isoRayOrigin, isoRayDirection);
		}

		/// <summary>
		/// Utility function for raycasting from the mouse position
		/// </summary>
		/// <param name="screenSpacePoint"></param>
		/// <returns></returns>
		public static Ray MouseToIsoRay() {
			return ScreenSpaceToIsoRay(Input.mousePosition);
		}

		public enum Projection {
			Isometric,
			Dimetric1To2,
			DimetricCustom,	
			Military,
			Dimetric42To7,
			OrthographicCustom
		}
	}
}
