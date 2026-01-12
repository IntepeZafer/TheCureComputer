using UnityEngine;

public class GameIntroManager : MonoBehaviour
{
    public Animator playerAnimator;
    public void PlayAnimation()
    {
        playerAnimator.SetTrigger("StartGame");
    }
}
