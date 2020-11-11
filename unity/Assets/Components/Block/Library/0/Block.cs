using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	//public ParticleSystem Particles;

	private void OnCollisionEnter(Collision collision)
	{/*
		for (int i=0 ; i<10 ; ++i)
		{
			ParticleSystem.EmitParams particle = new ParticleSystem.EmitParams();
			particle.position = transform.position + new Vector3((Random.value - 0.5f) * transform.parent.localScale.x, (Random.value - 0.5f) * transform.parent.localScale.y, 0.0f);
			particle.startSize = 5.0f;

			Particles.Emit(particle, 10);
		}
		*/
		Destroy(transform.parent.gameObject);
	}
}
