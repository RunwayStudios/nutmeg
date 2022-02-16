using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects.Gameplay.Items;

public abstract class Item : MonoBehaviour
{
    [SerializeField] private ItemInformation itemInfo;
    
    public abstract void Use();
}
