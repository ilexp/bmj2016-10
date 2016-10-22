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
	[RequiredComponent(typeof(GermController))]
	public class GermAI : Component, ICmpUpdatable
	{
		[DontSerialize] private float selectTargetTimer = 0.0f;
		[DontSerialize] private GermController attackTarget = null;

		void ICmpUpdatable.OnUpdate()
		{
			GermController controller = this.GameObj.GetComponent<GermController>();

			// Attack
			if (this.attackTarget != null)
			{
				Vector3 thisPos = this.GameObj.Transform.Pos;
				Vector3 otherPos = this.attackTarget.GameObj.Transform.Pos;
				Vector2 direction = (otherPos - thisPos).Xy.Normalized;
				controller.TargetMovement = direction;

				//VisualLog.Default.DrawConnection(thisPos.X, thisPos.Y, 0, otherPos.X, otherPos.Y);
			}
			// Wander
			else
			{
				controller.TargetMovement += MathF.Rnd.NextVector2(0.01f) * Time.TimeMult;

				// Avoid obstacles
				Vector2 moveDir = controller.TargetMovement.Normalized;
				controller.TargetMovement -= controller.TargetMovement * this.CheckObstacle(moveDir * 200);
			}

			this.selectTargetTimer -= Time.TimeMult * Time.SPFMult;
			if (this.selectTargetTimer <= 0.0f)
			{
				this.selectTargetTimer = MathF.Rnd.NextFloat(0.5f, 3.0f);
				if (MathF.Rnd.NextBool())
					this.SelectTarget();
				else
					this.attackTarget = null;
			}
		}

		private void SelectTarget()
		{
			GermController controller = this.GameObj.GetComponent<GermController>();
			if (controller.Color == ColorRgba.White) return;

			GermController[] germs = this.GameObj.ParentScene.FindComponents<GermController>().ToArray();

			float nearestDist = 600;
			GermController nearestGerm = null;
			foreach (GermController germ in germs)
			{
				if (germ.Energy >= controller.Energy) continue;
				if (germ.Color == controller.Color) continue;

				float distance = (germ.GameObj.Transform.Pos - this.GameObj.Transform.Pos).Length;
				if (distance < nearestDist)
				{
					nearestDist = distance;
					nearestGerm = germ;
				}
			}

			this.attackTarget = nearestGerm;
		}
		private float CheckObstacle(Vector2 ray)
		{
			Vector2 pos = this.GameObj.Transform.Pos.Xy;

			RayCastData firstHit;
			bool hitSomething = RigidBody.RayCast(
				pos, pos + ray, d =>
				{
					if (d.GameObj == this.GameObj) return -1.0f;
					return d.Fraction;
				}, 
				out firstHit);


			if (hitSomething)
			{
				//VisualLog.Default.DrawConnection(pos.X, pos.Y, 0, firstHit.Pos.X, firstHit.Pos.Y);
				return firstHit.Fraction;
			}
			else
			{
				//VisualLog.Default.DrawConnection(pos.X, pos.Y, 0, pos.X + ray.X, pos.Y + ray.Y);
				return 0.0f;
			}
		}
	}
}
