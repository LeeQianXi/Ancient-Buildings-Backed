#!/bin/bash

export DATABASE_CONNECTION_STRING_FILE='/home/qianxi/source/repos/Web/Buildings.WebApi/secrets/connection_string'

dotnet-ef migrations add AddUserMoreInfo
dotnet-ef database update