# Play Catalog
Play Economy Catalog microservice

## Create and publish package
```powershell
$version="1.0.3"
$owner="waikahu"
$gh_pat="[PAT HERE]"

dotnet pack src\Play.Catalog.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=https://github.com/$owner/play.catalog -o ..\packages

dotnet nuget push ..\packages\Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat --source "github" 
```

## Build the Docker image (-t just so tag the build)
```powershell
$env:GH_OWNER="waikahu"
$env:GH_PAT="[PAT HERE]"
$crname="wbplayeconomy"
docker build --secret id=GH_OWNER --secret id=GH_PAT -t "$crname.azurecr.io/play.catalog:$version" .
```

## Run the docker image
```powershell
$cosmosDbConnString="[Conn HERE]"
$serviceBusConnString="[Conn HERE]"
docker run -it --rm -p 5000:5000 --name catalog -e MongoDbSettings__ConnectionString=$cosmosDbConnString -e ServiceBusSettings__ConnectionString=$serviceBusConnString -e ServiceSettings__MessageBroker="SERVICEBUS" play.catalog:$version
```

## Publishing the Docker image
```powershell
az acr login --name $crname
docker push "$crname.azurecr.io/play.catalog:$version"
```