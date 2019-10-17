echo "Downloading OpenAPI Spec."
Invoke-WebRequest -Uri "http://www.mingweisamuel.com/riotapi-schema/openapi-3.0.0.json" -OutFile .\.spec.json

echo "Downloading DD Info."
$DD = ((Invoke-WebRequest -Uri "http://ddragon.leagueoflegends.com/realms/na.json").Content | ConvertFrom-Json).dd
echo "DD version $DD."

Invoke-WebRequest -Uri "http://ddragon.leagueoflegends.com/cdn/$DD/data/en_US/champion.json" -OutFile .\.champion.json

echo "Downloading static data"

Invoke-WebRequest -Uri "http://static.developer.riotgames.com/docs/lol/seasons.json" -OutFile .\Enums\.seasons.json
Invoke-WebRequest -Uri "http://static.developer.riotgames.com/docs/lol/queues.json" -OutFile .\Enums\.queues.json
Invoke-WebRequest -Uri "http://static.developer.riotgames.com/docs/lol/maps.json" -OutFile .\Enums\.maps.json
Invoke-WebRequest -Uri "http://static.developer.riotgames.com/docs/lol/gameModes.json" -OutFile .\Enums\.gameModes.json
Invoke-WebRequest -Uri "http://static.developer.riotgames.com/docs/lol/gameTypes.json" -OutFile .\Enums\.gameTypes.json
