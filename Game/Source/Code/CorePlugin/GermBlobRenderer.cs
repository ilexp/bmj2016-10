using System;
using System.Collections.Generic;
using System.Linq;

using Duality;
using Duality.Components;
using Duality.Resources;
using Duality.Drawing;

namespace Game
{
	public class GermBlobRenderer : Renderer
	{
		private ContentRef<Material> sharedMat = Material.DualityIcon;
		private ColorRgba color = ColorRgba.White;
		private float radius = 50.0f;
		private float depthOffset = 0.0f;
		private Vector2 displayedMoveDir = Vector2.Zero;

		[DontSerialize] private VertexC1P3T2[] vertices = null;
		[DontSerialize] private BatchInfo materialInstance = null;


		public ContentRef<Material> SharedMaterial
		{
			get { return this.sharedMat; }
			set { this.sharedMat = value; this.materialInstance = null; }
		}
		public ColorRgba Color
		{
			get { return this.color; }
			set { this.color = value; }
		}
		public float Radius
		{
			get { return this.radius; }
			set { this.radius = value; }
		}
		public float DepthOffset
		{
			get { return this.depthOffset; }
			set { this.depthOffset = value; }
		}
		public Vector2 DisplayedMoveDir
		{
			get { return this.displayedMoveDir; }
			set { this.displayedMoveDir = value; }
		}
		public override float BoundRadius
		{
			get { return this.radius * MathF.Sqrt(2) * this.GameObj.Transform.Scale; }
		}


		public override void Draw(IDrawDevice device)
		{
			Texture mainTex = (this.sharedMat.IsAvailable ? this.sharedMat.Res.MainTexture : null).Res ?? Texture.Checkerboard.Res;
			ColorRgba mainClr = (this.sharedMat.IsAvailable ? this.sharedMat.Res.MainColor : ColorRgba.White);

			Rect uvRect;
			if (mainTex != null)
				uvRect = new Rect(mainTex.UVRatio.X, mainTex.UVRatio.Y);
			else
				uvRect = new Rect(1.0f, 1.0f);

			if (this.materialInstance == null)
				this.materialInstance = new BatchInfo(this.sharedMat.Res ?? Material.Checkerboard.Res);

			this.PrepareVertices(ref this.vertices, device, mainClr, uvRect);

			this.materialInstance.SetUniform("MoveDir", this.displayedMoveDir.X, this.displayedMoveDir.Y);
			this.materialInstance.SetUniform("TimeOffset", this.GameObj.Id.GetHashCode() % 10000);
			device.AddVertices(this.materialInstance, VertexMode.Quads, this.vertices);
		}
		private void PrepareVertices(ref VertexC1P3T2[] vertices, IDrawDevice device, ColorRgba mainClr, Rect uvRect)
		{
			Vector3 posTemp = this.GameObj.Transform.Pos;
			float scaleTemp = 1.0f;
			device.PreprocessCoords(ref posTemp, ref scaleTemp);

			Vector2 xDot, yDot;
			MathF.GetTransformDotVec(this.GameObj.Transform.Angle, scaleTemp, out xDot, out yDot);

			float transformScale = this.GameObj.Transform.Scale;
			Rect rectTemp = new Rect(
				-this.radius * transformScale,
				-this.radius * transformScale,
				this.radius * 2.0f * transformScale,
				this.radius * 2.0f * transformScale);

			Vector2 edge1 = rectTemp.TopLeft;
			Vector2 edge2 = rectTemp.BottomLeft;
			Vector2 edge3 = rectTemp.BottomRight;
			Vector2 edge4 = rectTemp.TopRight;

			MathF.TransformDotVec(ref edge1, ref xDot, ref yDot);
			MathF.TransformDotVec(ref edge2, ref xDot, ref yDot);
			MathF.TransformDotVec(ref edge3, ref xDot, ref yDot);
			MathF.TransformDotVec(ref edge4, ref xDot, ref yDot);

			float left = uvRect.X;
			float right = uvRect.RightX;
			float top = uvRect.Y;
			float bottom = uvRect.BottomY;

			if (vertices == null || vertices.Length != 4) vertices = new VertexC1P3T2[4];

			vertices[0].Pos.X = posTemp.X + edge1.X;
			vertices[0].Pos.Y = posTemp.Y + edge1.Y;
			vertices[0].Pos.Z = posTemp.Z + this.depthOffset;
			vertices[0].TexCoord.X = left;
			vertices[0].TexCoord.Y = top;
			vertices[0].Color = mainClr * this.color;

			vertices[1].Pos.X = posTemp.X + edge2.X;
			vertices[1].Pos.Y = posTemp.Y + edge2.Y;
			vertices[1].Pos.Z = posTemp.Z + this.depthOffset;
			vertices[1].TexCoord.X = left;
			vertices[1].TexCoord.Y = bottom;
			vertices[1].Color = mainClr * this.color;

			vertices[2].Pos.X = posTemp.X + edge3.X;
			vertices[2].Pos.Y = posTemp.Y + edge3.Y;
			vertices[2].Pos.Z = posTemp.Z + this.depthOffset;
			vertices[2].TexCoord.X = right;
			vertices[2].TexCoord.Y = bottom;
			vertices[2].Color = mainClr * this.color;

			vertices[3].Pos.X = posTemp.X + edge4.X;
			vertices[3].Pos.Y = posTemp.Y + edge4.Y;
			vertices[3].Pos.Z = posTemp.Z + this.depthOffset;
			vertices[3].TexCoord.X = right;
			vertices[3].TexCoord.Y = top;
			vertices[3].Color = mainClr * this.color;
		}
	}
}
