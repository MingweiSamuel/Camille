/*
 * Generates source code via doT.js templates.
 *
 * Usage: node index.js [root directory]
 *
 * Root directory is an optional argument specifying where to search for .dt files.
 * If it is not provided the script will look in the current working directory.
 */
const util = require('util');
const fs = require('fs');
fs.readFileAsync = util.promisify(fs.readFile);
fs.writeFileAsync = util.promisify(fs.writeFile);
const process = require('process');
const path = require('path');

const doT = require('dot');
const glob = require('glob-promise');

const _ = require('./polyfill');

const log = a => { console.log(a); return a; };
const suffix = '.dt';

const templateNames = [
    'dataClasses',
    'endpointMethods'
];
const templatesPromise = Promise
    .all(templateNames.map(file => fs.readFileAsync(`${__dirname}/${file}${suffix}`, 'utf8')))
    .then(contents => {
        const templates = {};
        for (let i = 0; i < templateNames.length; i++)
            templates[templateNames[i]] = doT.template(contents[i]);
        return templates;
    });

doT.templateSettings = {
  evaluate: /\r?\n?\{\{([\s\S]+?)\}\}/g,
  interpolate: /\r?\n?\{\{=([\s\S]+?)\}\}/g,
  encode: /\r?\n?\{\{!([\s\S]+?)\}\}/g,
  use: /\r?\n?\{\{#([\s\S]+?)\}\}/g,
  define: /\r?\n?\{\{##\s*([\w\.$]+)\s*(\:|=)([\s\S]+?)#\}\}/g,
  conditional: /\r?\n?\{\{\?(\?)?\s*([\s\S]*?)\s*\}\}/g,
  iterate: /\r?\n?\{\{~\s*(?:\}\}|([\s\S]+?)\s*\:\s*([\w$]+)\s*(?:\:\s*([\w$]+))?\s*\}\})/g,
  varname: 'it',
  strip: false,
  append: false,
  selfcontained: false
};

global.require = require;

const dir = process.argv[2] || process.cwd();

glob
  .promise(dir + '/**/*' + suffix, { ignore: [
    __dirname + '/**',
    '**/node_modules/**',
    '**/bin/**',
    '**/obj/**',
    '**/Properties/**',
  ] })
  .then(files => Promise.all(files
    .map(log)
    .map(file => Promise.all([ templatesPromise, fs.readFileAsync(file, 'utf8') ])
      .then(([ templates, input ])=> {
        try {
          return doT.template(input)({
            path: path.dirname(file),
            templates,
          });
        }
        catch (e) {
          console.error(`Error thrown while running "${file}":`, e);
        }
      })
      .then(output => fs.writeFileAsync(file.slice(0, -suffix.length), output, 'utf8'))
    )
  ))
  .catch(console.error);
