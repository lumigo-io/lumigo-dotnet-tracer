#!/usr/bin/env bash
set -e
pushd test/Lumigo.DotNET.Test/ 2>&1
dotnet add package coverlet.collector
dotnet test --collect:"XPlat Code Coverage"
popd 2>&1
../utils/common_bash/defaults/code_cov.sh