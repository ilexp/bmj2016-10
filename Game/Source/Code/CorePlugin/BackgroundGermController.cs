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
	[RequiredComponent(typeof(GermBlobRenderer))]
	public class BackgroundGermController : Component, ICmpUpdatable, ICmpEditorUpdatable
	{
		private Vector2 currentVelocity;
		private Vector2 targetMovement;
		private float moveSpeed;
		
		public float MoveSpeed
		{
			get { return this.moveSpeed; }
			set { this.moveSpeed = value; }
		}


		private void UpdateColor()
		{
			GermBlobRenderer blobRenderer = this.GameObj.GetComponent<GermBlobRenderer>();
			float awayFromZeroPlane = MathF.Abs(this.GameObj.Transform.Pos.Z) / 1000.0f;
			blobRenderer.Tint = new ColorRgba(0.6f / (1.0f + awayFromZeroPlane));
		}
		void ICmpUpdatable.OnUpdate()
		{
			GermBlobRenderer blobRenderer = this.GameObj.GetComponent<GermBlobRenderer>();

			this.targetMovement += MathF.Rnd.NextVector2(0.01f) * Time.TimeMult;

			float targetSpeed = MathF.Min(this.targetMovement.Length, 1.0f);
			Vector2 targetDir = (this.targetMovement / MathF.Max(targetSpeed, 0.001f));
			Vector2 targetVel = targetDir * targetSpeed * this.moveSpeed;

			this.currentVelocity += (targetVel - this.currentVelocity) * 0.1f * Time.TimeMult;
			this.GameObj.Transform.MoveBy(this.currentVelocity * Time.TimeMult);

			Vector2 targetDisplayedMoveDir = targetDir * (this.targetMovement.Length / (0.35f + this.targetMovement.Length));
			blobRenderer.DisplayedMoveDir += (targetDisplayedMoveDir - blobRenderer.DisplayedMoveDir) * 0.05f * Time.TimeMult;

			this.UpdateColor();
		}
		void ICmpEditorUpdatable.OnUpdate()
		{
			this.UpdateColor();
		}
	}
}
