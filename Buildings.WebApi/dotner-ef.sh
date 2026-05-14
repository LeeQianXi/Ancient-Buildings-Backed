#!/bin/bash

export DATABASE_CONNECTION_STRING_FILE='/home/qianxi/source/repos/Web/Buildings.WebApi/secrets/connection_string'

#dotnet-ef migrations add AddRed
dotnet-ef database update