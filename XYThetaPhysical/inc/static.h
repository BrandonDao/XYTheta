#pragma once
#include <iostream>
#include <string.h>
#include "ev3dev.h"

static double Max(double val1, double val2)
{
    if (val1 < val2)
    {
        return val2;
    }
    else
    {
        return val1;
    }
}

static const char* GetI2CPort(const char* port)
{
    if(strcmp(port, ev3dev::INPUT_1) == 0)
    {
        return "ev3-ports:in1:i2c1";
    }
    else if(strcmp(port, ev3dev::INPUT_2) == 0)
    {
        return "ev3-ports:in2:i2c1";
    }
    else if(strcmp(port, ev3dev::INPUT_3) == 0)
    {
        return "ev3-ports:in3:i2c1";
    }
    else if(strcmp(port, ev3dev::INPUT_4) == 0)
    {
        return "ev3-ports:in4:i2c1";
    }
    else
    {
        //return port;
        std::cout << "I Don't know how to throw exceptions so I'll just stall the program. Also, invalid SensorPort selected" << std::flush;
        while (true)
        {
        }
        return "INVALID ADDRESS";
    }
}


static const char* GetPort(int portNumber)
{
    switch (portNumber)
    {
    case 1:
        return ev3dev::INPUT_1;
        break;

    case 2:
        return ev3dev::INPUT_2;
        break;

    case 3:
        return ev3dev::INPUT_3;
        break;

    case 4:
        return ev3dev::INPUT_4;
        break;

    default:
        std::cout << "Can't throw exceptions so I'll just stall the program. Also, invalid SensorPort selected" << std::flush;
        while (true) { }
        break;
    }
}

static ev3dev::address_type GetPort(char portLetter)
{
    switch (portLetter)
    {
    case 'a':
        return ev3dev::OUTPUT_A;
        break;

    case 'A':
        return ev3dev::OUTPUT_A;
        break;

    case 'b':
        return ev3dev::OUTPUT_B;
        break;

    case 'B':
        return ev3dev::OUTPUT_B;
        break;

    case 'c':
        return ev3dev::OUTPUT_C;
        break;

    case 'C':
        return ev3dev::OUTPUT_C;
        break;

    case 'd':
        return ev3dev::OUTPUT_D;
        break;

    case 'D':
        return ev3dev::OUTPUT_D;
        break;

    default:
        std::cout << "Can't throw exceptions so I'll just stall the program. Also, invalid SensorPort selected" << std::flush;
        while (true) { }
        break;
    }
}