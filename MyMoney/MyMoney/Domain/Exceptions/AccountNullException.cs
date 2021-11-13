﻿using System;
using System.Runtime.Serialization;

namespace MyMoney.Domain.Exceptions
{
    [Serializable]
    public class AccountNullException : Exception
    {
        public AccountNullException()
        {
        }

        protected AccountNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AccountNullException(string message) : base(message)
        {
        }

        public AccountNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
