FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS temp
RUN mkdir -p /var/app/
COPY Lusid.FinDataEx/bin/Debug/netcoreapp3.1 /var/app/

FROM mcr.microsoft.com/dotnet/core/sdk:3.1
RUN mkdir -p /var/app/
COPY --from=temp /var/app/ /var/app/
WORKDIR /var/app/
ENTRYPOINT [ "dotnet", "Lusid.FinDataEx.dll"]