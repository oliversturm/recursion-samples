let outputResult p v = printfn $"%s{p}: %d{v}"

let rec sumTail l r =
    match l with
    | [] -> r
    | x :: xs -> (sumTail xs (r + x))

let longList: list<uint64> = [ 1UL .. 300000UL ]
outputResult "sumTail long" (sumTail longList 0UL)
