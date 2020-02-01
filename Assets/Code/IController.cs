using System;

namespace Code
{
	public interface IController
	{
		void TakeTurn(Action finishedTurn);
		void SetControlledEntity(Entity entity);
		void AddTargetEntity(Entity target);
	}
}