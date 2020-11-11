using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDum : Block
{
	public int ParticlesCount = 10;

	public static void OnGrab(int count)
	{
	}

	protected override void OnTouch()
	{
		for (int i = 0; i < ParticlesCount; ++i)
		{
			ParticleSystem.EmitParams particle = new ParticleSystem.EmitParams();
			particle.position = transform.position + new Vector3((Random.value - 0.5f) * transform.parent.localScale.x, (Random.value - 0.5f) * transform.parent.localScale.y, -Settings.RacketHeight);
			particle.startSize = 0.5f;

			_type.Particles.Emit(particle, 10);
		}
	}
}
