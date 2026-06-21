const CACHE = 'expenses-v2';
const PRECACHE = ['/manifest.json', '/icon.svg'];

self.addEventListener('install', e => {
  e.waitUntil(caches.open(CACHE).then(c => c.addAll(PRECACHE)));
  self.skipWaiting();
});

self.addEventListener('activate', e => {
  e.waitUntil(caches.keys().then(keys =>
    Promise.all(keys.filter(k => k !== CACHE).map(k => caches.delete(k)))
  ));
  self.clients.claim();
});

self.addEventListener('fetch', e => {
  const url = new URL(e.request.url);

  // Always network-first for the main HTML — never serve stale app
  if (url.pathname === '/expenses.html' || url.pathname === '/') {
    e.respondWith(
      fetch(e.request).catch(() => caches.match('/expenses.html'))
    );
    return;
  }

  // Network-first for Firebase and external APIs
  if (url.hostname.includes('firebase') || url.hostname.includes('googleapis') ||
      url.hostname.includes('er-api') || url.hostname.includes('gstatic') ||
      url.hostname.includes('jsdelivr') || url.hostname.includes('fonts')) {
    e.respondWith(fetch(e.request).catch(() => caches.match(e.request)));
    return;
  }

  // Cache-first for icons and manifest
  e.respondWith(caches.match(e.request).then(r => r || fetch(e.request)));
});
