function trampoline(f) {
  while (f && f instanceof Function) {
    f = f();
  }
  return f;
}

function sum(n, current = 0) {
  if (n === 0) {
    return current;
  }
  return () => sum(n - 1, current + n);
}

const result = trampoline(sum(300000));
console.log(result);
