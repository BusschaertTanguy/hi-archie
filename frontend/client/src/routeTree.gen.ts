/* eslint-disable */

// @ts-nocheck

// noinspection JSUnusedGlobalSymbols

// This file was automatically generated by TanStack Router.
// You should NOT make any changes in this file as it will be overwritten.
// Additionally, you should also exclude this file from your linter and/or formatter to prevent it from being checked or modified.

import { createFileRoute } from '@tanstack/react-router'

// Import Routes

import { Route as rootRoute } from './routes/__root'
import { Route as CommunitiesIndexImport } from './routes/communities/index'

// Create Virtual Routes

const IndexLazyImport = createFileRoute('/')()
const CommunitiesCommunityIdIndexLazyImport = createFileRoute(
  '/communities/$communityId/',
)()

// Create/Update Routes

const IndexLazyRoute = IndexLazyImport.update({
  id: '/',
  path: '/',
  getParentRoute: () => rootRoute,
} as any).lazy(() => import('./routes/index.lazy').then((d) => d.Route))

const CommunitiesIndexRoute = CommunitiesIndexImport.update({
  id: '/communities/',
  path: '/communities/',
  getParentRoute: () => rootRoute,
} as any).lazy(() =>
  import('./routes/communities/index.lazy').then((d) => d.Route),
)

const CommunitiesCommunityIdIndexLazyRoute =
  CommunitiesCommunityIdIndexLazyImport.update({
    id: '/communities/$communityId/',
    path: '/communities/$communityId/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import('./routes/communities/$communityId/index.lazy').then((d) => d.Route),
  )

// Populate the FileRoutesByPath interface

declare module '@tanstack/react-router' {
  interface FileRoutesByPath {
    '/': {
      id: '/'
      path: '/'
      fullPath: '/'
      preLoaderRoute: typeof IndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/communities/': {
      id: '/communities/'
      path: '/communities'
      fullPath: '/communities'
      preLoaderRoute: typeof CommunitiesIndexImport
      parentRoute: typeof rootRoute
    }
    '/communities/$communityId/': {
      id: '/communities/$communityId/'
      path: '/communities/$communityId'
      fullPath: '/communities/$communityId'
      preLoaderRoute: typeof CommunitiesCommunityIdIndexLazyImport
      parentRoute: typeof rootRoute
    }
  }
}

// Create and export the route tree

export interface FileRoutesByFullPath {
  '/': typeof IndexLazyRoute
  '/communities': typeof CommunitiesIndexRoute
  '/communities/$communityId': typeof CommunitiesCommunityIdIndexLazyRoute
}

export interface FileRoutesByTo {
  '/': typeof IndexLazyRoute
  '/communities': typeof CommunitiesIndexRoute
  '/communities/$communityId': typeof CommunitiesCommunityIdIndexLazyRoute
}

export interface FileRoutesById {
  __root__: typeof rootRoute
  '/': typeof IndexLazyRoute
  '/communities/': typeof CommunitiesIndexRoute
  '/communities/$communityId/': typeof CommunitiesCommunityIdIndexLazyRoute
}

export interface FileRouteTypes {
  fileRoutesByFullPath: FileRoutesByFullPath
  fullPaths: '/' | '/communities' | '/communities/$communityId'
  fileRoutesByTo: FileRoutesByTo
  to: '/' | '/communities' | '/communities/$communityId'
  id: '__root__' | '/' | '/communities/' | '/communities/$communityId/'
  fileRoutesById: FileRoutesById
}

export interface RootRouteChildren {
  IndexLazyRoute: typeof IndexLazyRoute
  CommunitiesIndexRoute: typeof CommunitiesIndexRoute
  CommunitiesCommunityIdIndexLazyRoute: typeof CommunitiesCommunityIdIndexLazyRoute
}

const rootRouteChildren: RootRouteChildren = {
  IndexLazyRoute: IndexLazyRoute,
  CommunitiesIndexRoute: CommunitiesIndexRoute,
  CommunitiesCommunityIdIndexLazyRoute: CommunitiesCommunityIdIndexLazyRoute,
}

export const routeTree = rootRoute
  ._addFileChildren(rootRouteChildren)
  ._addFileTypes<FileRouteTypes>()

/* ROUTE_MANIFEST_START
{
  "routes": {
    "__root__": {
      "filePath": "__root.tsx",
      "children": [
        "/",
        "/communities/",
        "/communities/$communityId/"
      ]
    },
    "/": {
      "filePath": "index.lazy.tsx"
    },
    "/communities/": {
      "filePath": "communities/index.tsx"
    },
    "/communities/$communityId/": {
      "filePath": "communities/$communityId/index.lazy.tsx"
    }
  }
}
ROUTE_MANIFEST_END */
