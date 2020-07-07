﻿using System.Collections;
using System.Collections.Generic;

namespace GameModels
{
    public class State 
    {
        private enum possibleStates{
            Idle,
            Moving,
            Attacking,
            Stunned,
            Diyng
        };

        private possibleStates currentState;

        public State() {
            currentState = possibleStates.Idle;
        }

        public bool IsIdle() { return currentState == possibleStates.Idle; }

        public void SetIdle() { currentState = possibleStates.Idle; }

        public bool IsMoving() { return currentState == possibleStates.Moving; }

        public void SetMoving() { currentState = possibleStates.Moving; }

        public bool IsAttacking() { return currentState == possibleStates.Attacking; }

        public void SetAttacking() { currentState = possibleStates.Attacking; }

        public bool IsStunned() { return currentState == possibleStates.Stunned; }

        public void SetStunned() { currentState = possibleStates.Stunned; }

        public bool IsDiyng() { return currentState == possibleStates.Diyng; }

        public void SetDiyng() { currentState = possibleStates.Diyng; }
    }
}