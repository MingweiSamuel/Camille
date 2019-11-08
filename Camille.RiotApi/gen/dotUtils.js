// flatMap: https://gist.github.com/samgiles/762ee337dff48623e729
// [B](f: (A) ⇒ [B]): [B]  ; Although the types in the arrays aren't strict (:
Array.prototype.flatMap = function(lambda) {
  return Array.prototype.concat.apply([], this.map(lambda));
};

Array.prototype.sortBy = function(lambda) {
  return this.sort((a, b) => {
    const va = lambda(a);
    const vb = lambda(b);
    if ((typeof va) !== (typeof vb))
      throw Error(`Mismatched sort types: ${typeof va}, ${typeof vb}.`);
    if (typeof va === 'number')
      return va - vb;
    if (typeof va === 'string')
      return va.localeCompare(vb);
    throw Error(`Unknown sort type: ${typeof va}.`);
  });
};

function capitalize(input) {
  return input[0].toUpperCase() + input.slice(1);
}

function decapitalize(input) {
  return input[0].toLowerCase() + input.slice(1);
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
  if (prop.anyOf) {
    prop = prop.anyOf[0];
  }

  let refType = prop['$ref'];
  if (refType) {
    return (!endpoint ? '' : endpoint + '.') +
      normalizeSchemaName(refType.slice(refType.indexOf('.') + 1));
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
      return 'IDictionary<' + stringifyType(prop['x-key'], endpoint) + ', ' +
        stringifyType(prop.additionalProperties, endpoint) + '>';
    default: return prop.type;
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

module.exports = {
  capitalize,
  decapitalize,
  normalizeEndpointName,
  normalizeSchemaName,
  normalizeArgName,
  normalizePropName,
  stringifyType,
  replaceEnumCasts,
  formatJsonProperty,
  formatAddQueryParam
};