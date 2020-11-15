using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

class Fluid : CustomPass
{
	private const int _blurStrength = 8;
	private const int _downscale = 1;

	public Material FluidMat;
	public Material RendererMat;
	public Light DirectionalLight;
	public Cubemap Sky;
	[Range(0, 1)] public float Roughness = 0.0f;

	private static class Textures
	{
		public static int DepthTex = Shader.PropertyToID("_DepthTex");
		public static int DiffuseTex = Shader.PropertyToID("_DiffuseTex");
		public static int NormalsTex = Shader.PropertyToID("_NormalsTex");
		public static int BlurTex = Shader.PropertyToID("_BlurTex");
		public static int GlobalDepthTex = Shader.PropertyToID("_GlobalDepthTex");
		public static int SkyTex = Shader.PropertyToID("_Sky");
	}

	private static class Properties
	{
		public static int LightDirection = Shader.PropertyToID("_LightDirection");
		public static int Roughness = Shader.PropertyToID("_Roughness");
	}

	private static class Passes
	{
		public static int ComputeNormals = 0;
		public static int Blur = 1;
		public static int Compositing = 2;
	}

	private static class Keywords
	{
		public static string Vertical = "VERTICAL";
	}

	protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
	{
	}

	protected override void Cleanup()
	{
	}

	protected override void Execute(ScriptableRenderContext context, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
	{
		Renderer[] scene_renderers = Object.FindObjectsOfType<Renderer>();
		Renderer[] renderers = System.Array.FindAll(scene_renderers, x => x.sharedMaterial != null && x.sharedMaterial.shader == RendererMat.shader);

		if (renderers.Length == 0)
		{
			return;
		}

		RTHandle sourceColor, sourceDepth;
		GetCameraBuffers(out sourceColor, out sourceDepth);

		int width = hdCamera.camera.pixelWidth;
		int height = hdCamera.camera.pixelHeight;
		Rect viewportBlur = new Rect(Vector2.zero, new Vector2(width / _downscale, height / _downscale));
		Rect viewport = new Rect(Vector2.zero, new Vector2(width, height));

		RenderTargetIdentifier depthRti = new RenderTargetIdentifier(Textures.DepthTex);
		RenderTargetIdentifier diffuseRti = new RenderTargetIdentifier(Textures.DiffuseTex);
		RenderTargetIdentifier normalsRti = new RenderTargetIdentifier(Textures.NormalsTex);
		RenderTargetIdentifier blurRti = new RenderTargetIdentifier(Textures.BlurTex);
		RenderTargetIdentifier globalDepthRti = new RenderTargetIdentifier(Textures.GlobalDepthTex);

		cmd.GetTemporaryRT(Textures.BlurTex, width / _downscale, height / _downscale, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);
		cmd.GetTemporaryRT(Textures.GlobalDepthTex, width, height, 24, FilterMode.Point, RenderTextureFormat.Depth);

		cmd.BeginSample("Draw Renderers");
		{
			cmd.GetTemporaryRT(Textures.DiffuseTex, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);
			cmd.GetTemporaryRT(Textures.DepthTex, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.RFloat);

			RenderTargetIdentifier[] rts = { depthRti, diffuseRti };
			cmd.SetRenderTarget(rts, globalDepthRti);
			cmd.ClearRenderTarget(true, true, Color.black);
			cmd.SetViewProjectionMatrices(hdCamera.camera.worldToCameraMatrix, hdCamera.camera.projectionMatrix);

			for (int i = 0; i < renderers.Length; ++i)
			{
				if (renderers[i] is ParticleSystemRenderer)
				{
					// DrawRenderer doesn't work, need to bake.
					ParticleSystemRenderer renderer = renderers[i] as ParticleSystemRenderer;
					Mesh mesh = new Mesh();
					renderer.BakeMesh(mesh, hdCamera.camera, true);
					cmd.DrawMesh(mesh, Matrix4x4.identity, renderer.sharedMaterial);
				}
				else
				{
					cmd.DrawRenderer(renderers[i], renderers[i].sharedMaterial);
				}
			}

			cmd.SetGlobalTexture(Textures.DepthTex, depthRti);
		}
		cmd.EndSample("Draw Renderers");

		cmd.BeginSample("Compute Normals");
		{
			cmd.GetTemporaryRT(Textures.NormalsTex, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);
			HDUtils.DrawFullScreen(cmd, viewport, FluidMat, normalsRti, null, Passes.ComputeNormals);

			cmd.BeginSample("Blur");
			{
				for (int i = 0; i < _blurStrength; ++i)
				{
					cmd.SetGlobalTexture(Textures.DiffuseTex, normalsRti);
					cmd.DisableShaderKeyword(Keywords.Vertical);
					HDUtils.DrawFullScreen(cmd, viewportBlur, FluidMat, blurRti, null, Passes.Blur);
					cmd.SetGlobalTexture(Textures.DiffuseTex, blurRti);
					cmd.EnableShaderKeyword(Keywords.Vertical);
					HDUtils.DrawFullScreen(cmd, viewport, FluidMat, normalsRti, null, Passes.Blur);
				}
			}
			cmd.EndSample("Blur");

			cmd.SetGlobalTexture(Textures.NormalsTex, normalsRti);
		}
		cmd.EndSample("Compute Normals");

		cmd.BeginSample("Blur Diffuse");
		{
			for (int i = 0; i < _blurStrength; ++i)
			{
				cmd.SetGlobalTexture(Textures.DiffuseTex, diffuseRti);
				cmd.DisableShaderKeyword(Keywords.Vertical);
				HDUtils.DrawFullScreen(cmd, viewportBlur, FluidMat, blurRti, null, Passes.Blur);
				cmd.SetGlobalTexture(Textures.DiffuseTex, blurRti);
				cmd.EnableShaderKeyword(Keywords.Vertical);
				HDUtils.DrawFullScreen(cmd, viewport, FluidMat, diffuseRti, null, Passes.Blur);
			}
		}

		cmd.BeginSample("Compositing");
		{
			MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
			matBlock.SetTexture(Textures.SkyTex, Sky);
			matBlock.SetVector(Properties.LightDirection, DirectionalLight.transform.forward);
			matBlock.SetFloat(Properties.Roughness, Roughness);

			cmd.SetGlobalTexture(Textures.DiffuseTex, new RenderTargetIdentifier(Textures.DiffuseTex));
			HDUtils.DrawFullScreen(cmd, FluidMat, sourceColor, sourceDepth, matBlock, Passes.Compositing);

			cmd.ReleaseTemporaryRT(Textures.DepthTex);
			cmd.ReleaseTemporaryRT(Textures.DiffuseTex);
			cmd.ReleaseTemporaryRT(Textures.NormalsTex);
			cmd.ReleaseTemporaryRT(Textures.BlurTex);
		}
		cmd.EndSample("Combine");
	}

}