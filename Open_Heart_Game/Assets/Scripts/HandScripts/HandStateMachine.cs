using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// State machine that tracks that hand's state
/// used by the animator controller
/// </summary>
public class HandStateMachine  {

    private Dictionary<StateTransition, HandState> transitions; // the different transitions the state machine can take
    private Dictionary<HandState, bool> handStateActive; // whether the state is active
    public HandState CurrentState { get; private set; }

    public enum HandState
    {
        Idle,
        Pinch,
        Clamp,
        Scissors,
        Forceps,
        Scalpel,
        SmallClamp,
        RetrieveSmallClamp
    }


    public enum Command
    {
        Next
    }


    // constructor
    public HandStateMachine()
    {
        CurrentState = HandState.Idle;
        transitions = new Dictionary<StateTransition, HandState>
        {
            { new StateTransition(HandState.Idle, Command.Next), HandState.Pinch },
            { new StateTransition(HandState.Pinch, Command.Next), HandState.Clamp },
            { new StateTransition(HandState.Clamp, Command.Next), HandState.Scissors },
            { new StateTransition(HandState.Scissors, Command.Next), HandState.Forceps },
            { new StateTransition(HandState.Forceps, Command.Next), HandState.Scalpel },
            { new StateTransition(HandState.Scalpel, Command.Next), HandState.SmallClamp },
            {new StateTransition(HandState.SmallClamp, Command.Next),HandState.RetrieveSmallClamp},
            {new StateTransition(HandState.RetrieveSmallClamp, Command.Next),HandState.Idle},
        };
        handStateActive = new Dictionary<HandState, bool>
        {
            {HandState.Idle, true},
            {HandState.Pinch, true},
            {HandState.Clamp, true},
            {HandState.Scissors, true},
            {HandState.Forceps, true},
            {HandState.Scalpel, true},
            {HandState.SmallClamp, true},
            {HandState.RetrieveSmallClamp, true},
        };

    }

    public HandState GetNext(Command command)
    {
        HandState nextState = CurrentState;
        bool nextStateActive;

        do
        {
            StateTransition transition = new StateTransition(nextState, command);
            if (!transitions.TryGetValue(transition, out nextState))
            {
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            }
            if (!handStateActive.TryGetValue(nextState, out nextStateActive))                    // check if our next state is active
            {
                throw new Exception("Invalid state: " + nextState);
            }
        } while (!nextStateActive && nextState != HandState.Idle); // to avoid possible infinite loop, idle should always be active

        return nextState;
    }

    public HandState MoveNext(Command command)
    {
        CurrentState = GetNext(command);
        return CurrentState;
    }

    public HandState MoveToIdle()
    {
        while ((CurrentState = GetNext(Command.Next)) != HandState.Idle) ;
        return CurrentState;
    }



    // helper class
    private class StateTransition
    {
        readonly HandState CurrentState;
        readonly Command Command;

        public StateTransition(HandState currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

 

        

    



}
