using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ControllerInputManager : InputManager {
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void CheckInputs() {
        playerIndex = PlayerIndex.One;
        prevState = state;
        state = GamePad.GetState(playerIndex);
        
        Vector3 oldDirection = Direction;
        Direction = Vector3.zero;

        if (state.Buttons.Y == ButtonState.Pressed) {
            Direction += Vector3.up;
        }
        if (state.Buttons.A == ButtonState.Pressed) {
            Direction += Vector3.down;
        }
        if (state.Triggers.Right > 0) {
            Direction += Vector3.forward;
        }
        if (state.Triggers.Left > 0) {
            Direction += Vector3.back;
        }
        if (state.ThumbSticks.Left.X < 0) {
            Direction += Vector3.left;
        }
        if (state.ThumbSticks.Left.X > 0) {
            Direction += Vector3.right;
        }

        if (oldDirection != Direction) {
            Debug.Log(Direction);
        }
    }
}
