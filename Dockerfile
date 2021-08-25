FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN mkdir -p /usr/src/prebuild
COPY prebuild /usr/src/prebuild/
WORKDIR /usr/src/prebuild

ENTRYPOINT ["dotnet Lusid.FinDataEx.dll"]