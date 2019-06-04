using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.Authorization
{
    public class UserMessage : IUserMessage
    {
        [IgnoreMap]
        [Required]
        public Guid? UserRef { get; set; }
    }
}