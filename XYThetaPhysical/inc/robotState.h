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

private:
    RobotState()
        : LeftPosition{}, RightPosition{}, X{constants::StartingXDegrees}, Y{}, Theta{constants::StartingYDegrees}
    { }

public:
    RobotState(RobotState previousState, int leftPos, int rightPos)
    {
        LeftPosition = leftPos;
        RightPosition = rightPos;

        int deltaL = leftPos - previousState.LeftPosition;
        int deltaR = rightPos - previousState.RightPosition;

        float deltaTheta = (deltaL - deltaR) / constants::RobotDiameterDeg;
        Theta = previousState.Theta + deltaTheta; // fmod(previousState.Theta + deltaTheta, 2 * M_PI);

        float deltaCenter = (deltaL + deltaR) / 2.0f;
        float deltaX = (float)cos(Theta) * deltaCenter;
        float deltaY = (float)sin(Theta) * deltaCenter;

        X = previousState.X - deltaX;
        Y = previousState.Y - deltaY;
    }

    RobotState(float x, float y, float theta)
        : LeftPosition{}, RightPosition{}, X{x}, Y{y}, Theta{theta}
    { }
};