using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code
{
    public class TurnBasedBattleSystem : MonoBehaviour
    {
        [FormerlySerializedAs("playerControllerPrefab")]
        public Entity playerEntityPrefab;

        [FormerlySerializedAs("enemyControllerPrefab")]
        public Entity enemyEntityPrefab;

        public MenuController menuControllerPrefab;
        private MenuController _menuController;

        private readonly List<Entity> _entities = new List<Entity>();
        private int _entityIndex;
        private Entity _player;

        private void Start()
        {
            _player = Instantiate(playerEntityPrefab);
            var enemy = Instantiate(enemyEntityPrefab);

            _entities.Add(_player);
            _entities.Add(enemy);

            _menuController = Instantiate(menuControllerPrefab);

            for (var i = 0; i < _entities.Count; ++i)
            {
                // the loop i is just a pointer that is still retained afterward, so it will always be _entities.Count
                // copy the i into index here to avoid the reference and only capture the current loop's i.
                var index = i;
                _menuController.AddTargetButton(() => PlayerHealTarget(index), 
                    $"Repair {_entities[i].displayName}");
            }

            _entityIndex = 0;
            _entities[_entityIndex].TakeTurn(NextTurn);
        }

        private void PlayerHealTarget(int entityIndex)
        {
            _player.HealTarget(_entities[entityIndex]);
        }

        private void NextTurn()
        {
            if (++_entityIndex >= _entities.Count)
            {
                _entityIndex = 0;
            }

            _entities[_entityIndex].TakeTurn(NextTurn);
        }
    }
}
