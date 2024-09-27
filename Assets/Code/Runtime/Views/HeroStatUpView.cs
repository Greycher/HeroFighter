using System;
using TMPro;
using UnityEngine;

namespace HeroFighter.Runtime.Views
{
    public class HeroStatUpView : MonoBehaviour
    {
        [SerializeField] private HeroView heroView;
        [SerializeField] private TMP_Text labelTemplate;
        [SerializeField] private string xpFormat;
        [SerializeField] private string lvlFormat;
        [SerializeField] private string hpFormat;
        [SerializeField] private string apFormat;

        private void Awake()
        {
            labelTemplate.gameObject.SetActive(false);
        }

        public void UpdateView(string heroName)
        {
            heroView.UpdateView(heroName);
        }

        public void AddExperienceIncrease(int from, int to)
        {
            GetNewLabel().text = string.Format(xpFormat, from, to);
        }
        
        public void AddLevelIncrease(int from, int to)
        {
            GetNewLabel().text = string.Format(lvlFormat, from, to);
        }
        
        public void AddHealthIncrease(int from, int to)
        {
            GetNewLabel().text = string.Format(hpFormat, from, to);
        }
        
        public void AddAttackPowerIncrease(int from, int to)
        {
            GetNewLabel().text = string.Format(apFormat, from, to);
        }

        private TMP_Text GetNewLabel()
        {
            var label = Instantiate(labelTemplate, labelTemplate.transform.parent);
            label.gameObject.SetActive(true);
            return label;
        }
    }
}