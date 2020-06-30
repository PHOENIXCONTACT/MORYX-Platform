// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations.Schema;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    public enum WheelType
    {
        FrontLeft,
        FrontRight,
        RearLeft,
        RearRight
    }

    [Table(nameof(WheelEntity), Schema = TestModelConstants.CarsSchema)]
    public class WheelEntity : EntityBase
    {
        public virtual CarEntity Car { get; set; }

        public virtual WheelType WheelType { get; set; }
    }
}