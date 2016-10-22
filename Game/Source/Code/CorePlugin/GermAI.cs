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
		void ICmpUpdatable.OnUpdate()
		{
			GermController controller = this.GameObj.GetComponent<GermController>();
			controller.TargetMovement += MathF.Rnd.NextVector2(0.01f) * Time.TimeMult;
		}
	}
}
