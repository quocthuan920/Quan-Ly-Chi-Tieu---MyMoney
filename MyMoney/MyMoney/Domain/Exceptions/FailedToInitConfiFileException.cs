﻿using System;
using System.Runtime.Serialization;

namespace MyMoney.Domain.Exceptions
{
    [Serializable]
    public class FailedToInitConfiFileException : Exception
    {
        public FailedToInitConfiFileException()
        {
        }

        protected FailedToInitConfiFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FailedToInitConfiFileException(string message) : base(message)
        {
        }

        public FailedToInitConfiFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
