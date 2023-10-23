﻿using System.Collections.Generic;

namespace SFA.DAS.Telemetry.RedactionService
{
    public class UriRedactionOptions
    {
        public List<string> RedactionList { get; set; } = new List<string>();
        public string RedactionString = "*";
    }
}