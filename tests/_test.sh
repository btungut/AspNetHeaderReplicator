#!/bin/bash
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
rm -rf $SCRIPT_DIR/coverage*
SLN_DIR="$SCRIPT_DIR/../*.sln"

dotnet clean $SLN_DIR
dotnet build --configuration Release $SLN_DIR

dotnet test --collect:"XPlat Code Coverage" --results-directory $SCRIPT_DIR/coverage
reportgenerator -reports:$SCRIPT_DIR/coverage/*/*.xml -targetdir:$SCRIPT_DIR/coverage_report -reporttypes:Html

echo -e "Coverage report generated at \n\n$SCRIPT_DIR/coverage_report/index.html"