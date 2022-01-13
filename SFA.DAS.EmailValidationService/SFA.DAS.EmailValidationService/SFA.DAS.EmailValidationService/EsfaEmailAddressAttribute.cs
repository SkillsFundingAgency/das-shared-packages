using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmailValidationService
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EsfaEmailAddressAttribute : DataTypeAttribute
    {
        public EsfaEmailAddressAttribute() : base(DataType.EmailAddress)
        {
        }

        public override bool IsValid(object value)
            => value.ToString().IsAValidEmailAddress();
    }
}