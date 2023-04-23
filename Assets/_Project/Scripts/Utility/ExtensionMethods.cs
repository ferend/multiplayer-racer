using UnityEngine;

namespace Utility
{
	public static class ExtensionMethods
	{

		// So we can use the null-propagation operator with Unity objects
		public static T Ref<T>(this T obj) where T : Object
		{
			return obj == null ? null : obj;
		}

		public static float AspectRatio(this Texture texture)
		{
			return texture.width / texture.height;
		}

		// Clamp bottom
		public static float AtLeast(this float v, float minVal) => Mathf.Max(v, minVal);
		public static int AtLeast(this int v, int minVal) => Mathf.Max(v, minVal);


		// The circle constant
		// Amount one full turn in radiance.

		public const float TAU = 6.28318530718f;

		// Returns a normalized vector given an angle in radians
		public static Vector2 GetUnitVectorByAngle(float angRad)
		{
			return new Vector2(
				Mathf.Cos(angRad),
				Mathf.Sin(angRad)
			);
		}

		// Given a min (a) and a max (b) value,
		// this returns the percentage at which
		// v lies, in that range
		static float InverseLerp(float a, float b, float v)
		{
			return (v - a) / (b - a);
		}

		// Remaps a value in one range into another
		public static float Remap(float iMin, float iMax, float oMin, float oMax, float v)
		{
			float t = InverseLerp(iMin, iMax, v);
			return Mathf.LerpUnclamped(oMin, oMax, t);
		}

		// Extension method for round.
		public static Vector3 Round(this Vector3 v)
		{
			v.x = Mathf.Round(v.x);
			v.y = Mathf.Round(v.y);
			v.z = Mathf.Round(v.z);

			return v;
		}

		/// <summary>
		/// Rescaling it in that space and then scaling it up again
		/// </summary>
		/// <param name="v"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static Vector3 Round(this Vector3 v, float size)
		{
			return (v / size).Round() * size;
		}

		public static float Round(this float v, float size)
		{
			return Mathf.Round(v / size) * size;
		}

		public static float GetEased(this Ease ease, float t)
		{
			switch (ease)
			{
				case Ease.In: return t * t;
				case Ease.Out: return (2 - t) * t;
				case Ease.InOut: return -t * t * (2 * t - 3);
				default: return t;
			}
		}

		public static Quaternion Rotation(float angleA, float angleB, float angleC) => Quaternion.Euler(angleA, angleB, angleC);

		public static Quaternion InverseRotation(Quaternion rot) => Quaternion.Inverse(rot);

	}

}
