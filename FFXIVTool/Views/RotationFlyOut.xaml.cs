using FFXIVTool.Models;
using FFXIVTool.Utility;
using FFXIVTool.ViewModel;
using MahApps.Metro.Controls;
using System.Windows.Media.Media3D;

namespace FFXIVTool.Views
{
	/// <summary>
	/// Interaction logic for RotationFlyOut.xaml
	/// </summary>
	public partial class RotationFlyOut : Flyout
	{
		public CharacterDetails CharacterDetails { get => (CharacterDetails)BaseViewModel.model; set => BaseViewModel.model = value; }

		private readonly Vector3D AxisX = new Vector3D(1, 0, 0);
		private readonly Vector3D AxisY = new Vector3D(0, 1, 0);
		private readonly Vector3D AxisZ = new Vector3D(0, 0, 1);

		public RotationFlyOut()
		{
			InitializeComponent();
		}

		private void DecX_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisX, -XDelta.Value);
		private void DecY_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisY, -YDelta.Value);
		private void DecZ_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisZ, -ZDelta.Value);

		private void IncX_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisX, XDelta.Value);
		private void IncY_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisY, YDelta.Value);
		private void IncZ_Click(object sender, System.Windows.RoutedEventArgs e) => Rotate(AxisZ, ZDelta.Value);

		private void Rotate(Vector3D axis, double? deltaAngle)
		{
			// New rotation from input.
			var rotationDelta = new Quaternion(axis, deltaAngle ?? 0);

			// Current rotation.
			var currentRotation = new Quaternion(
				CharacterDetails.Rotation.value,
				CharacterDetails.Rotation2.value,
				CharacterDetails.Rotation3.value,
				CharacterDetails.Rotation4.value
			);

			// Resulting quaternion.
			var newRotation = (CharacterDetails.AltRotate) ? rotationDelta * currentRotation : currentRotation * rotationDelta;

			CharacterDetails.Rotation.value = (float)newRotation.X;
			CharacterDetails.Rotation2.value = (float)newRotation.Y;
			CharacterDetails.Rotation3.value = (float)newRotation.Z;
			CharacterDetails.Rotation4.value = (float)newRotation.W;

			var c = Settings.Instance.Character;

			MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, c.Body.Base, c.Body.Position.Rotation), CharacterDetails.Rotation.GetBytes());
			MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, c.Body.Base, c.Body.Position.Rotation2), CharacterDetails.Rotation2.GetBytes());
			MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, c.Body.Base, c.Body.Position.Rotation3), CharacterDetails.Rotation3.GetBytes());
			MemoryManager.Instance.MemLib.writeBytes(MemoryManager.GetAddressString(CharacterDetailsViewModel.baseAddr, c.Body.Base, c.Body.Position.Rotation4), CharacterDetails.Rotation4.GetBytes());
		}

		private void Rotation_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
		{
			if (Rotation.IsMouseOver || Rotation.IsKeyboardFocusWithin)
			{
				Rotation.ValueChanged -= Rotation_ValueChanged;
				Rotation.ValueChanged += Rotation_ValueChanged;
			}
		}

		private void Rotation2_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
		{
			if (Rotation2.IsMouseOver || Rotation2.IsKeyboardFocusWithin)
			{
				Rotation2.ValueChanged -= Rotation_ValueChanged;
				Rotation2.ValueChanged += Rotation_ValueChanged;
			}
		}

		private void Rotation3_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
		{
			if (Rotation3.IsMouseOver || Rotation3.IsKeyboardFocusWithin)
			{
				Rotation3.ValueChanged -= Rotation_ValueChanged;
				Rotation3.ValueChanged += Rotation_ValueChanged;
			}
		}

		private void Rotation4_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
		{
			if (Rotation4.IsMouseOver || Rotation4.IsKeyboardFocusWithin)
			{
				Rotation4.ValueChanged -= Rotation_ValueChanged;
				Rotation4.ValueChanged += Rotation_ValueChanged;
			}
		}

		private void Rotation_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double?> e)
		{
			var n = (NumericUpDown)sender;

			string addr;
			if (n.Name == Rotation.Name)
				addr = Settings.Instance.Character.Body.Position.Rotation;
			else if (n.Name == Rotation2.Name)
				addr = Settings.Instance.Character.Body.Position.Rotation2;
			else if (n.Name == Rotation3.Name)
				addr = Settings.Instance.Character.Body.Position.Rotation3;
			else if (n.Name == Rotation4.Name)
				addr = Settings.Instance.Character.Body.Position.Rotation4;
			else
				return;

			MemoryManager.Instance.MemLib.writeMemory(
				MemoryManager.GetAddressString(
					CharacterDetailsViewModel.baseAddr,
					Settings.Instance.Character.Body.Base,
					addr
				),
				"float",
				n.Value.ToString()
			);

			n.ValueChanged -= Rotation_ValueChanged;
		}
	}
}
