echo "Downloading OpenAPI Spec."
Invoke-WebRequest -Uri "http://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json" -OutFile .\.spec.json

echo "Downloading DD Info."
$DD = ((Invoke-WebRequest -Uri "http://ddragon.leagueoflegends.com/realms/na.json").Content | ConvertFrom-Json).dd
echo "DD version $DD."

Invoke-WebRequest -Uri "http://ddragon.leagueoflegends.com/cdn/$DD/data/en_US/champion.json" -OutFile .\.champion.json
