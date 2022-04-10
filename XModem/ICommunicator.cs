﻿namespace XModem;

public interface ICommunicator
{
    void PrintHeader(object message);
    void Prompt(object message);
    void Print(object message);
}