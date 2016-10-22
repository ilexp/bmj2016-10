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
	[RequiredComponent(typeof(RigidBody))]
	[RequiredComponent(typeof(GermBlobRenderer))]
	public class GermController : Component, ICmpUpdatable
	{
		private Vector2 targetMovement = Vector2.Zero;
		private float moveSpeed = 5.0f;
		private ColorRgba color = ColorRgba.White;

		public Vector2 TargetMovement
		{
			get { return this.targetMovement; }
			set { this.targetMovement = value; }
		}
		public float MoveSpeed
		{
			get { return this.moveSpeed; }
			set { this.moveSpeed = value; }
		}
		public ColorRgba Color
		{
			get { return this.color; }
			set { this.color = value; }
		}

		void ICmpUpdatable.OnUpdate()
		{
			RigidBody body = this.GameObj.GetComponent<RigidBody>();
			GermBlobRenderer blobRenderer = this.GameObj.GetComponent<GermBlobRenderer>();

			float targetSpeed = MathF.Min(this.targetMovement.Length, 1.0f);
			Vector2 targetDir = (this.targetMovement / MathF.Max(targetSpeed, 0.001f));

			Vector2 targetVel = targetDir * targetSpeed * this.moveSpeed;
			Vector2 actualVel = body.LinearVelocity;
			Vector2 velDiff = targetVel - actualVel;

			body.ApplyWorldForce(velDiff * body.Mass * 0.01f);

			Vector2 targetDisplayedMoveDir = targetDir * (this.targetMovement.Length / (0.35f + this.targetMovement.Length));
			blobRenderer.DisplayedMoveDir += (targetDisplayedMoveDir - blobRenderer.DisplayedMoveDir) * 0.05f * Time.TimeMult;

			// Perform a color shift
			if (blobRenderer.SecondColor != blobRenderer.FirstColor)
			{
				blobRenderer.ColorShift = MathF.Clamp(blobRenderer.ColorShift + Time.TimeMult * 0.01f, 0.0f, 1.0f);
				if (blobRenderer.ColorShift == 1.0f)
				{
					blobRenderer.FirstColor = blobRenderer.SecondColor;
					blobRenderer.ColorShift = 0.0f;
				}
			}
			// If the actual color is different from the displayed one, do a color shift
			else if (blobRenderer.FirstColor != this.color)
			{
				blobRenderer.FirstColor = blobRenderer.SecondColor;
				blobRenderer.SecondColor = this.color;
				blobRenderer.ColorShift = 0.0f;
			}
		}
	}
}
