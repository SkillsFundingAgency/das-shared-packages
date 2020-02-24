﻿using System;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IElasticClientFactory
    {
        IElasticClient CreateClient();
    }
}