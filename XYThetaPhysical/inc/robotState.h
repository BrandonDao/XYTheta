#pragma once
#include "constants.h"

struct RobotState
{
    int LeftPosition, RightPosition;
    float X, Y, Theta;

    static const RobotState& GetBaseState()
    {
        static RobotState baseState{};
        return baseState;
    }

    RobotState()
        : LeftPosition{}, RightPosition{}, X{constants::StartingXDegrees}, Y{constants::StartingYDegrees}, Theta{constants::StartingTheta}
    { }

    RobotState(RobotState previousState, int leftPos, int rightPos)
    {
        LeftPosition = leftPos;
        RightPosition = rightPos;

        int deltaL = leftPos - previousState.LeftPosition;
        int deltaR = rightPos - previousState.RightPosition;

        float deltaTheta = (deltaL - deltaR) / constants::RobotDiameterDeg;
        Theta = previousState.Theta + deltaTheta; // fmod(previousState.Theta + deltaTheta, 2 * M_PI);

        float deltaCenter = (deltaL + deltaR) / 2.0f;
        float deltaX = (float)sin(Theta) * deltaCenter;
        float deltaY = (float)cos(Theta) * deltaCenter;

        X = previousState.X + deltaX;
        Y = previousState.Y + deltaY;
    }

    RobotState(float x, float y, float theta)
        : LeftPosition{}, RightPosition{}, X{x}, Y{y}, Theta{theta}
    { }
};