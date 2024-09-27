using UnityEngine;

namespace HeroFighter.Runtime
{
    [CreateAssetMenu(menuName = Constants.ProjectName + "/" + FileName, fileName = FileName)]
    public class SceneConfiguration : ScriptableObject
    {
        private const string FileName = nameof(SceneConfiguration);
        public int heroSelectionSceneIndex = 0;
        public int battleSceneIndex = 1;
    }
}