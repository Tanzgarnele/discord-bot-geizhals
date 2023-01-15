#Multi-Stage-Docker-Build: Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

#Create Build folder
WORKDIR /Build

#Copy entire project into Build folder
COPY . ./

#Build the .dll file and move it into the folder /App/out
RUN dotnet dotnet publish -c Release -o out


#Multi-Stage-Docker-Build: Stage 2
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base-image

#Creat App directory
WORKDIR /App

#Copy the .dll from the build image
COPY --from=build /Build/out .

#Execute .dll
ENTRYPOINT [ "dotnet", "ManfredHorst.dll" ]