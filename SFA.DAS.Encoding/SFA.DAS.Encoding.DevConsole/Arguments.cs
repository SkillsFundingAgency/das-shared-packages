﻿namespace SFA.DAS.Encoding.DevConsole
{
    public class Arguments
    {
        public bool ShowHelp { get; set; }
        public ActionType ActionType { get; set; } = ActionType.Decode;
        public EncodingType EncodingType { get; set; } = EncodingType.AccountId;
        public string Value { get; set; }
    }
}