using UnityEngine;
using System.Collections;

public class DebugTools
{
	public static void DrawDebugBox(Vector2 position,float angle ,Vector2 size)
	{
		float radians = (angle / 180.0f) * Mathf.PI;
		Vector2 up = new Vector2(Mathf.Sin(-radians), Mathf.Cos(-radians));
		Vector2 right = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)); 

		up.Normalize();
		right.Normalize();

		Vector2[] corners = new Vector2[4];
		corners[0] = position + up * size.y * 0.5f - right * size.x * 0.5f;
		corners[1] = position + up * size.y * 0.5f + right * size.x * 0.5f;
		corners[2] = position + -up * size.y * 0.5f + right * size.x * 0.5f;
		corners[3] = position + -up * size.y * 0.5f - right * size.x * 0.5f;

		Debug.DrawLine(corners[0], corners[1]);
		Debug.DrawLine(corners[1], corners[2]);
		Debug.DrawLine(corners[2], corners[3]);
		Debug.DrawLine(corners[3], corners[0]);

		Debug.DrawLine(position, position + up, Color.red);
		Debug.DrawLine(position, position + right, Color.blue);
	}
}
