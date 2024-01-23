/*****************************************************************************
// File Name :         ObjectStats.cs
// Author :            Cade R. Naylor
// Creation Date :     January 23, 2024
//
// Brief Description : Creates a serializable class that stores and adjusts 
                        stats for objects

*****************************************************************************/
using UnityEngine;

[System.Serializable]
public class ObjectStats 
{
    [SerializeField]
    private float _dmgDealt;
    [SerializeField]
    private float _dmgCap;
    public float DamageDealt { get => _dmgDealt; }

}
