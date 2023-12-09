#include <chrono>
#include <ctime>
#include <thread>
#include <list>
#include <memory>
#include <iostream>

#include "Timer.h"
#include "Static.h"
#include "ev3dev.h"
#include "robotState.h"

int main()
{
    std::cout << "Press enter button to start" << std::endl;
    while (!ev3dev::button::enter.pressed()) { }

    ev3dev::medium_motor left{ev3dev::OUTPUT_B};
    ev3dev::medium_motor right{ev3dev::OUTPUT_C};
    left.set_position(0);
    right.set_position(0);
    left.set_polarity(ev3dev::medium_motor::polarity_inversed);

    int previousTime = Timer::GetInstance().ElapsedMilliseconds();

    RobotState previousState{RobotState::GetBaseState()};
    RobotState currentState = previousState;

    left.run_direct();
    right.run_direct();

    const int basePower = 60;
    const RobotState targetState{0, 100, 0};
    const int minPower = 50;
    const int minCorrectivePower = 5;

    const float kP = 20;

    while(true)
    {
        int currentTime = Timer::GetInstance().ElapsedMilliseconds();

        if(currentTime - previousTime < 50) continue;

        previousTime = currentTime;

        RobotState newState = {previousState, left.position(), right.position()};
        previousState = currentState;
        currentState = newState;

        int xError = targetState.X - currentState.X;
        float yError = targetState.Y - currentState.Y;
        
        float targetTheta = tanf(xError / yError);
        float thetaError = (targetTheta - currentState.Theta) * 57; // (57 = 180 / pi)

        int correctivePower = thetaError * kP;

        int leftPower = basePower + correctivePower;
        int rightPower = basePower - correctivePower;

        if(leftPower < 0)
        {
            if(leftPower < -100) leftPower = -100;
            if(rightPower < -100) rightPower = -100;
        }
        else
        {
            if(leftPower > 100) leftPower = 100;
            if(rightPower > 100) rightPower = 100;
        }

        left.set_duty_cycle_sp(leftPower);
        right.set_duty_cycle_sp(rightPower);
        
        // std::cout << (int)(correctivePower) << ", " << (int)(thetaError * 100) << "%" << std::endl;
        // std::cout << (int)(correctivePower) << ", " << (int)(thetaError * 100) << "%" << std::endl;
        std::cout << leftPower << ", " << rightPower << ", " << (int)(targetTheta * 180 / M_PI) << ", " << (int)(currentState.Theta * 180 / M_PI) << std::endl;
        //std::cout << "(" << (int)currentState.X << ", " << (int)currentState.Y << ")  theta: " << (int)(currentState.Theta * 180 / M_PI) << std::endl;
    }

    std::cout << "Press enter button to stop" << std::endl;
    while (!ev3dev::button::enter.pressed()) { }
}