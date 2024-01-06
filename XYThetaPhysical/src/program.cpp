#include <chrono>
#include <ctime>
#include <fstream>
#include <iostream>
#include <list>
#include <memory>
#include <thread>

#include "Timer.h"
#include "Static.h"
#include "ev3dev.h"
#include "robotState.h"

float Normalize(const float& input)
{
    float output = std::fmod(input, constants::M_2PI);
    while(output < 0)
    {
        output += constants::M_2PI;
    }
    return output;
}

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
    std::vector<RobotState> targetStates{};

    // std::ifstream waypointsFileStream{"./WaypointLog.csv"};
    // while(!waypointsFileStream.is_open())
    // {
    //     std::cout << "File could not be opened!" << std::endl;
    // }

    // std::string xStr;
    // std::string yStr;
    // std::string thetaStr;
    // while(waypointsFileStream.good())
    // {
    //     std::getline(waypointsFileStream, xStr);
    //     std::getline(waypointsFileStream, yStr);
    //     std::getline(waypointsFileStream, thetaStr);

    //     // std::cout << xStr << ", " << yStr << ", " << thetaStr << std::endl;

    //     targetStates.emplace_back(std::stod(xStr), std::stod(yStr), std::stod(thetaStr));
    // }


    targetStates.emplace_back(0,1500,0);
    targetStates.emplace_back(1500,1500,0);
    targetStates.emplace_back(1500,3000,0);
    targetStates.emplace_back(3000,3000,0);
    
    const float kP = 20;

    for(auto targetState = std::cbegin(targetStates); targetState != std::cend(targetStates); )
    {
        int currentTime = Timer::GetInstance().ElapsedMilliseconds();

        if(currentTime - previousTime < 50) continue;

        previousTime = currentTime;

        RobotState newState = {previousState, left.position(), right.position()};
        previousState = currentState;
        currentState = newState;

        float xError = targetState->X - currentState.X;
        float yError = targetState->Y - currentState.Y;
        
        if(yError < 0)
        {
            if(yError > -0.01f)
            {
                yError = -0.01f;
            }
        }
        else
        {
            if(yError < 0.01f)
            {
                yError = 0.01f;
            }
        }
        
        float currentTheta = Normalize(currentState.Theta);
        float targetTheta = Normalize(std::atan2(xError, yError));
        
        float thetaError = targetTheta - currentTheta;
        float altThetaError;
        if(std::abs(targetTheta) > M_PI)
        {
            altThetaError = (targetTheta - constants::M_2PI) - currentTheta;
        }
        else
        {
            altThetaError = (targetTheta + constants::M_2PI) - currentTheta;
        }

        // std::cout << "error: " << (int)(thetaError * 180 / M_PI) << "  alt: " << (int)(altThetaError * 180 / M_PI);

        if(std::abs(altThetaError) < std::abs(thetaError))
        {
            thetaError = altThetaError;
        }

        // std::cout << "selected: " << (int)(thetaError * 180 / M_PI) << std::endl;

        int turnPower = thetaError * kP;
        int drivePower;

        if(std::abs(turnPower) > 1) drivePower = basePower / thetaError;
        else drivePower = basePower;

        int leftPower = basePower + turnPower;
        int rightPower = basePower - turnPower;

        if(leftPower < 0)
        {
            if(leftPower < -100) leftPower = -100;
        }
        else
        {
            if(leftPower > 100) leftPower = 100;
        }

        if(rightPower > 0)
        {
            if(rightPower > 100) rightPower = 100;
        }
        else
        {
            if(rightPower < -100) rightPower = -100;
        }

        left.set_duty_cycle_sp(leftPower);
        right.set_duty_cycle_sp(rightPower);
        
        if(std::abs(xError) + std::abs(yError) < 180)
        {
            targetState++;
            std::cout << "next state" << std::endl;
        }

        // std::cout << "target: " << (int)(targetTheta * 180 / M_PI) << " curr: " << (int)(currentTheta * 180 / M_PI) << " error: " << (altThetaError * 180 / M_PI) << std::endl;
        // std::cout << "target: " << (int)(targetTheta * 180 / M_PI) << " (" << (int)currentState.X << ", " << (int)currentState.Y << ")  theta: " << (int)(currentTheta * 180 / M_PI) << std::endl;
        // std::cout << "theta error: " << (int)(thetaError * 180 / M_PI) << "current Theta: " <<  (int)(currentTheta * 180 / M_PI) << std::endl;
        // std::cout << "turn power: " << turnPower << std::endl;
    }

    left.set_duty_cycle_sp(0);
    right.set_duty_cycle_sp(0);

    std::cout << "Press enter button to stop" << std::endl;
    while (!ev3dev::button::enter.pressed()) { }
}