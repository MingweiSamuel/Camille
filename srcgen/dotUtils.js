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

function stringifyType(prop, endpoint = null, prefixToRemove = '') {
  if (prop.anyOf) {
    prop = prop.anyOf[0];
  }

  let refType = prop['$ref'];
  if (refType) {
    refType = refType.split('/').pop();
    refType = refType.split('.').pop();
    if (refType.startsWith(prefixToRemove))
        refType = refType.slice(prefixToRemove.length);
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
    case 'array': return stringifyType(prop.items, endpoint, prefixToRemove) + '[]';
    case 'object':
      const keyType = prop['x-key'] ? stringifyType(prop['x-key'], endpoint, prefixToRemove) : 'string';
      return `IDictionary<${keyType}, ${stringifyType(prop.additionalProperties, endpoint, prefixToRemove)}>`;
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

function getDtoNamespaceMapping(endpoints, spec) {
  const dtoNamespaceMapping = {};
  endpoints.map(endpoint => spec.paths[endpoint])
    .flatMap(path => Object.values(path))
    .forEach(op => {
      const namespace = normalizeEndpointName(
        op.tags.pop().split(' ').pop()
            .replace('riotclient', 'riot-client')); // TODO Hack.
      Object.values(op.responses || {})
        .map(resp => resp.content).defined()
        .map(cont => cont['application/json']).defined()
        .map(json => json.schema['$ref']).defined()
        .map(ref => ref.split('/').pop())
        .forEach(type => dtoNamespaceMapping[type] = namespace);
    });

  const stack = Object.entries(dtoNamespaceMapping);
  while (stack.length) {
    const [ type, endpoint ] = stack.pop();
    dtoNamespaceMapping[type] = endpoint;

    let schema = spec.components.schemas[type];
    let childRefs = Object.values(schema.properties || {})
      //.map(x => { console.log(x); return x; })
      .flatMap(propVal => [ propVal, propVal.items ]).defined()
      .map(propVal => propVal['$ref']).defined()
      .map(ref => ref.split('/').pop());
    stack.push(...childRefs.map(cr => [ cr, endpoint ]));
  }
  console.log(dtoNamespaceMapping);
  return dtoNamespaceMapping;
}

module.exports = {
  capitalize,
  decapitalize,
  escapeKeyword,
  normalizeEndpointName,
  normalizeSchemaName,
  normalizeArgName,
  normalizePropName,
  stringifyType,
  replaceEnumCasts,
  formatJsonProperty,
  formatAddQueryParam,
  getDtoNamespaceMapping
};