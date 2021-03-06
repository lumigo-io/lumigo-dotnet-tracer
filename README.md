<p align="center">
    <img src="lumigo-logo.png"/>
</p>

[![CircleCI](https://circleci.com/gh/lumigo-io/lumigo-dotnet-tracer/tree/master.svg?style=svg&circle-token=ecd1acbb299c9ba28a7e0f6011fbfd1919079e36)](https://circleci.com/gh/lumigo-io/lumigo-dotnet-tracer/tree/master)
[![Nuget](https://img.shields.io/nuget/v/Lumigo.DotNET.svg)](https://www.nuget.org/packages/Lumigo.DotNET)

Easily trace your .NET Lambda functions using the [Lumigo platform](https://platform.lumigo.io/) 🚀

## Supported Runtimes
* .NET Core 2.1
* .NET Core 3.1

## Setup

### Installation
Add the Lumigo tracer package via NuGet by running:
```bash
Install-Package Lumigo.DotNET
```

### Wrapping Your Lambda

Wrap your lambda function by implementing a supplier which contains your code:

#### sync handler:
```csharp
using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
...
public class Function : LumigoRequestHandler
    {
        public Function()
        {
            LumigoBootstrap.Bootstrap();
        }

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

#### async handler:
```csharp
using Lumigo.DotNET;
using Lumigo.DotNET.Instrumentation;
...
public class Function : LumigoRequestHandler
    {
        public Function()
        {
            LumigoBootstrap.Bootstrap();
        }

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

### Connect Your Lumigo Account

Add `LUMIGO_TRACER_TOKEN` environment variable to connect the tracing to your Lumigo account.

### Track HTTP Requests
To track HTTP requests add `UseLumigo` to the HTTP client:
```csharp
using Lumigo.DotNET.Utilities.Extensions;
...
    var httpClient = new HttpClient().UseLumigo();
```
