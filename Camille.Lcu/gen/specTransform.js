const dotUtils = require('../../srcgen/dotUtils.js'); // Evil relative path.
const spec = require('./.spec.json');
const lcuTypeToCamilleEnum = {
  "LolRankedLeagueQueueType": { "x-enum": "queueType", "x-type": "int" },
  "LolRankedLeagueTier": { "x-enum": "tier", "x-type": "string" },
  "LolRankedLeagueDivision": { "x-enum": "division", "x-type": "string" }
};
const endpoints = [
  "/lol-login/v1/session",
  "/riotclient/region-locale",
  "/riotclient/kill-and-restart-ux",
  "/riotclient/kill-ux",
  "/riotclient/launch-ux",
  "/riotclient/unload",
  "/riotclient/ux-flash",
  "/riotclient/ux-minimize",
  "/riotclient/ux-show",
  "/riotclient/ux-state",
  "/lol-summoner/v1/check-name-availability-new-summoners/{name}",
  "/lol-summoner/v1/check-name-availability/{name}",
  "/lol-summoner/v1/current-summoner",
  "/lol-summoner/v1/current-summoner/autofill",
  "/lol-summoner/v1/current-summoner/icon",
  "/lol-summoner/v1/current-summoner/rerollPoints",
  "/lol-summoner/v1/current-summoner/summoner-profile",
  "/lol-summoner/v1/summoner-profile",
  "/lol-summoner/v1/summoners",
  "/lol-ranked/v1/apex-leagues/{queueType}/{tier}",
  "/lol-ranked/v1/current-ranked-stats",
  "/lol-ranked/v1/league-ladders/{puuid}",
  "/lol-ranked/v1/ranked-stats",
  "/lol-ranked/v1/ranked-stats/{puuid}",

  "/lol-ranked/v2/tiers",

  "/lol-champ-select/v1/bannable-champions",
  "/lol-champ-select/v1/battle-training/launch",
  "/lol-champ-select/v1/current-champion",
  "/lol-champ-select/v1/disabled-champions",
  "/lol-champ-select/v1/pickable-champions",
  "/lol-champ-select/v1/pickable-skins",
  "/lol-champ-select/v1/retrieve-latest-game-dto",
  "/lol-champ-select/v1/session",
  "/lol-champ-select/v1/session/actions/{id}",
  "/lol-champ-select/v1/session/actions/{id}/complete",
  "/lol-champ-select/v1/session/bench/swap/{championId}",
  "/lol-champ-select/v1/session/my-selection",
  "/lol-champ-select/v1/session/my-selection/reroll",
  "/lol-champ-select/v1/session/simple-inventory",
  "/lol-champ-select/v1/session/timer",
  "/lol-champ-select/v1/session/trades",
  "/lol-champ-select/v1/session/trades/{id}",
  "/lol-champ-select/v1/session/trades/{id}/accept",
  "/lol-champ-select/v1/session/trades/{id}/cancel",
  "/lol-champ-select/v1/session/trades/{id}/decline",
  "/lol-champ-select/v1/session/trades/{id}/request",
  "/lol-champ-select/v1/team-boost",
  "/lol-champ-select/v1/team-boost/purchase"
];
const namespaceRenames = {
  "RIOTCLIENT": "RiotClient"
};

function remapNamespaces(spec, endpoints = null, namespaceRenames = {}, lcuTypeToCamilleEnum = {}) {
  const dtoNamespaceMapping = {};
  const dtoNamespaceNewToOldMapping = {};

  function namespaceRenamed(namespace) {
    return namespaceRenames[namespace.toUpperCase()] || namespace;
  }
  function updateRef(objWithRef) {
    const ref = objWithRef['$ref'];
    if (!ref || ref.includes('.')) return; // '.' hack to prevent double update.
    const path = ref.split('/');
    const type = path.pop();

    if (lcuTypeToCamilleEnum[type]) {
      Object.assign(objWithRef, lcuTypeToCamilleEnum[type]);
      delete objWithRef['$ref'];
      return;
    }

    const namespace = dtoNamespaceMapping[type];
    path.push(`${namespaceRenamed(namespace)}.${dotUtils.removePrefix(type, dtoNamespaceNewToOldMapping[namespace])}`);
    objWithRef['$ref'] = path.join('/');
  }

  (endpoints
    ? endpoints.map(ep => spec.paths[ep])
    : Object.values(spec.paths))
    .forEach(path => Object.entries(path)
      .forEach(([ verb, op ]) => {
        // TODO.
        if (!(op.tags && op.tags.length)) throw new Error("Can't handle missing tags.");

        const oldNamespace = dotUtils.normalizeEndpointName(op.tags[op.tags.length - 1].split(' ').pop());
        const oldNamespaceRenamed = namespaceRenamed(oldNamespace);
        let namespace = oldNamespace;

        // Shorten method names already containing namespace/endpoint.
        let match;
        const regex = new RegExp(`${verb}${oldNamespace}(V\\d+)?`, 'i');
        if ((match = regex.exec(op.operationId))) {
          const [ prefix, versionOpt ] = match;
          namespace = oldNamespaceRenamed;
          op.operationId = dotUtils.capitalize(verb) + op.operationId.slice(prefix.length) + (versionOpt || '');
        }

        dtoNamespaceNewToOldMapping[namespace] = oldNamespace;
        path['x-endpoint'] = namespace;

        Object.values(op.responses || {})
          .concat(op.requestBody || [])
          .map(resp => resp.content).defined()
          .map(cont => cont['application/json']).defined()
          .map(json => json.schema)
          .concat((op.parameters || []).map(param => param.schema || param)) // Add-in parameters.
          .flatMap(schem => [ schem, schem.items, schem.additionalProperties ]).defined()
          .filter(schem => schem['$ref'])
          .forEach(schem => {
            const type = schem['$ref'].split('/').pop();
            dtoNamespaceMapping[type] = namespace;
            updateRef(schem);
          });
      })
    );

  const stack = Object.entries(dtoNamespaceMapping);
  while (stack.length) {
    const [ type, namespace ] = stack.pop();
    dtoNamespaceMapping[type] = namespace;

    let schema = spec.components.schemas[type];
    let childRefs = Object.values(schema.properties || {})
      .flatMap(propVal => [ propVal, propVal.items, propVal.additionalProperties ]).defined()
      .filter(propVal => propVal['$ref'])
      .forEach(propVal => {
        const type = propVal['$ref'].split('/').pop();
        if (!dtoNamespaceMapping[type]) {
          dtoNamespaceMapping[type] = namespace;
          stack.push([ type, namespace ]);
        }
        updateRef(propVal);
      });
  }
  
  // Remove those enums.
  Object.keys(lcuTypeToCamilleEnum).forEach(key => delete dtoNamespaceMapping[key]);

  const schemas = spec.components.schemas;
  const schemasToInclude = new Set();
  for (const [ schemaKey, namespace ] of Object.entries(dtoNamespaceMapping)) {
    const newNamespace = namespaceRenamed(namespace);
    const newSchemaKey = `${newNamespace}.${dotUtils.removePrefix(schemaKey, dtoNamespaceNewToOldMapping[namespace])}`;
    if (newSchemaKey === schemaKey)
      continue;
    (schemas[newSchemaKey] = schemas[schemaKey])['x-endpoint'] = newNamespace;
    delete schemas[schemaKey];

    schemasToInclude.add(newSchemaKey);
  }

  return schemasToInclude;
}

const schemasToInclude = remapNamespaces(spec, endpoints, namespaceRenames, lcuTypeToCamilleEnum);

module.exports = { endpoints, spec, schemasToInclude };
