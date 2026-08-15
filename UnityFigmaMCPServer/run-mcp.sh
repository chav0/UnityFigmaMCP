#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

dotnet build UnityFigmaMCPServer.sln -c Debug --nologo -v q 1>&2

exec dotnet exec bin/Debug/net8.0/UnityFigmaMCPServer.dll
