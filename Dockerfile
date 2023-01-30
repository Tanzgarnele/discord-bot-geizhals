#Multi-Stage-Docker-Build: Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0

#Create Build folder
WORKDIR /App

#Copy entire project into Build folder
COPY . ./

#Build the projct
RUN dotnet build /App/ManfredHorst/ManfredHorst.csproj

ENTRYPOINT [ "dotnet", "run", "--project", "/App/ManfredHorst/" ]