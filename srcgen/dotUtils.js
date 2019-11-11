function capitalize(input) {
  return input[0].toUpperCase() + input.slice(1);
}

function decapitalize(input) {
  return input[0].toLowerCase() + input.slice(1);
}

function escapeKeyword(name) {
  switch (name.toUpperCase()) {
    case 'PUBLIC':
      return '@' + name;
  }
  return name;
}

function removePrefix(name, prefix) {
  return name.startsWith(prefix) ? name.slice(prefix.length) : name;
}

function normalizeEndpointName(name) {
  return name.split('-')
    //    .slice(0, -1)
    .map(capitalize)
    .join('');
}

function normalizeSchemaName(name) {
  return name.replace(/DTO/ig, '');
}

function normalizeArgName(name) {
  var tokens = name.split('_');
  var argName = decapitalize(tokens.map(capitalize).join(''));
  return 'base' === argName ? 'Base' : argName;
}

function normalizePropName(propName, schemaName, value) {
  var tokens = propName.split('_');
  var name = tokens.map(capitalize).join('');
  if (name === schemaName)
    name += stringifyType(value);
  return name;
}

function stringifyType(prop, endpoint = null) {
  if (!prop)
    return 'UNDEFINED_TYPE';

  // Use first field of anyOf. HACK.
  if (prop.anyOf)
    prop = prop.anyOf[0];

  let refType = prop['$ref'];
  if (refType) {
    refType = refType.split('/').pop();
    refType = refType.split('.').pop();
    return (endpoint ? endpoint + '.' : '') + normalizeSchemaName(refType);
  }
  var enumName = prop['x-enum'];
  if (enumName !== undefined) {
    return normalizePropName(enumName, 'placeholder', '') + (prop.type === 'array' ? '[]' : '');
  }

  switch (prop.type) {
    case 'boolean': return 'bool';
    case 'integer': return ('int32' === prop.format ? 'int' : 'long');
    case 'number': return prop.format;
    case 'array': return stringifyType(prop.items, endpoint) + '[]';
    case 'object':
      const keyType = prop['x-key'] ? stringifyType(prop['x-key'], endpoint) : 'string';
      return `IDictionary<${keyType}, ${stringifyType(prop.additionalProperties, endpoint)}>`;
    default: return prop.type || 'Dictionary<string, object>';
  }
}

function replaceEnumCasts(input) {
    return input.replace("{championId}", "{(int)championId}");
}

function formatJsonProperty(name) {
  return `[JsonProperty(\"${name}\")]`;
}

function formatQueryParamStringify(prop) {
  switch (prop.type) {
    case 'boolean': return '.ToString().ToLowerInvariant()';
    case 'string': return '';
    default: return '.ToString()';
  }
}

function formatAddQueryParam(param) {
  let k = `nameof(${param.name})`;
  let nc = param.required ? '' : `if (null != ${param.name}) `;
  let prop = param.schema;
  if (!prop)
    return 'UNDEFINED_QP';
  switch (prop.type) {
    case 'array': return `${nc}queryParams.AddRange(${param.name}.Select(`
          + `w => new KeyValuePair<string, string>(${k}, ((int)w)${formatQueryParamStringify(prop.items)})))`;
    case 'object': throw 'unsupported';
    default:
      let vnc = param.required ? '' : '.Value';
      return `${nc}queryParams.Add(new KeyValuePair<string, string>(${k}, `
        + `${param.name}${vnc}${formatQueryParamStringify(prop.type)}))`;
  }
}

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
    path.push(`${namespaceRenamed(namespace)}.${removePrefix(type, namespace)}`);
    objWithRef['$ref'] = path.join('/');
  }

  (endpoints
    ? endpoints.map(ep => spec.paths[ep])
    : Object.values(spec.paths))
    .forEach(path => Object.values(path)
      .forEach(op => {
        if (!(op.tags && op.tags.length)) return; // No need to update without tags.
        const namespace = normalizeEndpointName(op.tags[op.tags.length - 1].split(' ').pop());
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
    const newSchemaKey = `${newNamespace}.${removePrefix(schemaKey, namespace)}`;
    if (newSchemaKey === schemaKey)
      continue;
    (schemas[newSchemaKey] = schemas[schemaKey])['x-endpoint'] = newNamespace;
    delete schemas[schemaKey];

    schemasToInclude.add(newSchemaKey);
  }
  return schemasToInclude;
}

module.exports = {
  capitalize,
  decapitalize,
  escapeKeyword,
  removePrefix,
  normalizeEndpointName,
  normalizeSchemaName,
  normalizeArgName,
  normalizePropName,
  stringifyType,
  replaceEnumCasts,
  formatJsonProperty,
  formatAddQueryParam,
  remapNamespaces
};