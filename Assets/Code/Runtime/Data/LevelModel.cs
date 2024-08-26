using UnityEngine;

namespace HeroFighter.Runtime
{
    public class LevelModel
    {
        private string PlayCountPref = "play_count";
        
        public int GetPlayCount()
        {
            return PlayerPrefs.GetInt(PlayCountPref, 0);
        }

        public void SetPlayCount(int playCount)
        {
            PlayerPrefs.SetInt(PlayCountPref, playCount);
        }
    }
}