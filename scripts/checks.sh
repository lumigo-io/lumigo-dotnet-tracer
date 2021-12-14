#!/usr/bin/env bash
set -e

# Check that dotnet is installed
command -v dotnet >/dev/null 2>&1 || { echo >&2 "I require dotnet but it's not installed.  Aborting."; exit 1; }

dotnet format --help >/dev/null 2>&1 || { echo "Missing format command, installing"; dotnet tool install -g dotnet-format; }
echo "Formatting code..."
dotnet format

echo "Running tests..."
dotnet test test/Lumigo.DotNET.Test/Lumigo.DotNET.Test.csproj

echo "Building..."
dotnet build -c Release src/Lumigo.DotNET/Lumigo.DotNET.csproj 