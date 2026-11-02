# RTS Strategy landing page

Dependency-free Node.js website for Linux Docker hosting. The game APK is supplied separately; this website does not build the game.

## Run on Linux

From this directory:

```sh
mkdir -p downloads
docker compose up -d --build
```

Open `http://YOUR_SERVER_IP:8080`. Configure your server firewall or reverse proxy to expose the site. For a public domain, put an HTTPS reverse proxy in front of port 8080.

## Publish an APK

Copy an actual built APK to `downloads/rts-strategy.apk`. Ensure the file is readable by the container (normally mode 644) and the directory is traversable (normally mode 755). The directory is mounted read-only into the container. Never put signing keys in it.

For an atomic update, upload to another filename in this directory, then rename it:

```sh
chmod 644 downloads/new-build.apk
mv downloads/new-build.apk downloads/rts-strategy.apk
```

Reload the page: it discovers the APK automatically, displays its size and file modification date, and enables the download. No container rebuild or restart is needed. Before an APK exists, the site displays “APK coming soon”; the download endpoint returns 404. Availability is based on the supplied file, not a verification of its contents or Android signature. Publish only a valid, signed APK you intend to distribute.

## Local use

With Node.js 22 or newer installed:

```sh
npm start
```

Open `http://localhost:8080`. Optional environment variables: `PORT` and `APK_PATH` (the absolute path to your APK). Docker sets `APK_PATH=/releases/rts-strategy.apk`.

## Operations

```sh
docker compose logs -f
docker compose down
```

The container runs as an unprivileged user with a read-only filesystem and no npm dependencies. `/healthz` is its health endpoint, `/api/release` returns public APK availability, and `/download` serves the APK as an attachment. There is no upload or administration endpoint.

Page content describes planned game features and labels its tactical illustration as concept art. Update the development and compatibility copy when a release has actually been validated.
