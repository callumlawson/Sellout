﻿using Assets.Framework.States;
using UnityEngine;

namespace Assets.Scripts.States
{
    public class SelectedState : IState
    {
        public GameObject SelectedGameObject;

        public SelectedState(GameObject selectedGameObject)
        {
            SelectedGameObject = selectedGameObject;
        }
    }
}
