# Adda Social App

Frontend for **Adda**, a dating/social SPA in the Adda monorepo. Users browse members, bookmark profiles, exchange messages (including real-time chat), and manage their profile. Admins and moderators can manage users and photos.

## Stack

| Layer | Technology |
|-------|------------|
| Framework | Angular 21 (standalone components) |
| UI | Bootstrap 5.3, Bootswatch Vapor, ngx-bootstrap |
| Icons / toasts | Font Awesome 6, Alertify |
| Auth | JWT (`@auth0/angular-jwt`) |
| Realtime | SignalR (`@microsoft/signalr`) |
| State | Services + RxJS |

## Prerequisites

- Node.js and npm
- Backend API running at the URL in [`src/environments/environment.ts`](src/environments/environment.ts) (default: `https://localhost:44392`)

## Setup

```bash
npm install
npm start
```

Open [http://localhost:4200/](http://localhost:4200/). The dev server reloads on file changes.

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Dev server (`ng serve`) |
| `npm run build` | Production build → `dist/adda-app` |
| `npm run watch` | Development build with watch |
| `npm test` | Unit tests (Karma + Jasmine) |

## Features & routes

| Route | Feature | Access |
|-------|---------|--------|
| `/` | Home / landing | Public |
| `/login` | Sign in | Public |
| `/register` | Sign up | Public |
| `/members` | Member discovery | Authenticated |
| `/members/:id` | Profile detail + chat | Authenticated |
| `/member/edit` | Edit profile & photos | Authenticated |
| `/lists` | Bookmarks | Authenticated |
| `/messages` | Inbox / outbox / unread | Authenticated |
| `/admin` | User & photo management | Admin, Moderator |

## Project layout

```
src/app/
├── nav/              Global navbar and auth
├── home/             Landing page
├── register/         Registration
├── members/          List, card, detail, edit, photo editor, chat
├── lists/            Bookmarks
├── messages/         Message inbox
├── admin/            Admin panel, user/photo management, roles modal
├── shared/           Global loader
├── services/         auth, user, chat, admin, alertify, loader
├── guards/           auth, unsaved-changes
├── resolver/         Route data resolvers
├── pipes/            dateAgo, isInvalid, hasError
├── directives/       hasRole
└── models/           user, photo, message, pagination
```

Static assets live in [`public/`](public/) (logo, home image, default avatar, favicons).

## Environments

- [`src/environments/environment.ts`](src/environments/environment.ts) — development
- [`src/environments/environment.prod.ts`](src/environments/environment.prod.ts) — production

Both define `apiUrl` for the backend. Production builds replace the dev file via `angular.json` file replacements.

## UI modernization

The app uses Bootstrap + ngx-bootstrap with an Adda-branded token layer in [`src/styles.css`](src/styles.css). High-traffic surfaces (home, nav, members, messages, admin) are being refreshed incrementally while keeping the existing component library.
