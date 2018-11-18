﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.Common.Model
{
    public interface ICommandResult
    {
        string StatusCode { get; set; }
        object Result { get; set; }
    }


    public interface ICommand
    {
        string ArgumentCollection { get; set; }
        ICommandResult Run();

    }
}