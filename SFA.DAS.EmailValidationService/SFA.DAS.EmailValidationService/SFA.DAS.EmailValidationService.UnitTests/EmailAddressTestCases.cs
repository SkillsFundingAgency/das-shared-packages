using System.Collections;

namespace SFA.DAS.EmailValidationService.UnitTests
{
    internal class ValidEmailCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { "joe@home.org" };
            yield return new object[] { "joe@joebob.name" };
            yield return new object[] { "joe&bob@bob.com" };
            yield return new object[] { "~joe@bob.com" };
            yield return new object[] { "joe$@bob.com" };
            yield return new object[] { "joe+bob@bob.com" };
            yield return new object[] { "o'reilly@there.com" };
            yield return new object[] { "joe@home.com" };
            yield return new object[] { "joe.bob@home.com" };
            yield return new object[] { "joe@his.home.com" };
            yield return new object[] { "a@abc.org" };
            yield return new object[] { "a@abc-xyz.org" };
            yield return new object[] { "a@abc-xyz.london" };
        }
    }

    internal class InvalidEmailCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { "joe" };                   // should fail
            yield return new object[] { "joe@home" };              // should fail
            yield return new object[] { "joe-bob[at]home.com" };   // should fail because [at] is not valid
            yield return new object[] { "joe.@bob.com" };          // should fail because there is a dot at the end of the local-part
            yield return new object[] { ".joe@bob.com" };          // should fail because there is a dot at the beginning of the local-part
            yield return new object[] { "john..doe@bob.com" };     // should fail because there are two dots in the local-part
            yield return new object[] { "john.doe@bob..com" };     // should fail because there are two dots in the domain
            yield return new object[] { "joe<>bob@bob.com" };      // should fail because <> are not valid
            yield return new object[] { "joe@his.home.com." };     // should fail because it can't end with a period
            yield return new object[] { "john.doe@bob-.com" };     // should fail because there is a dash at the start of a domain part
            yield return new object[] { "john.doe@-bob.com" };     // should fail because there is a dash at the end of a domain part
            yield return new object[] { "a@10.1.100.1a" };         // Should fail because of the extra character
            yield return new object[] { "joe<>bob@bob.com\n" };    // should fail because it end with \n
            yield return new object[] { "joe<>bob@bob.com\r" };    // should fail because it ends with \r
            yield return new object[] { "a@192.168.0.1" };         // fails as IP addresses are not allowed
            yield return new object[] { "a@10.1.100.1" };          // fails as IP addresses are not allowed
            yield return new object[] { "BOBJONSTONE12@EXAMPLE.COM�" };
            yield return new object[] { "Emma1997@example.com�" };
            yield return new object[] { "bob59 @example" };
            yield return new object[] { "bob.bob53@ example.co.uk" };
            yield return new object[] { "'bob@example.co.uk'" };
            yield return new object[] { "bob91 @example.com0" };
            yield return new object[] { " bob@example.com" };
            yield return new object[] { "b o b@example.com" };
            yield return new object[] { "bob@example.com " }; // whitespace at the end of the domain
        }
    }
}