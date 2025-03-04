#pragma once
#include <stdint.h>
#include <math.h>

#include "robotState.h"

namespace constants
{
    const float DegreesPerCm = 13.3275007291f;

    const std::uint8_t StartingXCm = 23;
    const std::uint8_t StartingYCm = 95;
    const std::uint16_t StartingXDegrees = StartingXCm * DegreesPerCm;
    const std::uint16_t StartingYDegrees = StartingYCm * DegreesPerCm;
    const float StartingTheta = 0; //M_PI / 2.0f;

    const float RobotRadiusDeg = 127.21875f; // in degrees
    const float RobotDiameterDeg = 254.4375f; // in degrees
    
    const float FieldLengthCm = 236.2f;
    const float FieldHeightCm = 114.3f;

    const float M_2PI = 2 * M_PI;
}