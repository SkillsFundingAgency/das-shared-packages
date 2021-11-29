using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.EmailValidationService.UnitTests
{
    public class EmailValidationTests
    {

        [TestCase("joe@home.org")]
        [TestCase("joe@joebob.name")]
        [TestCase("joe&bob@bob.com")]
        [TestCase("~joe@bob.com")]
        [TestCase("joe$@bob.com")]
        [TestCase("joe+bob@bob.com")]
        [TestCase("o'reilly@there.com")]
        [TestCase("joe@home.com")]
        [TestCase("joe.bob@home.com")]
        [TestCase("joe@his.home.com")]
        [TestCase("a@abc.org")]
        [TestCase("a@abc-xyz.org")]
        public void EmailShouldBeValid(string email)
        {
            email.IsAValidEmailAddress().Should().BeTrue();
        }


        [TestCase("joe")]                   // should fail
        [TestCase("joe@home")]              // should fail
        [TestCase("a@b.c")]                 // should fail because .c is only one character but must be 2-4 characters
        [TestCase("joe-bob[at]home.com")]   // should fail because [at] is not valid
        [TestCase("joe@his.home.place")]    // should fail because place is 5 characters but must be 2-4 characters
        [TestCase("joe.@bob.com")]          // should fail because there is a dot at the end of the local-part
        [TestCase(".joe@bob.com")]          // should fail because there is a dot at the beginning of the local-part
        [TestCase("john..doe@bob.com")]     // should fail because there are two dots in the local-part
        [TestCase("john.doe@bob..com")]     // should fail because there are two dots in the domain
        [TestCase("joe<>bob@bob.com")]      // should fail because <> are not valid
        [TestCase("joe@his.home.com.")]     // should fail because it can't end with a period
        //[TestCase("john.doe@bob-.com")]     // should fail because there is a dash at the start of a domain part
        [TestCase("john.doe@-bob.com")]     // should fail because there is a dash at the end of a domain part
        [TestCase("a@10.1.100.1a")]         // Should fail because of the extra character
        [TestCase("joe<>bob@bob.com\n")]    // should fail because it end with \n
        [TestCase("joe<>bob@bob.com\r")]    // should fail because it ends with \r
        [TestCase("a@192.168.0.1")]         // fails as IP addresses are not allowed
        [TestCase("a@10.1.100.1")]          // fails as IP addresses are not allowed
        [TestCase("BOBJONSTONE12@EXAMPLE.COM�")]
        [TestCase("Emma1997@example.com�")]
        [TestCase("bob59 @example")]
        [TestCase("bob.bob53@ example.co.uk")]
        [TestCase("'bob@example.co.uk'")]
        [TestCase("bob91 @example.com0")]
        [TestCase(" bob@example.com")]
        [TestCase("b o b@example.com")]
        [TestCase("bob@example.com ")] // whitespace at the end of the domain
        public void EmailShouldBeInvalid(string email)
        {
            email.IsAValidEmailAddress().Should().BeFalse();
        }
    }
}
