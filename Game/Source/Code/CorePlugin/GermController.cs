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
		private Vector2 targetMovement;
		private float moveSpeed;
		
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

		void ICmpUpdatable.OnUpdate()
		{
			RigidBody body = this.GameObj.GetComponent<RigidBody>();
			GermBlobRenderer blobRenderer = this.GameObj.GetComponent<GermBlobRenderer>();

			float targetSpeed = MathF.Min(this.targetMovement.Length, 1.0f);
			Vector2 targetDir = (this.targetMovement / MathF.Max(targetSpeed, 0.001f));

			Vector2 targetVel = targetDir * targetSpeed * this.moveSpeed;
			Vector2 actualVel = body.LinearVelocity;
			Vector2 velDiff = targetVel - actualVel;

			body.ApplyWorldForce(velDiff * body.Mass);

			Vector2 targetDisplayedMoveDir = targetDir * (this.targetMovement.Length / (0.35f + this.targetMovement.Length));
			blobRenderer.DisplayedMoveDir += (targetDisplayedMoveDir - blobRenderer.DisplayedMoveDir) * 0.05f * Time.TimeMult;
		}
	}
}
