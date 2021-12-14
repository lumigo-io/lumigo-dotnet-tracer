#!/usr/bin/env bash
set -eo pipefail

pushd "$(dirname "$0")" &> /dev/null
# Go back one spot because we are in scripts dir. The other scripts assume you are in the root folder
cd ..
../utils/common_bash/defaults/ci_deploy.sh dotnet-tracer
popd &> /dev/null
