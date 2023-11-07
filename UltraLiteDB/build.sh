#!/bin/bash

echo "Building UltraLiteDB"

dotnet build -c Release
dotnet build -c Debug

