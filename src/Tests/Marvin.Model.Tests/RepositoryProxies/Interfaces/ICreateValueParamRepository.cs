﻿using System;

namespace Marvin.Model.Tests
{
    public interface ICreateValueParamRepository : IRepository<SomeEntity>
    {
        SomeEntity Create(Int32 value);
    }
}