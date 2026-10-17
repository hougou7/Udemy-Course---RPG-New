using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationEvents : MonoBehaviour
{
    public void DamageEnemies()
    {
        Player player = GetComponentInParent<Player>();
        if (player != null)
            player.DamageEnemies();
    }
}
