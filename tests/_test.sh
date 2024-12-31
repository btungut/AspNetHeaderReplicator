#!/bin/bash
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
rm -rf $SCRIPT_DIR/coverage*
dotnet build
dotnet test --collect:"XPlat Code Coverage" --results-directory $SCRIPT_DIR/coverage
reportgenerator -reports:$SCRIPT_DIR/coverage/*/*.xml -targetdir:$SCRIPT_DIR/coverage_report -reporttypes:Html

echo "Coverage report generated at $SCRIPT_DIR/coverage_report/index.html"