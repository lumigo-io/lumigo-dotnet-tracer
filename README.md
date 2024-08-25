![Lumigo.io](lumigo-logo.png)

[![CircleCI](https://dl.circleci.com/status-badge/img/gh/lumigo-io/lumigo-dotnet-tracer/tree/master.svg?style=svg)](https://dl.circleci.com/status-badge/redirect/gh/lumigo-io/lumigo-dotnet-tracer/tree/master)
[![Nuget](https://img.shields.io/nuget/v/Lumigo.DotNET.svg)](https://www.nuget.org/packages/Lumigo.DotNET)

Easily trace your .NET Lambda functions using the [Lumigo platform](https://platform.lumigo.io/) 🚀

## Supported Runtimes
* .NET Core 3.1
* .NET 6
* .NET 8

## Setup

### Installation

Add the Lumigo tracer package via NuGet by running:

```bash
dotnet add package Lumigo.DotNET
```

### Wrapping Your Lambda

Wrap your lambda function by implementing a supplier which contains your code:

#### Synchronous handler

```csharp
using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
...
public class Function : LumigoRequestHandler
    {
        public Response FunctionHandler(string input, ILambdaContext context)
        {
            return Handle(input, context, () =>
            {
                //Your lambda code
                //return <result>; - For void functions remove the return statements
            });
        }
    }
```

#### Asynchronous handler

```csharp
using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
...
public class Function : LumigoRequestHandler
    {
        public async Task<Response> FunctionHandler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
            {
                //Your lambda code
                //return <result>; - For void functions remove the return statements
            });
        }
    }
```

### Execution Tags

Execution tags allow you to dynamically add dimensions to your Lambda function invocations so that they can be identified, searched for, and filtered in Lumigo. 
They can be utilized to find specific invocations and create custom widgets, helping you simplify the complexity of monitoring distributed applications.
The [Quick Bytes video for Execution Tags](https://docs.lumigo.io/docs/execution-tags#lumigo-quick-bytes---execution-tags) video gives examples of use-cases for execution tags.

Adding an execution tag to a Lambda invocation is done via the `LumigoRequestHandler.AddExecutionTag` API:

```csharp
using Amazon.Lambda.Core;

using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
using Lumigo.DotNET.Utilities.Extensions;

using System.Net.Http;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloDotNet6 {

    public class Function : LumigoRequestHandler
    {
        public async Task<string> Handler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
                {
                    this.AddExecutionTag("Key1", "Value1");

                    return "\"Hello world\"";
                }
            );
        }
    }
}
```

It is possible to set multiple execution tags for the same Lambda invocation, as well as multiple values for the same execution tag:

```csharp
using Amazon.Lambda.Core;

using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
using Lumigo.DotNET.Utilities.Extensions;

using System.Net.Http;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloDotNet6 {

    public class Function : LumigoRequestHandler
    {
        public async Task<string> Handler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
                {
                    this.AddExecutionTag("Key1", "Value1");
                    this.AddExecutionTag("Key2", "Value2");
                    this.AddExecutionTag("Key2", "Value3");

                    return "\"Hello world\"";
                }
            );
        }
    }
}
```

### Connect Your Lumigo Account

Add `LUMIGO_TRACER_TOKEN` environment variable to connect the tracing to your Lumigo account.

### Track HTTP Requests
To track HTTP requests add `UseLumigo` to the HTTP client:

```csharp
using Amazon.Lambda.Core;

using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
using Lumigo.DotNET.Utilities.Extensions;

using System.Net.Http;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloDotNet6 {

    public class Function : LumigoRequestHandler
    {
        public async Task<string> Handler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
                {
                    HttpResponseMessage response = await new HttpClient().UseLumigo().GetAsync("https://httpbin.org/status/200");
                    response.EnsureSuccessStatusCode();
                    return "\"Hello world\"";
                }
            );
        }
    }
}
```
