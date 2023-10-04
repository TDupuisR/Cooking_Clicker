using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Recipes
{
    public enum States
    {
        WAIT, PREP, COOK, SERVE
    }
    public States currentState;

    public List<RessourceManager.RessourcesNames> ressourcesNeeded;
}
