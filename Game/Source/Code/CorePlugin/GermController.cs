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
	public class GermController : Component, ICmpUpdatable, ICmpCollisionListener, ICmpInitializable
	{
		public static readonly float MinAttackEnergy = 0.1f;

		private Vector2 targetMovement = Vector2.Zero;
		private float moveSpeed = 5.0f;
		private float energy = 0.0f;
		private float energyChargeRate = 1.0f;
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
		public float Energy
		{
			get { return this.energy; }
			set { this.energy = value; }
		}
		public float EnergyChargeRate
		{
			get { return this.energyChargeRate; }
			set { this.energyChargeRate = value; }
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

			this.energy += Time.TimeMult * 0.001f * this.energyChargeRate;

			float targetSpeed = MathF.Min(this.targetMovement.Length, 1.0f);
			Vector2 targetDir = (this.targetMovement / MathF.Max(targetSpeed, 0.001f));

			Vector2 targetVel = targetDir * targetSpeed * this.moveSpeed;
			Vector2 actualVel = body.LinearVelocity;
			Vector2 velDiff = targetVel - actualVel;

			body.ApplyWorldForce(velDiff * body.Mass * 0.01f);

			Vector2 targetDisplayedMoveDir = targetDir * (this.targetMovement.Length / (0.35f + this.targetMovement.Length));
			blobRenderer.DisplayedMoveDir += (targetDisplayedMoveDir - blobRenderer.DisplayedMoveDir) * 0.05f * Time.TimeMult;
			blobRenderer.DisplayedEnergyLevel += (this.energy - blobRenderer.DisplayedEnergyLevel) * 0.1f * Time.TimeMult;

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
		void ICmpCollisionListener.OnCollisionBegin(Component sender, CollisionEventArgs args) { }
		void ICmpCollisionListener.OnCollisionEnd(Component sender, CollisionEventArgs args) { }
		void ICmpCollisionListener.OnCollisionSolve(Component sender, CollisionEventArgs args)
		{
			GermController otherGerm = args.CollideWith.GetComponent<GermController>();
			if (otherGerm == null) return;

			float colorTransfer = (args.CollisionData.NormalImpulse - 50.0f) / 100.0f;
			if (colorTransfer > 0.0f)
			{
				if (this.color != otherGerm.color)
				{
					float remainingEnergy = this.energy - otherGerm.energy;
					if (MathF.Abs(remainingEnergy) >= MinAttackEnergy)
					{
						if (remainingEnergy > 0.0f)
							otherGerm.color = this.color;
						else
							this.color = otherGerm.color;

						this.energy = MathF.Abs(remainingEnergy) * 0.5f;
						this.energyChargeRate = MathF.Rnd.NextFloat(0.05f, 1.0f);
						otherGerm.energy = MathF.Abs(remainingEnergy) * 0.5f;
						otherGerm.energyChargeRate = MathF.Rnd.NextFloat(0.05f, 1.0f);
					}
				}
				else
				{
					float addedEnergy = this.energy + otherGerm.energy;

					this.energy = addedEnergy * 0.5f;
					otherGerm.energy = addedEnergy * 0.5f;
				}
			}
		}
		void ICmpInitializable.OnInit(InitContext context)
		{
			if (context == InitContext.Activate)
			{
				if (this.color != ColorRgba.White)
				{
					this.energyChargeRate = MathF.Rnd.NextFloat(0.05f, 1.0f);
				}
			}
		}
		void ICmpInitializable.OnShutdown(ShutdownContext context) { }
	}
}
