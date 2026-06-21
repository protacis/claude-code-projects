const CACHE = 'expenses-v1';
const PRECACHE = [
  '/expenses.html',
  '/manifest.json',
  '/icon.svg',
];

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
  // network-first for Firebase API calls, cache-first for app shell
  if (e.request.url.includes('firebasedatabase') || e.request.url.includes('firebasejs') ||
      e.request.url.includes('googleapis') || e.request.url.includes('open.er-api')) {
    e.respondWith(fetch(e.request).catch(() => caches.match(e.request)));
  } else {
    e.respondWith(caches.match(e.request).then(r => r || fetch(e.request).then(res => {
      const clone = res.clone();
      caches.open(CACHE).then(c => c.put(e.request, clone));
      return res;
    })));
  }
});
