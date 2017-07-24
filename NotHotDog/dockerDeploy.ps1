dotnet publish -c Release
cd ./bin/release/netcoreapp1.1/publish
docker build -t nothotdog .
docker tag nothotdog registry.heroku.com/nothotdog-dotnet/web
docker push registry.heroku.com/nothotdog-dotnet/web