using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]
public class MasterManager : SingletonScriptableObject<MasterManager>
{
    [SerializeField] private GameSettings _gameSettings;

    [SerializeField] private DebugConsole _debugConsole;
    public static GameSettings GameSettings { get { return Instance._gameSettings; } }
    
    public static DebugConsole DebugConsole { get { return Instance._debugConsole; } }
};
