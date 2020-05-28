using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{
    public string Name { get; private set; }
    protected Transform origin;

    public Weapon(Transform origin, string name)
    {
        this.Name = name;
        this.origin = origin;
    }

    public abstract void Shoot();
}

public class ForceGun : Weapon
{
    private float forcePower;

    private ForceGun(Transform origin, string name, float forcePower) : base(origin, name)
    {
        this.forcePower = forcePower;
    }

    public static ForceGun Grappler(Transform origin)
    {
        return new ForceGun(origin, "Grappler", -10f);
    }

    public static ForceGun PushGun(Transform origin)
    {
        return new ForceGun(origin, "PushGun", 10f);
    }

    public override void Shoot()
    {
        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, 100f))
        {
            Debug.Log("We hit: " + hit.transform.name);
            var rb = hit.transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                var direction = hit.transform.position - origin.position;

                direction.Normalize();

                rb.AddForceAtPosition(direction * forcePower, hit.point, ForceMode.Impulse);
            }
        }
    }
}

public class CannonGun : Weapon
{
    private GameObject cannonBall;

    private float initialForce = 100f;

    public CannonGun(Transform origin) : base(origin, "CannonGun")
    {
        cannonBall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cannonBall.transform.localScale *= 0.1f;
        var rb = cannonBall.AddComponent<Rigidbody>();
        rb.mass = 2f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        cannonBall.SetActive(false);
    }

    public override void Shoot()
    {
        var projectile = GameObject.Instantiate(cannonBall, origin.position, origin.rotation);
        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * initialForce, ForceMode.Impulse);
    }
}
