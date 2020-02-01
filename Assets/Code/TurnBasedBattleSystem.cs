using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

        private void Start()
        {
            var player = Instantiate(playerEntityPrefab);
            var enemy = Instantiate(enemyEntityPrefab);

            _entities.Add(player);
            _entities.Add(enemy);

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

        private void NextTurn()
        {
            if (++_controllerIndex >= _controllers.Count)
            {
                _controllerIndex = 0;
            }

            _controllers[_controllerIndex].TakeTurn(NextTurn);
        }
    }
}
