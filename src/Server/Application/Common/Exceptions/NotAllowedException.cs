﻿using System;
namespace WeeControl.Server.Application.Common.Exceptions
{
    public class NotAllowedException : Exception
    {
        public NotAllowedException() : base()
        {
        }

        public NotAllowedException(string argument) : base(argument)
        {
        }
    }
}