using System;
using System.Collections;
using UnityEngine;

// TODO: abstract
public class Controller : MonoBehaviour
{
	protected Action FinishedTurn;
	protected float Health;

	public void TakeTurn(Action finishedTurn)
	{
		Debug.Log("I'm doing a turn");
		FinishedTurn = finishedTurn;
		StartTurn();
	}

	public void TakeHeal(float amount)
	{
		
	}

	protected virtual void StartTurn()
	{
		StartCoroutine(DoTurn());
	}

	private IEnumerator DoTurn()
	{
		var time = 3;
		while (time > 0)
		{
			yield return new WaitForSeconds(1f);
			time -= 1;
		}
		FinishedTurn();
	}
}
