# SecureBank
make build
docker tag ssrd/securebank ssrd/securebank:2.x
docker login
docker push ssrd/securebank:2.x
docker tag ssrd/securebank ssrd/securebank:latest
docker push ssrd/securebank:latest

