let outputResult p v = printfn $"%s{p}: %d{v}"

let rec sumRecTail l r =
    match l with
    | [] -> r
    | x :: xs -> (sumRecTail xs (r + x))

let longList: list<uint64> = [ 1UL .. 300000UL ]
outputResult "sumRecTail long" (sumRecTail longList 0UL)
