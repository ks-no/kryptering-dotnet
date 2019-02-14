FROM microsoft/dotnet:sdk
WORKDIR /app

VOLUME ["/app"]

# RUN dotnet restore   
# RUN dotnet build --no-restore -c Release  
# RUN dotnet test
# RUN dotnet pack --no-restore --no-build -c Release 