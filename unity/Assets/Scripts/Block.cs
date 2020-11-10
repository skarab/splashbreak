using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(transform.parent.gameObject);
	}
}
