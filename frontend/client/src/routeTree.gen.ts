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
import { Route as CommunitiesCommunityIdIndexImport } from './routes/communities/$communityId/index'

// Create Virtual Routes

const IndexLazyImport = createFileRoute('/')()
const CommunitiesAddIndexLazyImport = createFileRoute('/communities/add/')()
const CommunitiesCommunityIdEditIndexLazyImport = createFileRoute(
  '/communities/$communityId/edit/',
)()
const CommunitiesCommunityIdPostsAddIndexLazyImport = createFileRoute(
  '/communities/$communityId/posts/add/',
)()
const CommunitiesCommunityIdPostsPostIdIndexLazyImport = createFileRoute(
  '/communities/$communityId/posts/$postId/',
)()
const CommunitiesCommunityIdPostsPostIdEditIndexLazyImport = createFileRoute(
  '/communities/$communityId/posts/$postId/edit/',
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

const CommunitiesAddIndexLazyRoute = CommunitiesAddIndexLazyImport.update({
  id: '/communities/add/',
  path: '/communities/add/',
  getParentRoute: () => rootRoute,
} as any).lazy(() =>
  import('./routes/communities/add/index.lazy').then((d) => d.Route),
)

const CommunitiesCommunityIdIndexRoute =
  CommunitiesCommunityIdIndexImport.update({
    id: '/communities/$communityId/',
    path: '/communities/$communityId/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import('./routes/communities/$communityId/index.lazy').then((d) => d.Route),
  )

const CommunitiesCommunityIdEditIndexLazyRoute =
  CommunitiesCommunityIdEditIndexLazyImport.update({
    id: '/communities/$communityId/edit/',
    path: '/communities/$communityId/edit/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import('./routes/communities/$communityId/edit/index.lazy').then(
      (d) => d.Route,
    ),
  )

const CommunitiesCommunityIdPostsAddIndexLazyRoute =
  CommunitiesCommunityIdPostsAddIndexLazyImport.update({
    id: '/communities/$communityId/posts/add/',
    path: '/communities/$communityId/posts/add/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import('./routes/communities/$communityId/posts/add/index.lazy').then(
      (d) => d.Route,
    ),
  )

const CommunitiesCommunityIdPostsPostIdIndexLazyRoute =
  CommunitiesCommunityIdPostsPostIdIndexLazyImport.update({
    id: '/communities/$communityId/posts/$postId/',
    path: '/communities/$communityId/posts/$postId/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import('./routes/communities/$communityId/posts/$postId/index.lazy').then(
      (d) => d.Route,
    ),
  )

const CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute =
  CommunitiesCommunityIdPostsPostIdEditIndexLazyImport.update({
    id: '/communities/$communityId/posts/$postId/edit/',
    path: '/communities/$communityId/posts/$postId/edit/',
    getParentRoute: () => rootRoute,
  } as any).lazy(() =>
    import(
      './routes/communities/$communityId/posts/$postId/edit/index.lazy'
    ).then((d) => d.Route),
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
      preLoaderRoute: typeof CommunitiesCommunityIdIndexImport
      parentRoute: typeof rootRoute
    }
    '/communities/add/': {
      id: '/communities/add/'
      path: '/communities/add'
      fullPath: '/communities/add'
      preLoaderRoute: typeof CommunitiesAddIndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/communities/$communityId/edit/': {
      id: '/communities/$communityId/edit/'
      path: '/communities/$communityId/edit'
      fullPath: '/communities/$communityId/edit'
      preLoaderRoute: typeof CommunitiesCommunityIdEditIndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/communities/$communityId/posts/$postId/': {
      id: '/communities/$communityId/posts/$postId/'
      path: '/communities/$communityId/posts/$postId'
      fullPath: '/communities/$communityId/posts/$postId'
      preLoaderRoute: typeof CommunitiesCommunityIdPostsPostIdIndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/communities/$communityId/posts/add/': {
      id: '/communities/$communityId/posts/add/'
      path: '/communities/$communityId/posts/add'
      fullPath: '/communities/$communityId/posts/add'
      preLoaderRoute: typeof CommunitiesCommunityIdPostsAddIndexLazyImport
      parentRoute: typeof rootRoute
    }
    '/communities/$communityId/posts/$postId/edit/': {
      id: '/communities/$communityId/posts/$postId/edit/'
      path: '/communities/$communityId/posts/$postId/edit'
      fullPath: '/communities/$communityId/posts/$postId/edit'
      preLoaderRoute: typeof CommunitiesCommunityIdPostsPostIdEditIndexLazyImport
      parentRoute: typeof rootRoute
    }
  }
}

// Create and export the route tree

export interface FileRoutesByFullPath {
  '/': typeof IndexLazyRoute
  '/communities': typeof CommunitiesIndexRoute
  '/communities/$communityId': typeof CommunitiesCommunityIdIndexRoute
  '/communities/add': typeof CommunitiesAddIndexLazyRoute
  '/communities/$communityId/edit': typeof CommunitiesCommunityIdEditIndexLazyRoute
  '/communities/$communityId/posts/$postId': typeof CommunitiesCommunityIdPostsPostIdIndexLazyRoute
  '/communities/$communityId/posts/add': typeof CommunitiesCommunityIdPostsAddIndexLazyRoute
  '/communities/$communityId/posts/$postId/edit': typeof CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute
}

export interface FileRoutesByTo {
  '/': typeof IndexLazyRoute
  '/communities': typeof CommunitiesIndexRoute
  '/communities/$communityId': typeof CommunitiesCommunityIdIndexRoute
  '/communities/add': typeof CommunitiesAddIndexLazyRoute
  '/communities/$communityId/edit': typeof CommunitiesCommunityIdEditIndexLazyRoute
  '/communities/$communityId/posts/$postId': typeof CommunitiesCommunityIdPostsPostIdIndexLazyRoute
  '/communities/$communityId/posts/add': typeof CommunitiesCommunityIdPostsAddIndexLazyRoute
  '/communities/$communityId/posts/$postId/edit': typeof CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute
}

export interface FileRoutesById {
  __root__: typeof rootRoute
  '/': typeof IndexLazyRoute
  '/communities/': typeof CommunitiesIndexRoute
  '/communities/$communityId/': typeof CommunitiesCommunityIdIndexRoute
  '/communities/add/': typeof CommunitiesAddIndexLazyRoute
  '/communities/$communityId/edit/': typeof CommunitiesCommunityIdEditIndexLazyRoute
  '/communities/$communityId/posts/$postId/': typeof CommunitiesCommunityIdPostsPostIdIndexLazyRoute
  '/communities/$communityId/posts/add/': typeof CommunitiesCommunityIdPostsAddIndexLazyRoute
  '/communities/$communityId/posts/$postId/edit/': typeof CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute
}

export interface FileRouteTypes {
  fileRoutesByFullPath: FileRoutesByFullPath
  fullPaths:
    | '/'
    | '/communities'
    | '/communities/$communityId'
    | '/communities/add'
    | '/communities/$communityId/edit'
    | '/communities/$communityId/posts/$postId'
    | '/communities/$communityId/posts/add'
    | '/communities/$communityId/posts/$postId/edit'
  fileRoutesByTo: FileRoutesByTo
  to:
    | '/'
    | '/communities'
    | '/communities/$communityId'
    | '/communities/add'
    | '/communities/$communityId/edit'
    | '/communities/$communityId/posts/$postId'
    | '/communities/$communityId/posts/add'
    | '/communities/$communityId/posts/$postId/edit'
  id:
    | '__root__'
    | '/'
    | '/communities/'
    | '/communities/$communityId/'
    | '/communities/add/'
    | '/communities/$communityId/edit/'
    | '/communities/$communityId/posts/$postId/'
    | '/communities/$communityId/posts/add/'
    | '/communities/$communityId/posts/$postId/edit/'
  fileRoutesById: FileRoutesById
}

export interface RootRouteChildren {
  IndexLazyRoute: typeof IndexLazyRoute
  CommunitiesIndexRoute: typeof CommunitiesIndexRoute
  CommunitiesCommunityIdIndexRoute: typeof CommunitiesCommunityIdIndexRoute
  CommunitiesAddIndexLazyRoute: typeof CommunitiesAddIndexLazyRoute
  CommunitiesCommunityIdEditIndexLazyRoute: typeof CommunitiesCommunityIdEditIndexLazyRoute
  CommunitiesCommunityIdPostsPostIdIndexLazyRoute: typeof CommunitiesCommunityIdPostsPostIdIndexLazyRoute
  CommunitiesCommunityIdPostsAddIndexLazyRoute: typeof CommunitiesCommunityIdPostsAddIndexLazyRoute
  CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute: typeof CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute
}

const rootRouteChildren: RootRouteChildren = {
  IndexLazyRoute: IndexLazyRoute,
  CommunitiesIndexRoute: CommunitiesIndexRoute,
  CommunitiesCommunityIdIndexRoute: CommunitiesCommunityIdIndexRoute,
  CommunitiesAddIndexLazyRoute: CommunitiesAddIndexLazyRoute,
  CommunitiesCommunityIdEditIndexLazyRoute:
    CommunitiesCommunityIdEditIndexLazyRoute,
  CommunitiesCommunityIdPostsPostIdIndexLazyRoute:
    CommunitiesCommunityIdPostsPostIdIndexLazyRoute,
  CommunitiesCommunityIdPostsAddIndexLazyRoute:
    CommunitiesCommunityIdPostsAddIndexLazyRoute,
  CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute:
    CommunitiesCommunityIdPostsPostIdEditIndexLazyRoute,
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
        "/communities/$communityId/",
        "/communities/add/",
        "/communities/$communityId/edit/",
        "/communities/$communityId/posts/$postId/",
        "/communities/$communityId/posts/add/",
        "/communities/$communityId/posts/$postId/edit/"
      ]
    },
    "/": {
      "filePath": "index.lazy.tsx"
    },
    "/communities/": {
      "filePath": "communities/index.tsx"
    },
    "/communities/$communityId/": {
      "filePath": "communities/$communityId/index.tsx"
    },
    "/communities/add/": {
      "filePath": "communities/add/index.lazy.tsx"
    },
    "/communities/$communityId/edit/": {
      "filePath": "communities/$communityId/edit/index.lazy.tsx"
    },
    "/communities/$communityId/posts/$postId/": {
      "filePath": "communities/$communityId/posts/$postId/index.lazy.tsx"
    },
    "/communities/$communityId/posts/add/": {
      "filePath": "communities/$communityId/posts/add/index.lazy.tsx"
    },
    "/communities/$communityId/posts/$postId/edit/": {
      "filePath": "communities/$communityId/posts/$postId/edit/index.lazy.tsx"
    }
  }
}
ROUTE_MANIFEST_END */
