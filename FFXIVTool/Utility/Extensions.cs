using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace FFXIVTool.Utility
{
	public static class Extensions
	{
		private static double Deg2Rad = (Math.PI * 2) / 360;
		private static double Rad2Deg = 360 / (Math.PI * 2);

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

			var halfRoll = (float)v.Z * 0.5f;
			sr = (float)Math.Sin(halfRoll);
			cr = (float)Math.Cos(halfRoll);

			var halfPitch = (float)v.X * 0.5f;
			sp = (float)Math.Sin(halfPitch);
			cp = (float)Math.Cos(halfPitch);

			var halfYaw = (float)v.Y * 0.5f;
			sy = (float)Math.Sin(halfYaw);
			cy = (float)Math.Cos(halfYaw);

			return new Quaternion(
				cy * sp * cr + sy * cp * sr,
				sy * cp * cr - cy * sp * sr,
				cy * cp * sr - sy * sp * cr,
				cy * cp * cr + sy * sp * sr
			);
		}

		private static double NormalizeAngle(double angle)
		{
			while (angle > 360)
				angle -= 360;
			while (angle < 0)
				angle += 360;
			return angle;
		}

		private static Vector3D NormalizeAngles(Vector3D angles)
		{
			angles.X = NormalizeAngle(angles.X);
			angles.Y = NormalizeAngle(angles.Y);
			angles.Z = NormalizeAngle(angles.Z);
			return angles;
		}

		/// <summary>
		/// Converts quaternion to euler angles.
		/// </summary>
		/// <param name="q1">Quaternion to convert.</param>
		/// <returns>Vector3D as euler angles.</returns>
		public static Vector3D ToEulerAngles(this Quaternion q)
		{
			var v = new Vector3D();

			var sqw = q.W * q.W;
			var sqx = q.X * q.X;
			var sqy = q.Y * q.Y;
			var sqz = q.Z * q.Z;

			var unit = sqx + sqy + sqz + sqw;
			var test = q.X * q.Y + q.Z * q.W;

			if (test > 0.4999f * unit)
			{
				v.Y = 2f * (float)Math.Atan2(q.X, q.W);
				v.X = Math.PI * 0.5f;
				v.Z = 0f;
			}
			else if (test < -0.4999f * unit)
			{
				v.Y = -2f * (float)Math.Atan2(q.X, q.W);
				v.X = -Math.PI * 0.5f;
				v.Z = 0f;
			}
			else
			{
				v.Y = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, sqw - sqy - sqz + sqw);
				v.X = (float)Math.Asin(2f * test / unit);
				v.Z = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, -sqx + sqy - sqz + sqw);
			}

			v.X *= Rad2Deg;
			v.Y *= Rad2Deg;
			v.Z *= Rad2Deg;

			return v;
		}
	}
}
