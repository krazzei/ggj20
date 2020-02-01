using System.Collections.Generic;
using UnityEngine;

namespace Code
{
	public class TurnBasedBattleSystem : MonoBehaviour
	{
		public Entity playerEntityPrefab;

		public Entity enemyEntityPrefab;

		public MenuController menuControllerPrefab;
		private MenuController _menuController;

		public AiController aiControllerPrefab;
		private AiController _aiController;

		private readonly List<Entity> _entities = new List<Entity>();
		private readonly List<IController> _controllers = new List<IController>();
		private int _controllerIndex;
		private bool _stopBattle = false;

		// This can evolve into a next level, or maybe that will be scene based and we configure enemies and such
		private void Start()
		{
			var player = Instantiate(playerEntityPrefab);
			var enemy = Instantiate(enemyEntityPrefab);

			_entities.Add(player);
			_entities.Add(enemy);

			enemy.OnDeath += GameOver;
			enemy.OnFullHealth += Win;

			player.OnDeath += GameOver;

			_menuController = Instantiate(menuControllerPrefab);
			_menuController.SetControlledEntity(player);

			_aiController = Instantiate(aiControllerPrefab);
			_aiController.SetControlledEntity(enemy);

			foreach (var entity in _entities)
			{
				_menuController.AddTargetEntity(entity);
				_aiController.AddTargetEntity(entity);
			}

			_controllerIndex = 0;
			_controllers.Add(_menuController);
			_controllers.Add(_aiController);
			_controllers[_controllerIndex].TakeTurn(NextTurn);
		}

		private void GameOver()
		{
			Debug.Log("Game Over :(");
			_stopBattle = true;
		}

		private void Win()
		{
			Debug.Log("You win!");
			_stopBattle = true;
		}

		private void NextTurn()
		{
			if (_stopBattle)
			{
				return;
			}
			
			if (++_controllerIndex >= _controllers.Count)
			{
				_controllerIndex = 0;
			}

			_controllers[_controllerIndex].TakeTurn(NextTurn);
		}
	}
}