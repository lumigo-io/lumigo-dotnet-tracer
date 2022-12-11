#!/usr/bin/env bash

set -e

pushd test/Lumigo.DotNET.Test/
    dotnet add package coverlet.collector
    dotnet test --collect:"XPlat Code Coverage"
popd

../utils/common_bash/defaults/code_cov.sh