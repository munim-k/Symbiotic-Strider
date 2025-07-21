using UnityEngine;

public class ProjectileType : MonoBehaviour
{
    public Projectile_Type projectileType;
    public float procBuildUp = 0f;
    public enum Projectile_Type
    {
        Frost,
        Poison,
        Stun
    }
}