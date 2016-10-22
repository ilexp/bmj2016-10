using System;
using System.Collections.Generic;
using System.Linq;

using Duality;
using Duality.Components;
using Duality.Resources;
using Duality.Drawing;

namespace Game
{
	[RequiredComponent(typeof(GermController))]
	public class GermStatsRenderer : Renderer
	{
		public override float BoundRadius
		{
			get { return 300 * this.GameObj.Transform.Scale; }
		}


		public override void Draw(IDrawDevice device)
		{
			Canvas canvas = new Canvas(device);
			GermController germ = this.GameObj.GetComponent<GermController>();
			Vector3 worldPos = this.GameObj.Transform.Pos;

			canvas.State.ZOffset = -1.0f;
			canvas.State.SetMaterial(new BatchInfo(DrawTechnique.Alpha, ColorRgba.White));

			canvas.State.ColorTint = ColorRgba.White.WithAlpha(0.25f);
			canvas.FillCircle(
				worldPos.X, worldPos.Y, worldPos.Z, 
				3.0f + 37.0f * germ.Energy / (1.0f + germ.Energy));

			canvas.State.ColorTint = ColorRgba.White.WithAlpha(0.25f);
			int chargeIndicators = MathF.RoundToInt(germ.EnergyChargeRate / 0.25f);
			Vector2 offset = new Vector2(-12, 35);
			for (int i = 0; i < chargeIndicators; i++)
			{
				canvas.FillCircle(worldPos.X + offset.X, worldPos.Y + offset.Y, worldPos.Z, 3);
				offset.X += 8;
			}
		}
	}
}
