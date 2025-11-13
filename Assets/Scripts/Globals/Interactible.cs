using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    [SerializeField] protected string interactText = "Взаимодействовать";
    [SerializeField] protected bool displayActionButton = true;
    
    public string InteractText => interactText;
    public bool DisplayActionButton => displayActionButton;

    public virtual void Interact(GameObject interactor) { }
}
