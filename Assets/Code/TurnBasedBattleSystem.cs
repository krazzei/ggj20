using System.Collections.Generic;
using UnityEngine;

public class TurnBasedBattleSystem : MonoBehaviour
{
    public Controller playerControllerPrefab;
    public Controller enemyControllerPrefab;
    
    private readonly List<Controller> _controllers = new List<Controller>();
    private int _controllerIndex;
    
    private void Start()
    {
        var player = Instantiate(playerControllerPrefab);
        var enemy = Instantiate(enemyControllerPrefab);
        
        _controllers.Add(player);
        _controllers.Add(enemy);

        _controllerIndex = 0;
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
