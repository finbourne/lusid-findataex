FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN mkdir -p /usr/src/prebuild
WORKDIR /usr/src

# until drive-sdk depedency is public need to use pre published builds
#COPY Lusid.FinDataEx/Lusid.FinDataEx.sln /usr/src/
#COPY Lusid.FinDataEx/Lusid.FinDataEx.csproj /usr/src/Lusid.FinDataEx/
#RUN dotnet restore Lusid.FinDataEx

COPY prebuild /usr/src/prebuild/
COPY submit_dlws.sh /usr/src/
ENV LUSID_FINDATA_EX_DLL=/usr/src/prebuild/Lusid.FinDataEx.dll

ENTRYPOINT ["./submit_dlws.sh"]
