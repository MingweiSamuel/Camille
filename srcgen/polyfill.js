// flatMap: https://gist.github.com/samgiles/762ee337dff48623e729
// [B](f: (A) ⇒ [B]): [B]  ; Although the types in the arrays aren't strict (:
Array.prototype.flatMap = function(lambda) {
  return Array.prototype.concat.apply([], this.map(lambda));
};

Array.prototype.sortBy = function(lambda) {
  return this.sort((a, b) => {
    const va = lambda(a);
    const vb = lambda(b);
    if ((typeof va) !== (typeof vb))
      throw Error(`Mismatched sort types: ${typeof va}, ${typeof vb}.`);
    if (typeof va === 'number')
      return va - vb;
    if (typeof va === 'string')
      return va.localeCompare(vb);
    throw Error(`Unknown sort type: ${typeof va}.`);
  });
};
