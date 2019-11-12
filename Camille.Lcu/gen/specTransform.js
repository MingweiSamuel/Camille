const dotUtils = require('../../srcgen/dotUtils.js'); // Evil relative path.
const spec = require('./spec.json');
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
  "/lol-ranked/v1/ranked-stats/{puuid}"
];
const namespaceRenames = {
  "RIOTCLIENT": "RiotClient"
};

function remapNamespaces(spec, endpoints = null, namespaceRenames = {}) {
  const dtoNamespaceMapping = {};

  function namespaceRenamed(namespace) {
    return namespaceRenames[namespace.toUpperCase()] || namespace;
  }
  function updateRef(objWithRef) {
    const ref = objWithRef['$ref'];
    if (!ref || ref.includes('.')) return; // '.' hack to prevent double update.
    const path = ref.split('/');
    const type = path.pop();
    const namespace = dtoNamespaceMapping[type];
    path.push(`${namespaceRenamed(namespace)}.${dotUtils.removePrefix(type, namespace)}`);
    objWithRef['$ref'] = path.join('/');
  }

  (endpoints
    ? endpoints.map(ep => spec.paths[ep])
    : Object.values(spec.paths))
    .forEach(path => Object.values(path)
      .forEach(op => {
        if (!(op.tags && op.tags.length)) return; // No need to update without tags.
        const namespace = dotUtils.normalizeEndpointName(op.tags[op.tags.length - 1].split(' ').pop());
        path['x-endpoint'] = namespaceRenamed(namespace);
        Object.values(op.responses || {})
          .concat(op.requestBody || [])
          .map(resp => resp.content).defined()
          .map(cont => cont['application/json']).defined()
          .map(json => json.schema)
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

  const schemas = spec.components.schemas;
  const schemasToInclude = new Set();
  for (const [ schemaKey, namespace ] of Object.entries(dtoNamespaceMapping)) {
    const newNamespace = namespaceRenamed(namespace);
    const newSchemaKey = `${newNamespace}.${dotUtils.removePrefix(schemaKey, namespace)}`;
    if (newSchemaKey === schemaKey)
      continue;
    (schemas[newSchemaKey] = schemas[schemaKey])['x-endpoint'] = newNamespace;
    delete schemas[schemaKey];

    schemasToInclude.add(newSchemaKey);
  }
  return schemasToInclude;
}

const schemasToInclude = remapNamespaces(spec, endpoints, namespaceRenames);

module.exports = { endpoints, spec, schemasToInclude };
