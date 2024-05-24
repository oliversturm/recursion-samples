let outputResult p v = printfn $"%s{p}: %d{v}"

let rec sumCps l c : unit =
    match l with
    | [] -> c 0UL
    | x :: xs -> sumCps xs (fun ix -> (c (ix + x)))

let manyValues: list<uint64> = [ 1UL .. 300000UL ]
sumCps manyValues (outputResult "sumCps")
