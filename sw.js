const CACHE = 'expenses-v3';
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

  // HTML — never cache, always network
  if (url.pathname.endsWith('.html') || url.pathname === '/') {
    e.respondWith(fetch(e.request));
    return;
  }

  // External (Firebase, CDN, fonts, FX) — network only
  if (url.hostname !== self.location.hostname) {
    e.respondWith(fetch(e.request));
    return;
  }

  // Static assets (icon, manifest) — cache first
  e.respondWith(caches.match(e.request).then(r => r || fetch(e.request)));
});
