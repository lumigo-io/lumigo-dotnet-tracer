# Lumigo .NET Tracer

[![CircleCI](https://circleci.com/gh/lumigo-io/dotnet-tracer/tree/master.svg?style=svg&circle-token=96ef77fcbf5871cfcab9edb54e47248744383589)](https://circleci.com/gh/lumigo-io/dotnet-tracer/tree/master)
[![Nuget](https://img.shields.io/nuget/v/Lumigo.DotNET.svg)](https://www.nuget.org/packages/Lumigo.DotNET)
[![codecov](https://codecov.io/gh/lumigo-io/dotnet-tracer/branch/master/graph/badge.svg?token=IzEidxOAuM)](https://codecov.io/gh/lumigo-io/dotnet-tracer)

Supported Runtimes: .NET Core 2.1, .NET Core 3.1

This readme explains how to deploy .NET Test function that impletemets "Lumigo .NET Tracer"

## Adding NuGet Package

```bash
    Install-Package Lumigo.DotNET
```

## Wrapping your Lambda

* Wrap your lambda function by implementing a supplier which contains your code:

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

        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
            {
                //Your lambda code
                //return <result>;
            });
        }
    }
```

* For handler return void use:

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

        public async Task FunctionHandler(string input, ILambdaContext context)
        {
            return await Handle(input, context, async () =>
            {
                //Your lambda code
            });
        }
    }
```

## Configuration

### Environment variables

- Add `LUMIGO_TRACER_TOKEN` environment variable.

## Building and Deployment using the Serverless framework

* Make sure you have `dotnet` installed on your machine by running `brew install --cask dotnet`.
* Create a .Net project template by running `sls create -t aws-csharp -n hello-world .`. This command will create a `.net` template for a service named `hello-world` in current directory.
* Add `Lumigo.DotNET` as a dependency of the project.
* Wrap the lambda handler using `Lumigo.DotNET`.
* Run `./build.sh` in the created template directory. The command will build the zip artifact for the Lambda.
* Run `sls deploy` to deploy your lambda.
**Pay attention** Each time you are changing the code you need to run `./buid.sh` and then `sls deploy`, in case you are only changing the `serverless.yml` run `sls deploy`.
