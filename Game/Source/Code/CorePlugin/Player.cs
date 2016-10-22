using System;
using System.Collections.Generic;
using System.Linq;

using Duality;
using Duality.Components;
using Duality.Components.Physics;
using Duality.Resources;
using Duality.Drawing;

namespace Game
{
	public class Player : Component, ICmpUpdatable
	{
		private CameraController camera;
		private GermController germ;

		public CameraController Camera
		{
			get { return this.camera; }
			set { this.camera = value; }
		}
		public GermController Germ
		{
			get { return this.germ; }
			set { this.germ = value; }
		}

		void ICmpUpdatable.OnUpdate()
		{
			Camera camera = this.GameObj.ParentScene.FindComponent<Camera>();
			Vector3 mouseWorldPos = camera.GetSpaceCoord(DualityApp.Mouse.Pos);
			Vector3 germPos = this.germ.GameObj.Transform.Pos;

			this.germ.TargetMovement = (mouseWorldPos.Xy - germPos.Xy) / 400;

			float targetCamZoom = 2.0f - MathF.Clamp(this.germ.TargetMovement.Length, 0.0f, 1.0f);
			if (this.camera.TargetZoom < targetCamZoom)
				this.camera.TargetZoom += (targetCamZoom - this.camera.TargetZoom) * 0.01f * Time.TimeMult;
			else
				this.camera.TargetZoom = targetCamZoom;
		}
	}
}
