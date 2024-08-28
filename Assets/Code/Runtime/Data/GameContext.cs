using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HeroFighter.Runtime
{
    [CreateAssetMenu(menuName = Constants.ProjectName + "/" + FileName, fileName = FileName)]
    public class GameContext : ScriptableObject
    {
        private const string FileName = nameof(GameContext);

        public HeroConfiguration heroConfiguration;
        public SceneConfiguration sceneConfiguration;

        private static GameContext _instance;

        public static GameContext Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = Load();
                }
                
                return _instance;
            }
        }
        
        private static GameContext Load()
        {
            var instance = Resources.Load<GameContext>(FileName);
#if UNITY_EDITOR

            if (!instance)
            {
                Debug.Log($"{nameof(GameContext)} was missing! Automatically created.");
                var gameContext = CreateInstance<GameContext>();
                gameContext.name = FileName;
                var directory = Path.Combine("Assets", nameof(Resources));
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh(); // Refresh the Asset Database to recognize the new directory
                }

                AssetDatabase.CreateAsset(gameContext, $"{Path.Combine(directory, FileName)}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = gameContext;
            }
#endif
            return instance;
        }
    }
}
