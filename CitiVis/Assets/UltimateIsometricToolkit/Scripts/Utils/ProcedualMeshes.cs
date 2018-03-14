using UnityEngine;

namespace Assets.UltimateIsometricToolkit.Scripts.Utils {
	public static class ProcedualMeshes {

		public static Mesh GenerateCapsule(float height = 1, float radius = 0.5f, int segments = 20) {
			// make segments an even number
			if (segments % 2 != 0)
				segments++;

			// extra vertex on the seam
			var points = segments + 1;

			// calculate points around a circle
			 var pX = new float[points];
			var pZ = new float[points];
			var pY = new float[points];
			var pR = new float[points];

			var calcH = 0f;
			var calcV = 0f;

			for (var i = 0; i < points; i++) {
				pX[i] = Mathf.Sin(calcH * Mathf.Deg2Rad);
				pZ[i] = Mathf.Cos(calcH * Mathf.Deg2Rad);
				pY[i] = Mathf.Cos(calcV * Mathf.Deg2Rad);
				pR[i] = Mathf.Sin(calcV * Mathf.Deg2Rad);

				calcH += 360f / segments;
				calcV += 180f / segments;
			}


			// - Vertices and UVs -

			var vertices = new Vector3[points * (points + 1)];
			var uvs = new Vector2[vertices.Length];
			var ind = 0;

			// Y-offset is half the height minus the diameter
			var yOff = (height - radius * 2f) * 0.5f;
			if (yOff < 0)
				yOff = 0;

			// uv calculations
			var stepX = 1f / (points - 1);
			float uvX, uvY;

			// Top Hemisphere
			var top = Mathf.CeilToInt(points * 0.5f);

			for (var y = 0; y < top; y++) {
				for (var x = 0; x < points; x++) {
					vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
					vertices[ind].y = yOff + vertices[ind].y;

					uvX = 1f - stepX * x;
					uvY = (vertices[ind].y + height * 0.5f) / height;
					uvs[ind] = new Vector2(uvX, uvY);

					ind++;
				}
			}

			// Bottom Hemisphere
			var btm = Mathf.FloorToInt(points * 0.5f);

			for (var y = btm; y < points; y++) {
				for (var x = 0; x < points; x++) {
					vertices[ind] = new Vector3(pX[x] * pR[y], pY[y], pZ[x] * pR[y]) * radius;
					vertices[ind].y = -yOff + vertices[ind].y;

					uvX = 1f - stepX * x;
					uvY = (vertices[ind].y + height * 0.5f) / height;
					uvs[ind] = new Vector2(uvX, uvY);

					ind++;
				}
			}


			

			var triangles = new int[segments * (segments + 1) * 2 * 3];

			for (int y = 0, t = 0; y < segments + 1; y++) {
				for (var x = 0; x < segments; x++, t += 6) {
					triangles[t + 0] = (y + 0) * (segments + 1) + x + 0;
					triangles[t + 1] = (y + 1) * (segments + 1) + x + 0;
					triangles[t + 2] = (y + 1) * (segments + 1) + x + 1;

					triangles[t + 3] = (y + 0) * (segments + 1) + x + 1;
					triangles[t + 4] = (y + 0) * (segments + 1) + x + 0;
					triangles[t + 5] = (y + 1) * (segments + 1) + x + 1;
				}
			}


			
			var mesh = new Mesh {
				name = "ProceduralCapsule",
				vertices = vertices,
				uv = uvs,
				triangles = triangles
			};

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			//mesh.Optimize();
			return mesh;
		}
	}
}
