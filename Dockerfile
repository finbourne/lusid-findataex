FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN mkdir -p /usr/src/findataex
WORKDIR /usr/src/findataex
COPY Lusid.FinDataEx\bin\Release\netcoreapp3.1 .

ENTRYPOINT ["./Lusid.FinDataEx.exe"]