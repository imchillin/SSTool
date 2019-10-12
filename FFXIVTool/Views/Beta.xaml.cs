using FFXIVTool.Models;
using FFXIVTool.Utility;
using FFXIVTool.ViewModel;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace FFXIVTool.Views
{
	/// <summary>
	/// Interaction logic for Beta.xaml
	/// </summary>
	public partial class Beta : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public CharacterDetails CharacterDetails { get => (CharacterDetails)BaseViewModel.model; set => BaseViewModel.model = value; }

		public Beta()
		{
			InitializeComponent();
			DataContext = this;

			var worker = new BackgroundWorker();

			worker.DoWork += (_, __) =>
			{
				while (true)
				{
					try
					{
						Dispatcher.Invoke(() =>
						{
							if (CharacterDetails == null)
								return;

							var q = new Quaternion(
								CharacterDetails.Rotation.value,
								CharacterDetails.Rotation2.value,
								CharacterDetails.Rotation3.value,
								CharacterDetails.Rotation4.value
							);

							var rUp = q.GetUpVector();
							var rRight = q.GetRightVector();
							var rForward = q.GetForwardVector();
							rUp.Normalize();
							rRight.Normalize();
							rForward.Normalize();

							var v = new Point3D(q.Axis.X, q.Axis.Y, q.Axis.Z);

							AddSegment(TestU, new Point3D(0, 0, 0), new Point3D(rUp.X, rUp.Y, rUp.Z), new Vector3D(0, 1, 0), true);
							AddSegment(TestR, new Point3D(0, 0, 0), new Point3D(rRight.X, rRight.Y, rRight.Z), new Vector3D(0, 1, 0), true);
							AddSegment(TestF, new Point3D(0, 0, 0), new Point3D(rForward.X, rForward.Y, rForward.Z), new Vector3D(0, 1, 0), true);
							AddSegment(TestV, new Point3D(0, 0, 0), v, new Vector3D(0, 1, 0), true);
						});

					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}

					Thread.Sleep(100);
				}
			};

			worker.RunWorkerAsync();
		}

		private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
		{
			// Create the points.
			int index1 = mesh.Positions.Count;
			mesh.Positions.Add(point1);
			mesh.Positions.Add(point2);
			mesh.Positions.Add(point3);

			// Create the triangle.
			mesh.TriangleIndices.Add(index1++);
			mesh.TriangleIndices.Add(index1++);
			mesh.TriangleIndices.Add(index1);
		}

		private Vector3D ScaleVector(Vector3D vector, double length)
		{
			double scale = length / vector.Length;
			return new Vector3D(
				vector.X * scale,
				vector.Y * scale,
				vector.Z * scale);
		}

		private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, bool extend)
		{
			const double thickness = 0.025;

			mesh.Positions = new Point3DCollection();

			// Get the segment's vector.
			Vector3D v = point2 - point1;

			if (extend)
			{
				// Increase the segment's length on both ends
				// by thickness / 2.
				Vector3D n = ScaleVector(v, thickness / 2.0);
				point1 -= n;
				point2 += n;
			}

			// Get the scaled up vector.
			Vector3D n1 = ScaleVector(up, thickness / 2.0);

			// Get another scaled perpendicular vector.
			Vector3D n2 = Vector3D.CrossProduct(v, n1);
			n2 = ScaleVector(n2, thickness / 2.0);

			// Make a skinny box.
			// p1pm means point1 PLUS n1 MINUS n2.
			Point3D p1pp = point1 + n1 + n2;
			Point3D p1mp = point1 - n1 + n2;
			Point3D p1pm = point1 + n1 - n2;
			Point3D p1mm = point1 - n1 - n2;
			Point3D p2pp = point2 + n1 + n2;
			Point3D p2mp = point2 - n1 + n2;
			Point3D p2pm = point2 + n1 - n2;
			Point3D p2mm = point2 - n1 - n2;

			// Sides.
			AddTriangle(mesh, p1pp, p1mp, p2mp);
			AddTriangle(mesh, p1pp, p2mp, p2pp);

			AddTriangle(mesh, p1pp, p2pp, p2pm);
			AddTriangle(mesh, p1pp, p2pm, p1pm);

			AddTriangle(mesh, p1pm, p2pm, p2mm);
			AddTriangle(mesh, p1pm, p2mm, p1mm);

			AddTriangle(mesh, p1mm, p2mm, p2mp);
			AddTriangle(mesh, p1mm, p2mp, p1mp);

			// Ends.
			AddTriangle(mesh, p1pp, p1pm, p1mm);
			AddTriangle(mesh, p1pp, p1mm, p1mp);

			AddTriangle(mesh, p2pp, p2mp, p2mm);
			AddTriangle(mesh, p2pp, p2mm, p2pm);
		}
	}

}
