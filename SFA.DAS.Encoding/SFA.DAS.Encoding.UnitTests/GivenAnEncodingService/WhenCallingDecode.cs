﻿using System;
using AutoFixture.NUnit3;
using HashidsNet;
using NUnit.Framework;

namespace SFA.DAS.Encoding.UnitTests.GivenAnEncodingService
{
    [TestFixture]
    public class WhenCallingDecode
    {
        [Test, AutoData]
        public void Then_Decodes_Based_On_EncodingTpe(
            long expectedDecoded,
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var hashids = new Hashids(config.Encodings[1].Salt, config.Encodings[1].MinHashLength, config.Encodings[1].Alphabet);
            var encoded = hashids.EncodeLong(expectedDecoded);
            var encodingService = new EncodingService(config);

            var decodedValue = encodingService.Decode(encoded, encodingType);

            Assert.AreEqual(expectedDecoded,decodedValue);
        }

        [Test, AutoData]
        public void Then_Throws_Exception_If_Empty_String(
            EncodingConfig config)
        {
            var encodingType = config.Encodings[1].EncodingType;
            var encodingService = new EncodingService(config);

            Assert.Throws<ArgumentException>(() => encodingService.Decode("", encodingType));
        }
    }
}