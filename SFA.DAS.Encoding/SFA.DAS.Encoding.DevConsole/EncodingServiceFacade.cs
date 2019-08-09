using System;
using SFA.DAS.Encoding.DevConsole.Interfaces;

namespace SFA.DAS.Encoding.DevConsole
{
    public class EncodingServiceFacade : IEncodingServiceFacade
    {
        private readonly IEncodingService _encodingService;

        public EncodingServiceFacade(IEncodingService encodingService)
        {
            _encodingService = encodingService;
        }

        public void RunEncodingService(Arguments arguments)
        {
            switch (arguments.ActionType)
            {
                case ActionType.Encode:
                    long.TryParse(arguments.Value, out var rawValue);
                    var encodedValue = _encodingService.Encode(rawValue, arguments.EncodingType);
                    Console.WriteLine($"encoded value: {encodedValue}");
                    break;
                case ActionType.Decode:
                    var decodedValue = _encodingService.Decode(arguments.Value, arguments.EncodingType);
                    Console.WriteLine($"decoded value: {decodedValue}");
                    break;
                default:
                    Console.WriteLine("Error: invalid action");
                    return;
            }
        }
    }
}