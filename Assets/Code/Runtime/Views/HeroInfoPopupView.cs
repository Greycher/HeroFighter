using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace HeroFighter.Runtime.Views
{
    public class HeroInfoPopupView : MonoBehaviour
    {
        [SerializeField] private RectTransform pivotTransform;
        [SerializeField] private TMP_Text heroNameLabel;
        [SerializeField] private string heroNameLabelFormat = "Name: {0}";
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private string levelLabelFormat = "Level: {0}";
        [SerializeField] private TMP_Text attackPowerLabel;
        [SerializeField] private string attackPowerLabelFormat = "Attack Power: {0}";
        [SerializeField] private TMP_Text experienceLabel;
        [SerializeField] private string experienceLabelFormat = "Experience: {0}/{1}";
        [SerializeField] private Button closeButton;

        public UnityEvent<HeroInfoPopupView> OnCloseBtnClicked = new();
        
        public void UpdateView(Vector2 screenPos, string heroName, int level, int attackPower, int experience, int experienceLimit)
        {
            pivotTransform.anchoredPosition = screenPos;
            heroNameLabel.text = String.Format(heroNameLabelFormat, heroName);
            levelLabel.text = String.Format(levelLabelFormat, level);
            attackPowerLabel.text = String.Format(attackPowerLabelFormat, attackPower);
            experienceLabel.text = String.Format(experienceLabelFormat, experience, experienceLimit);
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void OnCloseClicked()
        {
            OnCloseBtnClicked.Invoke(this);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        private void OnDestroy()
        {
            OnCloseBtnClicked.RemoveAllListeners();
        }
    }
}