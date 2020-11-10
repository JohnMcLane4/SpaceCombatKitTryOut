using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoContainer : MonoBehaviour
{
    public int numAmmo;

    public void DrawAmmo()
    {
        numAmmo = Mathf.Max(numAmmo - 1, 0);
    }

    public bool HasAmmo()
    {
        return (numAmmo > 0);
    }
}
