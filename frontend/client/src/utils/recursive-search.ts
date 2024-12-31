const recursiveSearch = <T, TKey>(
  nodes: T[],
  childFn: (item: T) => T[],
  searchFn: (item: T) => TKey,
  search: TKey,
): T | null => {
  for (const node of nodes) {
    if (searchFn(node) === search) {
      return node;
    }

    const found = recursiveSearch(childFn(node), childFn, searchFn, search);
    if (found) {
      return found;
    }
  }

  return null;
};

export default recursiveSearch;
