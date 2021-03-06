﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.CommonModel
{
    public interface ICommandResult
    {
        string StatusCode { get; set; }
        object Result { get; set; }
    }


    public interface ICommand
    {
        string Argument { get; set; }
        ICommandResult Run();

    }
}
