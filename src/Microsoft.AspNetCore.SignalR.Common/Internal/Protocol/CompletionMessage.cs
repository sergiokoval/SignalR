// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.SignalR.Internal.Protocol
{
    public class CompletionMessage : HubMessage
    {
        public string Error { get; }
        public object Result { get; }
        public bool HasResult { get; }
        public bool IsStreamingCompletion { get; }

        public CompletionMessage(string invocationId, string error, object result, bool hasResult, bool isStreamingCompletion)
            : base(invocationId)
        {
            if (error != null && result != null)
            {
                throw new ArgumentException($"Expected either '{nameof(error)}' or '{nameof(result)}' to be provided, but not both");
            }

            if (isStreamingCompletion && (result != null || hasResult))
            {
                throw new ArgumentException("Unexpected result for streaming completion.");
            }

            Error = error;
            Result = result;
            HasResult = hasResult;
            IsStreamingCompletion = isStreamingCompletion;
        }

        public override string ToString()
        {
            var caption = $"{(IsStreamingCompletion ? "Streaming" : string.Empty)} Completion";
            var errorStr = Error == null ? "<<null>>" : $"\"{Error}\"";
            var resultField = HasResult ? $", {nameof(Result)}: {Result ?? "<<null>>"}" : string.Empty;
            return $"{caption} {{ {nameof(InvocationId)}: \"{InvocationId}\", {nameof(Error)}: {errorStr}{resultField} }}";
        }

        // Static factory methods. Don't want to use constructor overloading because it will break down
        // if you need to send a payload statically-typed as a string. And because a static factory is clearer here
        public static CompletionMessage WithError(string invocationId, string error)
            => new CompletionMessage(invocationId, error, result: null, hasResult: false, isStreamingCompletion: false);

        public static CompletionMessage WithResult(string invocationId, object payload)
            => new CompletionMessage(invocationId, error: null, result: payload, hasResult: true, isStreamingCompletion: false);

        public static CompletionMessage Empty(string invocationId)
            => new CompletionMessage(invocationId, error: null, result: null, hasResult: false, isStreamingCompletion: false);

        public static CompletionMessage WithStreamError(string invocationId, string error)
            => new CompletionMessage(invocationId, error, result: null, hasResult: false, isStreamingCompletion: true);

        public static CompletionMessage ForStream(string invocationId)
            => new CompletionMessage(invocationId, error: null, result: null, hasResult: false, isStreamingCompletion: true);
    }
}
