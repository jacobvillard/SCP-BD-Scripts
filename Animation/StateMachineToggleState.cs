using UnityEngine;

public class StateMachineToggleState : StateMachineBehaviour
{
    [SerializeField] private bool setBlendParam;
    [SerializeField] private string parameterName = "RandomisedIndex";
    [SerializeField] private float paramMin = 0f;
    [SerializeField] private float paramMax = 1f;
    
    [SerializeField] private bool setAnimTime;
    [SerializeField] private float animLength = 1.1f;
    [SerializeField] private string parameterAnim = "AnimationTimer";

    // Called when the animator enters this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (setBlendParam) {
            animator.SetFloat(parameterName, Mathf.Round(Random.Range(paramMin, paramMax)));
        }
        
        if (setAnimTime) {
            animator.SetFloat(parameterAnim,  animLength);
        }
    }

    

}
