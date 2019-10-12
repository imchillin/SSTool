using System;
using System.Windows.Media.Media3D;

namespace FFXIVTool.Utility
{
	public static class Extensions
	{
		private static float Deg2Rad = (float)(Math.PI * 2) / 360;
		private static float Rad2Deg = 360 / (float)(Math.PI * 2);

		/// <summary>
		/// Convert into a Quaternion assuming the Vector3D represents euler angles.
		/// </summary>
		/// <param name="self"></param>
		/// <returns>Quaternion from Euler angles.</returns>
		public static Quaternion ToQuaternion(this Vector3D self)
		{
			var v = self;
			v.X *= Deg2Rad;
			v.Y *= Deg2Rad;
			v.Z *= Deg2Rad;

			float sr, cr, sp, cp, sy, cy;

			var halfRoll = v.Z * 0.5f;
			sr = (float)Math.Sin(halfRoll);
			cr = (float)Math.Cos(halfRoll);

			var halfPitch = v.X * 0.5f;
			sp = (float)Math.Sin(halfPitch);
			cp = (float)Math.Cos(halfPitch);

			var halfYaw = v.Y * 0.5f;
			sy = (float)Math.Sin(halfYaw);
			cy = (float)Math.Cos(halfYaw);

			return new Quaternion(
				cy * sp * cr + sy * cp * sr,
				sy * cp * cr - cy * sp * sr,
				cy * cp * sr - sy * sp * cr,
				cy * cp * cr + sy * sp * sr
			);
		}

		public static Quaternion Divide(this Quaternion q1, Quaternion q2)
		{
			Quaternion ans = new Quaternion();

			var q1x = q1.X;
			var q1y = q1.Y;
			var q1z = q1.Z;
			var q1w = q1.W;

			var ls = q2.X * q2.X + q2.Y * q2.Y + q2.Z * q2.Z + q2.W * q2.W;
			var invNorm = 2.0f / ls;

			var q2x = -q2.X * invNorm;
			var q2y = -q2.Y * invNorm;
			var q2z = -q2.Z * invNorm;
			var q2w = -q2.W * invNorm;

			var cx = q1y * q2z - q1z * q2y;
			var cy = q1z * q2x - q1x * q2z;
			var cz = q1x * q2y - q1y * q2x;

			var dot = q1x * q2x + q1y * q2y + q1z * q2z;

			ans.X = q1x * q2w + q2x * q1w + cx;
			ans.Y = q1y * q2w + q2y * q1w + cy;
			ans.Z = q1z * q2w + q2z * q1w + cz;
			ans.W = q1w * q2w - dot;

			return ans;
		}

		public static Vector3D GetForwardVector(this Quaternion q)
		{
			return new Vector3D(
				2 * (q.X * q.Z + q.W * q.Y),
				2 * (q.Y * q.Z - q.W * q.X),
				1 - 2 * (q.X * q.X + q.Y * q.Y)
			);
		}

		public static Vector3D GetUpVector(this Quaternion q)
		{
			return new Vector3D(
				2 * (q.X * q.Y - q.W * q.Z),
				1 - 2 * (q.X * q.X + q.Z + q.Z),
				2 * (q.Y * q.Z + q.W * q.X)
			);
		}

		public static Vector3D GetRightVector(this Quaternion q)
		{
			return new Vector3D(
				1 - 2 * (q.Y * q.Y + q.Z + q.Z),
				2 * (q.X * q.Y + q.W * q.Z),
				2 * (q.X * q.Z - q.W * q.Y)
			);
		}

		/// <summary>
		/// Converts quaternion to euler angles.
		/// </summary>
		/// <param name="q">Quaternion to convert.</param>
		/// <returns>Vector3D as euler angles.</returns>
		public static Vector3D ToEulerAngles(this Quaternion q)
		{
			var sqw = q.W * q.W;
			var sqx = q.X * q.X;
			var sqy = q.Y * q.Y;
			var sqz = q.Z * q.Z;
			var unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
			var test = q.X * q.Y + q.Z * q.W;
			var v = new Vector3D();

			if (test > 0.499 * unit)
			{
				// singularity at north pole
				v.Y = 2 * Math.Atan2(q.X, q.W);
				v.X = Math.PI / 2;
				v.Z = 0;
				return v * Rad2Deg;
			}
			if (test < -0.499 * unit)
			{
				// singularity at south pole
				v.Y = -2 * Math.Atan2(q.X, q.W);
				v.X = -Math.PI / 2;
				v.Z = 0;
				return v * Rad2Deg;
			}
			v.Y = Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);
			v.X = Math.Asin(2 * test / unit);
			v.Z = Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);

			return v * Rad2Deg;
		}
	}
}
