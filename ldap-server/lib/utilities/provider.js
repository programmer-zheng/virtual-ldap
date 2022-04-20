const {
  provider: providerConfig,
} = require('../config');
const log = require('log').get('provider');

let provider = null;

async function createProvider() {
  if (!provider) {
    const name = providerConfig.name;
    console.info('setup provider：', name);
    provider = require('../providers/' + name);
    await provider.setupProvider(providerConfig);
    console.info('Provider init complete...');
  }

  return provider;
};

function getProviderLDAPEntries() {
  if (provider) {
    return provider.getAllLDAPEntries();
  }

  return [];
}

function reloadEntriesFromProvider() {
  if (provider) {
    console.info('Reload entries from provider');
    provider.reloadEntriesFromProvider();
  }
}

module.exports = {
  createProvider,
  getProviderLDAPEntries,
  reloadEntriesFromProvider,
};
