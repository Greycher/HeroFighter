using UnityEngine;

namespace HeroFighter.Runtime.Views
{
    public class TurnIndicatorView : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField, AnimatorState] private int playerTurnAnimationState;
        [SerializeField, AnimatorState] private int enemyTurnAnimationState;
        
        public void OnTurnStarted(bool playerTurn)
        {
            animator.Play(playerTurn ? playerTurnAnimationState : enemyTurnAnimationState);
        }
    }
}