FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN mkdir -p /usr/src/findataex
COPY ./src/Lusid.FinDataEx/bin/Release/netcoreapp3.1 /usr/src/findataex

WORKDIR /usr/src/findataex

ENTRYPOINT [ "dotnet", "Lusid.FinDataEx.dll" ]
CMD [ "--help" ]