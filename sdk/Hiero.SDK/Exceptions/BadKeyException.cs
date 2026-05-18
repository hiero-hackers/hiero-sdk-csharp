// SPDX-License-Identifier: Apache-2.0
using Hiero.Reference.Error;
using System;

namespace Hiero.SDK.Exceptions
{
    /// <include file="BadKeyException.cs.xml" path='docs/member[@name="T:BadKeyException"]' />
    public class BadKeyException : Exception, IBadKey 
    {
        public BadKeyException(string message) : base(message) { }
        public BadKeyException(Exception exception) : base(exception.Message, exception) { }
        public BadKeyException(string message, Exception exception) : base(message, exception) { }
    }
}